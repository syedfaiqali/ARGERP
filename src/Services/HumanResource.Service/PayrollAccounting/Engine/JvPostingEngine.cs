using HumanResource.Service.PayrollAccounting.Commands;
using HumanResource.Service.PayrollAccounting.Contracts;
using HumanResource.Service.PayrollAccounting.Data;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.PayrollAccounting.Engine;

/// <summary>
/// Writes JVs to FITransactionMaster/FITransactionDetail on the caller's connection and transaction,
/// so a failed JV rolls back the document that triggered it.
///
///  Post    - new JV; a source can have only one active JV
///  Repost  - document edited: the active JV is overwritten in place (same Id/code, lines replaced)
///  Reverse - document voided: a "-R" JV with swapped lines; the original stays intact
/// </summary>
public sealed class JvPostingEngine
{
	private const int AuditLength = 25;
	private const int DescriptionLength = 500;

	private readonly DbContext _caller;
	private readonly IJvPostingContextFactory _contextFactory;
	private readonly IFiPostingLock _postingLock;
	private readonly FiscalYearResolver _fiscalYears;
	private readonly PostingPolicyValidator _policy;
	private readonly IReadOnlyList<IJvPostingDimensionHook> _hooks;
	private readonly TimeProvider _clock;
	private readonly IJvPostingUserContext _user;

	public JvPostingEngine(
		DbContext caller,
		IJvPostingContextFactory contextFactory,
		IFiPostingLock postingLock,
		FiscalYearResolver fiscalYears,
		PostingPolicyValidator policy,
		IEnumerable<IJvPostingDimensionHook> hooks,
		TimeProvider clock,
		IJvPostingUserContext user)
	{
		_caller = caller;
		_contextFactory = contextFactory;
		_postingLock = postingLock;
		_fiscalYears = fiscalYears;
		_policy = policy;
		_hooks = hooks.ToList();
		_clock = clock;
		_user = user;
	}

	public async Task<JvPostingResult> PostAsync(PostJvCommand posting, CancellationToken cancellationToken)
	{
		EnsureCallerTransaction();
		JvLineRules.EnsureValid(posting.Lines);
		var reference = ReferenceOf(posting);
		posting = WithUser(posting);

		await using var db = _contextFactory.Create(_caller);

		await EnsureAccountsAsync(db, posting.Lines, cancellationToken);
		var fiscalYearId = await _fiscalYears.ResolveOpenAsync(db, posting.ClientId, posting.EntityId, posting.TransactionDate, cancellationToken);
		if (posting.EnforceBackDatePolicy)
		{
			await _policy.ValidateTransactionDateAsync(db, posting.TransactionDate, posting.ClientId, posting.EntityId, null, cancellationToken);
		}
		foreach (var hook in _hooks)
		{
			await hook.ValidateAsync(posting, null, cancellationToken);
		}

		await _postingLock.AcquireAsync(db, cancellationToken);

		// Checked under the lock so two parallel posts of one document cannot both pass.
		if (reference != null && await db.ActiveFor(posting.ClientId, reference).AnyAsync(cancellationToken))
		{
			throw new JvPostingException($"{reference} already has an active JV. Edit the document to update it.");
		}

		var code = await ResolveCodeAsync(db, posting, null, cancellationToken);
		var masterId = await FiPostingNumbers.NextMasterIdAsync(db, cancellationToken);
		var nextDetailId = await FiPostingNumbers.NextDetailIdAsync(db, cancellationToken);
		var now = Now();

		var master = new FiTransactionMaster
		{
			Id = masterId,
			Code = code,
			TransactionDate = posting.TransactionDate,
			Description = Truncate(posting.Narration, DescriptionLength),
			TotalAmount = (double)posting.Lines.Sum(x => JvLineRules.Round(x.Debit)),
			VoucherTypeId = JvReference.JvVoucherTypeId,
			ClientId = posting.ClientId,
			EntityId = posting.EntityId,
			FiscalYearId = fiscalYearId,
			ReferenceNo = reference,
			SystemGenerated = posting.SystemGenerated,
			RecordVersion = 1,
			AddedOn = now,
			AddedBy = Truncate(posting.UserName, AuditLength),
			AddedIp = Truncate(posting.IpAddress, AuditLength),
		};
		db.TransactionMasters.Add(master);
		var postedLines = AddDetails(db, master, posting.Lines, ref nextDetailId, posting, now);

		await db.SaveChangesAsync(cancellationToken);
		await RunPostedHooksAsync(posting, master.Id, postedLines, cancellationToken);

		return new JvPostingResult(master.Id, code, JvPostingAction.Posted);
	}

