using HumanResource.Service.PayrollAccounting.Data;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.PayrollAccounting.Engine;

/// <summary>
/// The "&lt;SOURCE&gt;:&lt;SourceId&gt;" value stored in FITransactionMaster.referenceno, and the
/// "-R" reversal code convention.
/// </summary>
public static class JvReference
{
	public const int JvVoucherTypeId = 1;
	public const int DebitTransType = 1093;
	public const int CreditTransType = 1094;
	public const string ReversalSuffix = "-R";

	public static string Build(string source, string sourceId) => $"{source.Trim().ToUpperInvariant()}:{sourceId.Trim()}";

	public static bool TryParse(string? referenceNo, out string source, out string sourceId)
	{
		source = sourceId = string.Empty;
		if (string.IsNullOrWhiteSpace(referenceNo)) return false;

		var separator = referenceNo.IndexOf(':');
		if (separator <= 0 || separator == referenceNo.Length - 1) return false;

		source = referenceNo[..separator].Trim().ToUpperInvariant();
		sourceId = referenceNo[(separator + 1)..].Trim();
		return source.Length > 0 && sourceId.Length > 0;
	}

	public static bool IsReversalCode(string? code) =>
		code != null && code.EndsWith(ReversalSuffix, StringComparison.OrdinalIgnoreCase);

	/// <summary>
	/// Which of the given references already have an active JV. Used by backlog providers to
	/// filter candidate documents in bulk.
	/// </summary>
	public static async Task<HashSet<string>> FindActiveAsync(
		JvPostingDbContext db, int clientId, IReadOnlyCollection<string> references, CancellationToken cancellationToken)
	{
		var active = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (var chunk in references.Distinct().Chunk(500))
		{
			var found = await db.TransactionMasters
				.Where(m => m.ClientId == clientId
					&& m.VoucherTypeId == JvVoucherTypeId
					&& m.SystemGenerated
					&& !m.Blocked
					&& m.ReferenceNo != null && chunk.Contains(m.ReferenceNo)
					&& m.Code != null
					&& !m.Code.EndsWith(ReversalSuffix)
					&& !db.TransactionMasters.Any(r => r.ClientId == m.ClientId
						&& r.VoucherTypeId == JvVoucherTypeId
						&& !r.Blocked
						&& r.Code == m.Code + ReversalSuffix))
				.Select(m => m.ReferenceNo!)
				.ToListAsync(cancellationToken);
			active.UnionWith(found);
		}
		return active;
	}

	/// <summary>Amount (FITransactionMaster.totalamount) of the active JV of each reference that has one.</summary>
	public static async Task<Dictionary<string, decimal>> ActiveAmountsAsync(
		JvPostingDbContext db, int clientId, IReadOnlyCollection<string> references, CancellationToken cancellationToken)
	{
		var amounts = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);
		foreach (var chunk in references.Distinct().Chunk(500))
		{
			var found = await db.TransactionMasters
				.Where(m => m.ClientId == clientId
					&& m.VoucherTypeId == JvVoucherTypeId
					&& m.SystemGenerated
					&& !m.Blocked
					&& m.ReferenceNo != null && chunk.Contains(m.ReferenceNo)
					&& m.Code != null
					&& !m.Code.EndsWith(ReversalSuffix)
					&& !db.TransactionMasters.Any(r => r.ClientId == m.ClientId
						&& r.VoucherTypeId == JvVoucherTypeId
						&& !r.Blocked
						&& r.Code == m.Code + ReversalSuffix))
				.Select(m => new { m.ReferenceNo, m.TotalAmount })
				.ToListAsync(cancellationToken);
			foreach (var row in found)
			{
				amounts[row.ReferenceNo!] = Math.Round((decimal)(row.TotalAmount ?? 0), 2);
			}
		}
		return amounts;
	}

	/// <summary>All JVs (posts and reversals) written for a reference. Soft-deleted (utblock) rows are ignored.</summary>
	internal static IQueryable<FiTransactionMaster> ForReference(this IQueryable<FiTransactionMaster> masters, int clientId, string reference) =>
		masters.Where(m => m.ClientId == clientId
			&& m.VoucherTypeId == JvVoucherTypeId
			&& m.SystemGenerated
			&& !m.Blocked
			&& m.ReferenceNo == reference);

	/// <summary>The posted, not reversed JV of a reference (at most one by construction).</summary>
	internal static IQueryable<FiTransactionMaster> ActiveFor(this JvPostingDbContext db, int clientId, string reference) =>
		db.TransactionMasters
			.ForReference(clientId, reference)
			.Where(m => m.Code != null
				&& !m.Code.EndsWith(ReversalSuffix)
				&& !db.TransactionMasters.Any(r => r.ClientId == m.ClientId
					&& r.VoucherTypeId == JvVoucherTypeId
					&& !r.Blocked
					&& r.Code == m.Code + ReversalSuffix));
}
