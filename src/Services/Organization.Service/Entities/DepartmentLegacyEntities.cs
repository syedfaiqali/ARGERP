using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Organization.Service.Entities
{
    [Table("OrgDepartment")]
    public class OrgDepartmentLegacy
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("depcode")]
        public string? depcode { get; set; }

        [Column("depstxt")]
        public string? depstxt { get; set; }

        [Column("depltxt")]
        public string? depltxt { get; set; }

        [Column("clientid")]
        public int? clientid { get; set; }

        [Column("ficoaid")]
        public int? ficoaid { get; set; }

        [Column("ficoaid_rec")]
        public int? ficoaid_rec { get; set; }

        [Column("utblock")]
        public bool? utblock { get; set; }

        [ForeignKey("ficoaid")]
        public virtual FicoaLegacy? Ficoa { get; set; }

        [ForeignKey("ficoaid_rec")]
        public virtual FicoaLegacy? FicoaRec { get; set; }
    }

    [Table("FICOA_NEW")]
    public class FicoaLegacy
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Title")]
        public string? Title { get; set; }

        [Column("accountcode")]
        public string? AccountCode { get; set; }

        [Column("clientid")]
        public int? clientid { get; set; }

        [Column("Subsidiary")]
        public bool? Subsidiary { get; set; }

        [Column("Active")]
        public bool? Active { get; set; }

        [Column("accountnatureid")]
        public int? AccountNatureId { get; set; }
    }
}