	public async Task<JvPostingResult> RepostAsync(RepostJvCommand command, CancellationToken cancellationToken)
	{
		EnsureCallerTransaction();
		var posting = WithUser(command.NewPosting);
		JvLineRules.EnsureValid(posting.Lines);
		var reference = ReferenceOf(posting);

		await using var db = _contextFactory.Create(_caller);

		var master = await FindForRepostAsync(db, command, reference, cancellationToken);
		if (master == null)
		{
			// The document never got a JV (or its JV was reversed): post a fresh one.
			return await PostAsync(posting, cancellationToken);
		}

		var entityId = master.EntityId ?? posting.EntityId;
		await _fiscalYears.EnsureOpenAsync(db, master, entityId, cancellationToken);
		var fiscalYearId = await _fiscalYears.ResolveOpenAsync(db, posting.ClientId, entityId, posting.TransactionDate, cancellationToken);
		if (posting.EnforceBackDatePolicy)
		{
			await _policy.ValidateTransactionDateAsync(db, posting.TransactionDate, posting.ClientId, entityId, master.TransactionDate, cancellationToken);
		}
		await EnsureAccountsAsync(db, posting.Lines, cancellationToken);
		foreach (var hook in _hooks)
		{
			await hook.ValidateAsync(posting, master.Id, cancellationToken);
		}

		await _postingLock.AcquireAsync(db, cancellationToken);

		var code = string.IsNullOrWhiteSpace(posting.Code) || posting.SystemGenerated
			? master.Code!
			: await ResolveCodeAsync(db, posting, master.Id, cancellationToken);

		var now = Now();
		var editedBy = Truncate(posting.UserName, AuditLength);
		var editedIp = Truncate(posting.IpAddress, AuditLength);

		// Old lines are soft-deleted (utblock = 1), the same way the finance screens delete lines.
		var oldDetails = await db.TransactionDetails
			.Where(x => x.TransactionId == master.Id && !x.Blocked)
			.ToListAsync(cancellationToken);
		if (oldDetails.Count > 0)
		{
			var removedIds = oldDetails.Select(x => x.Id).ToList();
			foreach (var hook in _hooks)
			{
				await hook.OnLinesRemovedAsync(removedIds, cancellationToken);
			}
			foreach (var detail in oldDetails)
			{
				detail.Blocked = true;
				detail.EditedOn = now;
				detail.EditedBy = editedBy;
				detail.EditedIp = editedIp;
			}
		}

		master.Code = code;
		master.TransactionDate = posting.TransactionDate;
		master.Description = Truncate(posting.Narration, DescriptionLength);
		master.TotalAmount = (double)posting.Lines.Sum(x => JvLineRules.Round(x.Debit));
		master.FiscalYearId = fiscalYearId;
		master.RecordVersion += 1;
		master.EditedOn = now;
		master.EditedBy = editedBy;
		master.EditedIp = editedIp;

		var nextDetailId = await FiPostingNumbers.NextDetailIdAsync(db, cancellationToken);
		var postedLines = AddDetails(db, master, posting.Lines, ref nextDetailId, posting, now);

		await db.SaveChangesAsync(cancellationToken);
		await RunPostedHooksAsync(posting, master.Id, postedLines, cancellationToken);

		return new JvPostingResult(master.Id, code, JvPostingAction.Overwritten);
	}

