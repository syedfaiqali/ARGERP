namespace EDXS.Shared.Finance.Contracts;

/// <summary>
/// A document type that can post a system JV.
/// </summary>
/// <param name="Code">Prefix stored in FITransactionMaster.referenceno ("FCHLN:10234")</param>
/// <param name="Label">Display name shown on the JV screen</param>
/// <param name="Module">Owning service/module</param>
/// <param name="ScreenKey">Frontend screen key used for the back-link</param>
public sealed record JvSourceDefinition(string Code, string Label, string Module, string ScreenKey)
{
	/// <summary>Where the document number lives (e.g. SMFeechallanhdr.challanno), for display next to the JV.</summary>
	public JvSourceNumberColumn? Number { get; init; }
}

/// <summary>Table and columns holding a source document's number; the id column is numeric.</summary>
public sealed record JvSourceNumberColumn(string Table, string IdColumn, string NumberColumn);

/// <summary>
/// The only place source codes are defined. Add a new module here first.
/// </summary>
public static class JvSources
{
	public const string FeeChallan = "FCHLN";
	public const string BulkFeeChallan = "BFCH";
	public const string FeeCollection = "FCOL";
	public const string Payroll = "PAYR";
	public const string GoodsReceivedNote = "GRN";
	public const string SalesInvoice = "SINV";
	public const string SalesReturn = "SRET";

	private static readonly Dictionary<string, JvSourceDefinition> Definitions =
		new JvSourceDefinition[]
		{
			new(FeeChallan, "Fee Challan", "fees", "FeeChallan") { Number = new("SMFeechallanhdr", "Id", "challanno") },
			new(BulkFeeChallan, "Bulk Fee Challan", "fees", "BulkFeeChallan"),
			new(FeeCollection, "Fee Collection", "fees", "FeeCollection") { Number = new("SMFeecollectionhdr", "Id", "transno") },
			new(Payroll, "Payroll", "hr", "PaySlip") { Number = new("HRpayrollprocess", "Id", "transno") },
			new(GoodsReceivedNote, "Goods Received Note", "inventory", "GoodsReceivedNote") { Number = new("PMGoodsReceivednotehdr", "Id", "grncode") },
			new(SalesInvoice, "Sales Invoice", "inventory", "SalesInvoice") { Number = new("PMSaleshdr", "Id", "invoiceno") },
			new(SalesReturn, "Sales Return", "inventory", "SalesReturn"),
		}.ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);

	public static IReadOnlyCollection<JvSourceDefinition> All => Definitions.Values;

	public static bool IsKnown(string? code) => code != null && Definitions.ContainsKey(code);

	public static JvSourceDefinition? Find(string? code) =>
		code != null && Definitions.TryGetValue(code, out var definition) ? definition : null;
}
