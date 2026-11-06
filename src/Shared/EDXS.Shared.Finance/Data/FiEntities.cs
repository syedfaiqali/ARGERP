namespace EDXS.Shared.Finance.Data;

// Minimal maps of the legacy FI tables, only the columns the posting engine reads or writes.
// Column mapping lives in JvPostingDbContext. No schema changes.

public sealed class FiTransactionMaster
{
	public int Id { get; set; }
	public string? Code { get; set; }
	public DateTime TransactionDate { get; set; }
	public string? Description { get; set; }
	public double? TotalAmount { get; set; }
	public int? VoucherTypeId { get; set; }
	public int? ClientId { get; set; }
	public int? EntityId { get; set; }
	public int? FiscalYearId { get; set; }
	public string? ReferenceNo { get; set; }
	public bool SystemGenerated { get; set; }
	public bool Blocked { get; set; }
	public bool IsPartial { get; set; }
	public int RecordVersion { get; set; }
	public DateTime? AddedOn { get; set; }
	public string? AddedBy { get; set; }
	public string? AddedIp { get; set; }
	public DateTime? EditedOn { get; set; }
	public string? EditedBy { get; set; }
	public string? EditedIp { get; set; }

	public List<FiTransactionDetail> Details { get; set; } = new();
}

public sealed class FiTransactionDetail
{
	public int Id { get; set; }
	public int? TransactionId { get; set; }
	public int? ClientId { get; set; }
	public int? EntityId { get; set; }
	public int? AccountId { get; set; }
	public string? Description { get; set; }
	public double? Debit { get; set; }
	public double? Credit { get; set; }
	public int? TransType { get; set; }
	public bool SystemGenerated { get; set; }
	public bool Blocked { get; set; }
	public bool IsPartial { get; set; }
	public int RecordVersion { get; set; }
	public DateTime? AddedOn { get; set; }
	public string? AddedBy { get; set; }
	public string? AddedIp { get; set; }
	public DateTime? EditedOn { get; set; }
	public string? EditedBy { get; set; }
	public string? EditedIp { get; set; }
}

public sealed class FiChartOfAccount
{
	public int Id { get; set; }
	public string? Title { get; set; }
	public string? AccountCode { get; set; }
	/// <summary>True for leaf (posting) accounts.</summary>
	public bool? Subsidiary { get; set; }
	public bool? Active { get; set; }
	public int? ClientId { get; set; }
	/// <summary>utblock: soft-deleted.</summary>
	public bool Blocked { get; set; }
}

public sealed class FiFiscalYear
{
	public int Id { get; set; }
	public string? Code { get; set; }
	public string? Title { get; set; }
	/// <summary>True = Active (open), false = closed.</summary>
	public bool Status { get; set; }
	public DateTime? StartDate { get; set; }
	public DateTime? EndDate { get; set; }
	public int? ClientId { get; set; }
	public int? EntityId { get; set; }
	public bool Blocked { get; set; }
}

public sealed class FiPolicyHeader
{
	public int Id { get; set; }
	public int? ClientId { get; set; }
	public int? EntityId { get; set; }
	public bool TransactionAllowInBackDate { get; set; }
	public bool CanChangeTransactionDate { get; set; }
}
