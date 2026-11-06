using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Organization.Service.Entities
{
    [Table("FOPurpose")]
    public class FOPurposeLegacy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("id")]
        public int Id { get; set; }

        [Column("code")]
        public string? Code { get; set; }

        [Column("stxt")]
        public string? Stxt { get; set; }

        [Column("ltxt")]
        public string? Ltxt { get; set; }

        [Column("clientid")]
        public int? ClientId { get; set; }

        [Column("utsubdoctype")]
        public int? UtSubDocType { get; set; }

        [Column("utblock")]
        public bool UtBlock { get; set; } = false;

        [Column("utispartial")]
        public bool UtIsPartial { get; set; } = false;

        [Column("utsysgen")]
        public bool UtSysGen { get; set; } = false;

        [Column("utrecordversion")]
        public int UtRecordVersion { get; set; } = 0;
    }
}
