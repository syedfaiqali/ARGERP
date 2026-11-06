namespace HumanResource.Service.PayrollAccounting.Contracts;

/// <summary>
/// A document type that can post a system JV.
/// </summary>
/// <param name="Code">Prefix stored in FITransactionMaster.referenceno ("PAYR:10234")</param>
/// <param name="Label">Display name shown on the JV screen</param>
/// <param name="Module">Owning service/module</param>
/// <param name="ScreenKey">Frontend screen key used for the back-link</param>
public sealed record JvSourceDefinition(string Code, string Label, string Module, string ScreenKey)
{
	/// <summary>Where the document number lives (e.g. HRpayrollprocess.transno), for display next to the JV.</summary>
	public JvSourceNumberColumn? Number { get; init; }
}

/// <summary>Table and columns holding a source document's number; the id column is numeric.</summary>
public sealed record JvSourceNumberColumn(string Table, string IdColumn, string NumberColumn);

/// <summary>
/// The only place source codes are defined. Only HR payroll is supported.
/// </summary>
public static class JvSources
{
	public const string Payroll = "PAYR";

	private static readonly Dictionary<string, JvSourceDefinition> Definitions =
		new JvSourceDefinition[]
		{
		}.ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);

	public static IReadOnlyCollection<JvSourceDefinition> All => Definitions.Values;

	public static bool IsKnown(string? code) => code != null && Definitions.ContainsKey(code);

	public static JvSourceDefinition? Find(string? code) =>
		code != null && Definitions.TryGetValue(code, out var definition) ? definition : null;
}
