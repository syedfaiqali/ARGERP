using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Organization.Service.Entities
{
    [Table("FOCalllog")]
    public class FOCalllogLegacy
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("logid")]
        [MaxLength(15)]
        public string? LogId { get; set; }

        [Column("calldate")]
        public DateTime? CallDate { get; set; }

        [Column("calltime")]
        public DateTime? CallTime { get; set; }

        [Column("purposetypeid")]
        public int? PurposeTypeId { get; set; }

        [Column("name")]
        [MaxLength(50)]
        public string? Name { get; set; }

        [Column("phoneno")]
        [MaxLength(15)]
        public string? PhoneNo { get; set; }

        [Column("calltypeid")]
        public int? CallTypeId { get; set; }

        [Column("callmediumid")]
        public int? CallMediumId { get; set; }

        [Column("duration")]
        [MaxLength(50)]
        public string? Duration { get; set; }

        [Column("description")]
        [MaxLength(500)]
        public string? Description { get; set; }

        [Column("entityid")]
        public int EntityId { get; set; }

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
        
        [ForeignKey("PurposeTypeId")]
        public FOPurposeLegacy? Purpose { get; set; }
    }
}
