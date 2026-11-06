namespace HumanResource.Service.Entities;

public sealed class ZkImportReceipt
{
    public string SourceKey { get; set; } = "";
    public int TransactionId { get; set; }
    public int AttendanceId { get; set; }
    public DateTime ImportedAtUtc { get; set; }
}
