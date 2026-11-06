using EDXS.Shared.Finance.Contracts;
using EDXS.Shared.Finance.Data;
using EDXS.Shared.Finance.Engine;
using EDXS.Shared.Finance.Queries;
using EDXS.Shared.Infrastructure.CQRS;
using Microsoft.EntityFrameworkCore;

namespace EDXS.Shared.Finance.Handlers;

public sealed class JvPostingQueryHandlers :
	IQueryHandler<GetJvBySourceQuery, JvTrailItem?>,
	IQueryHandler<GetJvPostingStatusQuery, JvPostingStatus>,
	IQueryHandler<GetJvTrailQuery, IReadOnlyList<JvTrailItem>>,
	IQueryHandler<GetJvBacklogQuery, IReadOnlyList<JvBacklogItem>>,
	IQueryHandler<GetJvSourceQuery, JvSourceInfo?>,
	IQueryHandler<GetJvReconciliationQuery, IReadOnlyList<JvReconciliationLine>>
{
	private readonly DbContext _caller;
	private readonly IJvPostingContextFactory _contextFactory;
	private readonly IEnumerable<IJvBacklogProvider> _backlogProviders;

	public JvPostingQueryHandlers(DbContext caller, IJvPostingContextFactory contextFactory, IEnumerable<IJvBacklogProvider> backlogProviders)
	{
		_caller = caller;
		_contextFactory = contextFactory;
		_backlogProviders = backlogProviders;
	}

	public async Task<JvTrailItem?> Handle(GetJvBySourceQuery request, CancellationToken cancellationToken)
	{
		var trail = await LoadTrailAsync(request.ClientId, request.Source, request.SourceId, cancellationToken);
		return trail.FirstOrDefault(x => x.IsActive);
	}

	public async Task<JvPostingStatus> Handle(GetJvPostingStatusQuery request, CancellationToken cancellationToken)
	{
		var trail = await LoadTrailAsync(request.ClientId, request.Source, request.SourceId, cancellationToken);
		var active = trail.FirstOrDefault(x => x.IsActive);
		if (active != null) return new JvPostingStatus(JvPostingState.Posted, active.TransactionId, active.Code);

		var last = trail.LastOrDefault(x => x.Kind == JvTrailKind.Post);
		return last == null
			? new JvPostingStatus(JvPostingState.NotPosted, null, null)
			: new JvPostingStatus(JvPostingState.Reversed, last.TransactionId, last.Code);
	}

	public Task<IReadOnlyList<JvTrailItem>> Handle(GetJvTrailQuery request, CancellationToken cancellationToken) =>
		LoadTrailAsync(request.ClientId, request.Source, request.SourceId, cancellationToken);

	public async Task<IReadOnlyList<JvBacklogItem>> Handle(GetJvBacklogQuery request, CancellationToken cancellationToken)
	{
		var items = new List<JvBacklogItem>();
		foreach (var provider in _backlogProviders.Where(x => request.Source == null
			|| string.Equals(x.Source, request.Source, StringComparison.OrdinalIgnoreCase)))
		{
			items.AddRange(await provider.GetBacklogAsync(request.ClientId, request.EntityId, request.From, request.To, cancellationToken));
		}
		return items.OrderBy(x => x.DocumentDate).ThenBy(x => x.Source).ToList();
	}

	public async Task<IReadOnlyList<JvReconciliationLine>> Handle(GetJvReconciliationQuery request, CancellationToken cancellationToken)
	{
		var lines = new List<JvReconciliationLine>();
		foreach (var provider in _backlogProviders)
		{
			lines.Add(await provider.ReconcileAsync(request.ClientId, request.EntityId, request.From, request.To, cancellationToken));
		}
		return lines;
	}

	public async Task<JvSourceInfo?> Handle(GetJvSourceQuery request, CancellationToken cancellationToken)
	{
		await using var db = _contextFactory.Create(_caller);
		var master = await db.TransactionMasters
			.AsNoTracking()
			.Where(x => x.Id == request.TransactionId && x.ClientId == request.ClientId && x.VoucherTypeId == JvReference.JvVoucherTypeId)
			.Select(x => new { x.Id, x.Code, x.ReferenceNo, x.SystemGenerated })
			.FirstOrDefaultAsync(cancellationToken);
		if (master == null) return null;

		var isReversal = JvReference.IsReversalCode(master.Code);
		if (!master.SystemGenerated || !JvReference.TryParse(master.ReferenceNo, out var source, out var sourceId))
		{
			return new JvSourceInfo(master.Id, master.Code ?? "", null, null, "Manual", null, null, master.SystemGenerated, isReversal);
		}

		var definition = JvSources.Find(source);
		var numbers = await JvSourceNumbers.ResolveAsync(_caller, [(source, sourceId)], cancellationToken);
		return new JvSourceInfo(
			master.Id,
			master.Code ?? "",
			source,
			sourceId,
			definition?.Label ?? source,
			definition?.Module,
			definition?.ScreenKey,
			true,
			isReversal)
		{
			SourceNo = numbers.GetValueOrDefault((source, sourceId)),
		};
	}

	private async Task<IReadOnlyList<JvTrailItem>> LoadTrailAsync(int clientId, string source, string sourceId, CancellationToken cancellationToken)
	{
		var reference = JvReference.Build(source, sourceId);
		await using var db = _contextFactory.Create(_caller);

		var masters = await db.TransactionMasters
			.AsNoTracking()
			.ForReference(clientId, reference)
			.OrderBy(x => x.Id)
			.Select(x => new
			{
				x.Id, x.Code, x.TransactionDate, x.TotalAmount, x.RecordVersion, x.Description,
				x.AddedOn, x.AddedBy, x.EditedOn, x.EditedBy,
			})
			.ToListAsync(cancellationToken);

		var reversedCodes = masters
			.Where(x => JvReference.IsReversalCode(x.Code))
			.Select(x => x.Code![..^JvReference.ReversalSuffix.Length])
			.ToHashSet(StringComparer.OrdinalIgnoreCase);

		return masters.Select(x =>
		{
			var isReversal = JvReference.IsReversalCode(x.Code);
			return new JvTrailItem(
				x.Id,
				x.Code ?? "",
				x.TransactionDate,
				(decimal)(x.TotalAmount ?? 0),
				isReversal ? JvTrailKind.Reversal : JvTrailKind.Post,
				!isReversal && x.Code != null && !reversedCodes.Contains(x.Code),
				x.RecordVersion,
				x.Description,
				x.AddedOn,
				x.AddedBy,
				x.EditedOn,
				x.EditedBy);
		}).ToList();
	}
}
