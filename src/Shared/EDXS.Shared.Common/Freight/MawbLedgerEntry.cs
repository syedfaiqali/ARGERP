using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDXS.Shared.Common.Freight;

// Freight subledger postings are snapshots of the owner's finance mapping at save time.
[Table("FRJobMAWBLedgerEntry")]
public sealed class MawbLedgerEntry
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public int ClientId { get; set; }
    public int? EntityId { get; set; }
    [MaxLength(32)] public string Component { get; set; } = string.Empty;
    [MaxLength(50)] public string AccountNo { get; set; } = string.Empty;
    [MaxLength(20)] public string OwnerCode { get; set; } = string.Empty;
    [MaxLength(3)] public string AirlineCode { get; set; } = string.Empty;
    [Column(TypeName = "decimal(19,6)")] public decimal Debit { get; set; }
    [Column(TypeName = "decimal(19,6)")] public decimal Credit { get; set; }
}
