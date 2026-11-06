namespace EDXS.Shared.Finance.Contracts;

/// <summary>
/// One Dr or Cr line of a JV. Exactly one of Debit/Credit must be positive.
/// </summary>
/// <param name="Dimension">Opaque per-line payload handed to <see cref="IJvPostingDimensionHook"/> (e.g. project dimension)</param>
public sealed record JvLine(int AccountId, decimal Debit, decimal Credit, string? Narration = null, object? Dimension = null)
{
	public static JvLine Dr(int accountId, decimal amount, string? narration = null, object? dimension = null) =>
		new(accountId, amount, 0m, narration, dimension);

	public static JvLine Cr(int accountId, decimal amount, string? narration = null, object? dimension = null) =>
		new(accountId, 0m, amount, narration, dimension);
}
