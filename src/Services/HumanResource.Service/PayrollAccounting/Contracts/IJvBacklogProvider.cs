using HumanResource.Service.PayrollAccounting.Commands;

namespace HumanResource.Service.PayrollAccounting.Contracts;

/// <summary>
/// A document that should have a system JV but does not.
/// </summary>
public sealed record JvBacklogItem(
	string Source,
	string SourceId,
	string? SourceNo,
	DateTime DocumentDate,
	decimal Amount,
	string Reason);

public sealed record JvBacklogPostResult(string SourceId, bool Succeeded, JvPostingResult? Result, string? Error);

/// <summary>
/// Document side vs JV side for one source over a period. Difference is DocumentTotal - JvTotal;
/// BacklogCount documents have no JV at all.
/// </summary>
public sealed record JvReconciliationLine(
	string Source,
	string Label,
	int DocumentCount,
	decimal DocumentTotal,
	decimal JvTotal,
	int BacklogCount)
{
	public decimal Difference => DocumentTotal - JvTotal;
}

/// <summary>
/// Implemented by each module (one per source code) to list unposted documents and build their JV.
/// </summary>
public interface IJvBacklogProvider
{
	/// <summary>Source code from <see cref="JvSources"/> this provider serves.</summary>
	string Source { get; }

	Task<IReadOnlyList<JvBacklogItem>> GetBacklogAsync(
		int clientId, int entityId, DateTime? from, DateTime? to, CancellationToken cancellationToken);

	/// <summary>Builds the JV for one document, or throws when the document cannot be posted.</summary>
	Task<PostJvCommand> BuildPostingAsync(int clientId, int entityId, string sourceId, CancellationToken cancellationToken);

	/// <summary>Called after the JV was posted, e.g. to set the document's jvpost flag.</summary>
	Task OnPostedAsync(int clientId, string sourceId, JvPostingResult result, CancellationToken cancellationToken);

	/// <summary>Totals of the documents that should be posted against the JVs that carry them.</summary>
	Task<JvReconciliationLine> ReconcileAsync(int clientId, int entityId, DateTime? from, DateTime? to, CancellationToken cancellationToken);
}
