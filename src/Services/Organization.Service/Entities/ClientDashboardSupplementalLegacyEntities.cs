using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Organization.Service.Entities;

[Table("SMStuAttendance")]
public sealed class ClientStudentAttendanceLegacy
{
    [Key, Column("Id")] public long Id { get; set; }
    [Column("studentid")] public int? StudentId { get; set; }
    [Column("atndate")] public DateTime? AttendanceDate { get; set; }
    [Column("atnactivityid")] public int? ActivityId { get; set; }
    [Column("entityid")] public int? EntityId { get; set; }
    [Column("clientid")] public int? ClientId { get; set; }
    [Column("utblock")] public bool UtBlock { get; set; }
}

[Table("HRDailyAttendance")]
public sealed class ClientEmployeeAttendanceLegacy
{
    [Key, Column("Id")] public long Id { get; set; }
    [Column("employeeid")] public int? EmployeeId { get; set; }
    [Column("atndate")] public DateTime? AttendanceDate { get; set; }
    [Column("entityid")] public int? EntityId { get; set; }
    [Column("isabsent")] public bool IsAbsent { get; set; }
    [Column("utblock")] public bool UtBlock { get; set; }
}

[Table("SMFeecollectionhdr")]
public sealed class ClientFeeCollectionLegacy
{
    [Key, Column("Id")] public long Id { get; set; }
    [Column("clientid")] public int? ClientId { get; set; }
    [Column("entityid")] public int? EntityId { get; set; }
    [Column("transdate")] public DateTime? TransactionDate { get; set; }
    [Column("paidamount")] public double? PaidAmount { get; set; }
    [Column("revcoll")] public bool? IsReversed { get; set; }
    [Column("utblock")] public bool? UtBlock { get; set; }
}

[Table("SMFeecollectiondtl")]
public sealed class ClientFeeCollectionDetailLegacy
{
    [Key, Column("id")] public long Id { get; set; }
    [Column("transid")] public long? TransactionId { get; set; }
    [Column("fineamount")] public double? FineAmount { get; set; }
}

[Table("SMFeechallanhdr")]
public sealed class ClientFeeChallanLegacy
{
    [Key, Column("Id")] public long Id { get; set; }
    [Column("clientid")] public int? ClientId { get; set; }
    [Column("entityid")] public int? EntityId { get; set; }
    [Column("challandate")] public DateTime? ChallanDate { get; set; }
    [Column("balanceamount")] public double? BalanceAmount { get; set; }
    [Column("chlnfineamt")] public double? FineAmount { get; set; }
    [Column("revchln")] public bool? IsReversed { get; set; }
    [Column("utblock")] public bool? UtBlock { get; set; }
}

[Table("FITransactionMaster")]
public sealed class ClientFinancialTransactionMasterLegacy
{
    [Key, Column("Id")] public int Id { get; set; }
    [Column("clientid")] public int? ClientId { get; set; }
    [Column("entityid")] public int? EntityId { get; set; }
    [Column("transdate")] public DateTime TransactionDate { get; set; }
    [Column("utblock")] public bool UtBlock { get; set; }
}

[Table("FITransactionDetail")]
public sealed class ClientFinancialTransactionDetailLegacy
{
    [Key, Column("Id")] public int Id { get; set; }
    [Column("transactionid")] public int? TransactionId { get; set; }
    [Column("ficoaid")] public int? AccountId { get; set; }
    [Column("debit")] public double? Debit { get; set; }
    [Column("credit")] public double? Credit { get; set; }
    [Column("utblock")] public bool UtBlock { get; set; }
}

[Table("SMFeetype")]
public sealed class ClientFeeTypeAccountLegacy
{
    [Key, Column("Id")] public int Id { get; set; }
    [Column("clientid")] public int? ClientId { get; set; }
    [Column("entityid")] public int? EntityId { get; set; }
    [Column("ficoaid")] public int? AccountId { get; set; }
    [Column("utblock")] public bool? UtBlock { get; set; }
}
