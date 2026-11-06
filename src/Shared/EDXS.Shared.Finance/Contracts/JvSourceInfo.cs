namespace EDXS.Shared.Finance.Contracts;

/// <summary>
/// Where a JV came from. Used by the JV screen to open the source document (back-link).
/// </summary>
/// <param name="Source">Source code (FCHLN, GRN ...), or null for a manual JV</param>
/// <param name="IsReversal">True when the JV is a "-R" reversal; it still points at the original document</param>
public sealed record JvSourceInfo(
	int TransactionId,
	string Code,
	string? Source,
	string? SourceId,
	string Label,
	string? Module,
	string? ScreenKey,
	bool IsSystemGenerated,
	bool IsReversal)
{
	public bool IsManual => Source == null;

	/// <summary>Document number (e.g. CHL-20261028-512) when the source table is known.</summary>
	public string? SourceNo { get; init; }
}