	public async Task<JvPostingResult> ReverseAsync(ReverseJvCommand command, CancellationToken cancellationToken)
	{
		EnsureCallerTransaction();
		EnsureKnownSource(command.Source, command.SourceId);
		var reference = JvReference.Build(command.Source, command.SourceId);

		await using var db = _contextFactory.Create(_caller);

		var original = await db.ActiveFor(command.ClientId, reference)
			.AsNoTracking()
			.Include(x => x.Details.Where(d => !d.Blocked))
			.FirstOrDefaultAsync(cancellationToken)
			?? throw new JvPostingException($"{reference} has no active JV to reverse.");

		var entityId = original.EntityId ?? command.EntityId;
		var date = command.ReversalDate ?? original.TransactionDate;
		var fiscalYearId = await _fiscalYears.ResolveOpenAsync(db, command.ClientId, entityId, date, cancellationToken);

		await _postingLock.AcquireAsync(db, cancellationToken);

		if (!await db.ActiveFor(command.ClientId, reference).AnyAsync(x => x.Id == original.Id, cancellationToken))
		{
			throw new JvPostingException($"JV {original.Code} is already reversed.");
		}

		var code = original.Code + JvReference.ReversalSuffix;
		var masterId = await FiPostingNumbers.NextMasterIdAsync(db, cancellationToken);
		var nextDetailId = await FiPostingNumbers.NextDetailIdAsync(db, cancellationToken);
		var now = Now();
		var narration = string.IsNullOrWhiteSpace(command.Reason)
			? $"Reversal of {original.Code}"
			: $"Reversal of {original.Code}: {command.Reason.Trim()}";

		var reversal = new FiTransactionMaster
		{
			Id = masterId,
			Code = code,
			TransactionDate = date,
			Description = Truncate(narration, DescriptionLength),
			TotalAmount = original.TotalAmount,
			VoucherTypeId = JvReference.JvVoucherTypeId,
			ClientId = command.ClientId,
			EntityId = entityId,
			FiscalYearId = fiscalYearId,
			ReferenceNo = reference,
			SystemGenerated = original.SystemGenerated,
			RecordVersion = 1,
			AddedOn = now,
			AddedBy = Truncate(command.UserName ?? _user.UserName, AuditLength),
			AddedIp = Truncate(command.IpAddress ?? _user.IpAddress, AuditLength),
		};
		db.TransactionMasters.Add(reversal);

		foreach (var line in original.Details.OrderBy(x => x.Id))
		{
			var debit = line.Credit ?? 0;
			db.TransactionDetails.Add(new FiTransactionDetail
			{
				Id = nextDetailId++,
				TransactionId = reversal.Id,
				ClientId = command.ClientId,
				EntityId = entityId,
				AccountId = line.AccountId,
				Description = line.Description,
				Debit = debit,
				Credit = line.Debit ?? 0,
				TransType = debit > 0 ? JvReference.DebitTransType : JvReference.CreditTransType,
				SystemGenerated = reversal.SystemGenerated,
				RecordVersion = 1,
				AddedOn = now,
				AddedBy = reversal.AddedBy,
				AddedIp = reversal.AddedIp,
			});
		}

		await db.SaveChangesAsync(cancellationToken);

		return new JvPostingResult(reversal.Id, code, JvPostingAction.Reversed, original.Id);
	}

	/// <summary>True when the source document currently has a posted, not reversed JV.</summary>
	public async Task<bool> HasActiveAsync(int clientId, string source, string sourceId, CancellationToken cancellationToken)
	{
		await using var db = _contextFactory.Create(_caller);
		return await db.ActiveFor(clientId, JvReference.Build(source, sourceId)).AnyAsync(cancellationToken);
	}

	private void EnsureCallerTransaction()
	{
		if (_caller.Database.CurrentTransaction == null)
		{
			throw new InvalidOperationException(
				"JV posting must run inside the caller's database transaction (send it as a command, or begin a transaction first).");
		}
	}

	private static string? ReferenceOf(PostJvCommand posting)
	{
		if (string.IsNullOrWhiteSpace(posting.Source))
		{
			if (posting.SystemGenerated)
				throw new JvPostingException("A system generated JV needs a source document.");
			return null;
		}

		if (!posting.SystemGenerated)
			throw new JvPostingException("A manual JV cannot carry a source document.");

		EnsureKnownSource(posting.Source, posting.SourceId);
		return JvReference.Build(posting.Source, posting.SourceId!);
	}

	private static void EnsureKnownSource(string? source, string? sourceId)
	{
		if (!JvSources.IsKnown(source))
			throw new JvPostingException($"Unknown JV source '{source}'.");
		if (string.IsNullOrWhiteSpace(sourceId))
			throw new JvPostingException("Source document id is required.");
	}

	private static async Task<FiTransactionMaster?> FindForRepostAsync(
		JvPostingDbContext db, RepostJvCommand command, string? reference, CancellationToken cancellationToken)
	{
		var posting = command.NewPosting;

		if (command.TransactionId is not int transactionId)
		{
			if (reference == null)
				throw new JvPostingException("A manual JV is reposted by its transaction id.");
			return await db.ActiveFor(posting.ClientId, reference).FirstOrDefaultAsync(cancellationToken);
		}

		var master = await db.TransactionMasters.FirstOrDefaultAsync(x => x.Id == transactionId
			&& x.ClientId == posting.ClientId
			&& x.VoucherTypeId == JvReference.JvVoucherTypeId
			&& !x.Blocked, cancellationToken)
			?? throw new JvPostingException($"JV {transactionId} was not found.");

		if (JvReference.IsReversalCode(master.Code))
			throw new JvPostingException($"JV {master.Code} is a reversal and cannot be changed.");
		if (master.SystemGenerated && !posting.SystemGenerated)
			throw new JvPostingException($"JV {master.Code} is system generated. Change it from its source document.");
		if (master.SystemGenerated && master.ReferenceNo != reference)
			throw new JvPostingException($"JV {master.Code} belongs to {master.ReferenceNo}, not {reference}.");
		if (await db.TransactionMasters.AnyAsync(x => x.ClientId == master.ClientId
			&& x.VoucherTypeId == JvReference.JvVoucherTypeId
			&& !x.Blocked
			&& x.Code == master.Code + JvReference.ReversalSuffix, cancellationToken))
			throw new JvPostingException($"JV {master.Code} is reversed and cannot be changed.");

		return master;
	}

