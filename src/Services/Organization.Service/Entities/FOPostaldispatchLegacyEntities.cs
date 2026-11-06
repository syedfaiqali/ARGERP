using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Organization.Service.Entities
{
    [Table("FOPostaldispatch")]
    public class FOPostaldispatchLegacy
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("postalno")]
        [MaxLength(50)]
        public string? PostalNo { get; set; }

        [Column("postaldate")]
        public DateTime? PostalDate { get; set; }

        /// <summary>
        /// 226 = Dispatch, 227 = Receive (from Lov vtype = PTYPE, GenEnum.PostalType)
        /// </summary>
        [Column("postaltypeid")]
        public int? PostalTypeId { get; set; }

        [Column("courierid")]
        public int? CourierId { get; set; }

        [Column("title")]
        [MaxLength(250)]
        public string? Title { get; set; }

        [Column("fromperson")]
        [MaxLength(250)]
        public string? FromPerson { get; set; }

        [Column("toperson")]
        [MaxLength(250)]
        public string? ToPerson { get; set; }

        [Column("address")]
        [MaxLength(500)]
        public string? Address { get; set; }

        [Column("refno")]
        [MaxLength(100)]
        public string? RefNo { get; set; }

        [Column("attention")]
        [MaxLength(250)]
        public string? Attention { get; set; }

        // NOTE: legacy column is "Cellno" (capital C) — keep exact casing
        [Column("Cellno")]
        [MaxLength(20)]
        public string? CellNo { get; set; }

        [Column("note")]
        [MaxLength(1500)]
        public string? Note { get; set; }

        [Column("entityid")]
        public int? EntityId { get; set; }

        [Column("clientid")]
        public int? ClientId { get; set; }

        [Column("utblock")]
        public bool UtBlock { get; set; } = false;

        [Column("utispartial")]
        public bool UtIsPartial { get; set; } = false;

        [Column("utsysgen")]
        public bool UtSysGen { get; set; } = false;

        [Column("utrecordversion")]
        public int UtRecordVersion { get; set; } = 1;

        // Navigation property — Courier is stored in Lov table (vtype = "COURI")
        [ForeignKey("CourierId")]
        public virtual Lov? Courier { get; set; }
    }
}
