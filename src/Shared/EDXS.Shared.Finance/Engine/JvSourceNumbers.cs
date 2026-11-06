using EDXS.Shared.Finance.Contracts;
using Microsoft.EntityFrameworkCore;

namespace EDXS.Shared.Finance.Engine;

/// <summary>
/// Looks up document numbers (CHL-..., COL-..., PS-...) for JV source references, so the JV
/// screen can show "Fee Challan CHL-20261028-512" instead of a bare id. All services share one
/// database, so any service can resolve any source.
/// </summary>
public static class JvSourceNumbers
{
	private sealed record Row(string SourceId, string? Number);

	public static async Task<Dictionary<(string Source, string SourceId), string>> ResolveAsync(
		DbContext db, IEnumerable<(string Source, string SourceId)> references, CancellationToken cancellationToken)
	{
		var result = new Dictionary<(string, string), string>();
		foreach (var group in references.Distinct().GroupBy(x => x.Source.ToUpperInvariant()))
		{
			if (group.Key == JvSources.BulkFeeChallan)
			{
				// The batch number is the source id itself.
				foreach (var item in group) result[(group.Key, item.SourceId)] = item.SourceId;
				continue;
			}

			var number = JvSources.Find(group.Key)?.Number;
			if (number == null) continue;

			// Ids are parsed to numbers before they reach the SQL text.
			var ids = group.Select(x => long.TryParse(x.SourceId, out var id) ? id : (long?)null)
				.Where(x => x.HasValue).Select(x => x!.Value).Distinct().ToList();
			foreach (var chunk in ids.Chunk(500))
			{
				var sql = $"SELECT CAST({number.IdColumn} AS varchar(30)) AS SourceId, {number.NumberColumn} AS Number " +
					$"FROM {number.Table} WHERE {number.IdColumn} IN ({string.Join(",", chunk)})";
				var rows = await db.Database.SqlQueryRaw<Row>(sql).ToListAsync(cancellationToken);
				foreach (var row in rows.Where(x => !string.IsNullOrWhiteSpace(x.Number)))
				{
					result[(group.Key, row.SourceId)] = row.Number!;
				}
			}
		}
		return result;
	}
}