	private static async Task EnsureAccountsAsync(JvPostingDbContext db, IReadOnlyList<JvLine> lines, CancellationToken cancellationToken)
	{
		var ids = lines.Select(x => x.AccountId).Distinct().ToList();
		var accounts = await db.Accounts
			.AsNoTracking()
			.Where(x => ids.Contains(x.Id) && !x.Blocked)
			.ToListAsync(cancellationToken);

		var missing = ids.Except(accounts.Select(x => x.Id)).ToList();
		if (missing.Count > 0)
			throw new JvPostingException($"Account(s) not found: {string.Join(", ", missing)}.");

		foreach (var account in accounts)
		{
			var name = $"'{account.Title}' ({account.AccountCode ?? account.Id.ToString()})";
			if (account.Active == false)
				throw new JvPostingException($"Account {name} is inactive.");
			if (account.Subsidiary == false)
				throw new JvPostingException($"Account {name} is a group account. JV lines must use a posting account.");
		}
	}

	private static async Task<string> ResolveCodeAsync(JvPostingDbContext db, PostJvCommand posting, int? currentId, CancellationToken cancellationToken)
	{
		if (posting.SystemGenerated || string.IsNullOrWhiteSpace(posting.Code))
			return await FiPostingNumbers.NextJvCodeAsync(db, posting.ClientId, cancellationToken);

		var code = posting.Code.Trim();
		if (JvReference.IsReversalCode(code))
			throw new JvPostingException($"Codes ending with '{JvReference.ReversalSuffix}' are reserved for reversals.");

		var normalized = code.ToUpperInvariant();
		if (await db.TransactionMasters.AnyAsync(x => x.ClientId == posting.ClientId
			&& x.VoucherTypeId == JvReference.JvVoucherTypeId
			&& x.Id != currentId
			&& !x.Blocked
			&& x.Code != null && x.Code.Trim().ToUpper() == normalized, cancellationToken))
			throw new JvPostingException("Voucher code already exists.");

		return code;
	}

	private static List<JvPostedLine> AddDetails(
		JvPostingDbContext db, FiTransactionMaster master, IReadOnlyList<JvLine> lines, ref int nextDetailId, PostJvCommand posting, DateTime now)
	{
		var posted = new List<JvPostedLine>(lines.Count);
		foreach (var line in lines)
		{
			var debit = JvLineRules.Round(line.Debit);
			var detail = new FiTransactionDetail
			{
				Id = nextDetailId++,
				TransactionId = master.Id,
				ClientId = master.ClientId,
				EntityId = master.EntityId,
				AccountId = line.AccountId,
				Description = Truncate(line.Narration, DescriptionLength),
				Debit = (double)debit,
				Credit = (double)JvLineRules.Round(line.Credit),
				TransType = debit > 0 ? JvReference.DebitTransType : JvReference.CreditTransType,
				SystemGenerated = posting.SystemGenerated,
				RecordVersion = 1,
				AddedOn = now,
				AddedBy = Truncate(posting.UserName, AuditLength),
				AddedIp = Truncate(posting.IpAddress, AuditLength),
			};
			db.TransactionDetails.Add(detail);
			posted.Add(new JvPostedLine(detail.Id, line));
		}
		return posted;
	}

	private async Task RunPostedHooksAsync(PostJvCommand posting, int transactionId, IReadOnlyList<JvPostedLine> lines, CancellationToken cancellationToken)
	{
		foreach (var hook in _hooks)
		{
			await hook.OnLinesPostedAsync(posting, transactionId, lines, cancellationToken);
		}
	}

	private PostJvCommand WithUser(PostJvCommand posting) =>
		posting.UserName != null && posting.IpAddress != null
			? posting
			: posting with { UserName = posting.UserName ?? _user.UserName, IpAddress = posting.IpAddress ?? _user.IpAddress };

	private DateTime Now() => _clock.GetLocalNow().DateTime;

	private static string? Truncate(string? value, int length) =>
		value == null ? null : value.Length <= length ? value : value[..length];
}
