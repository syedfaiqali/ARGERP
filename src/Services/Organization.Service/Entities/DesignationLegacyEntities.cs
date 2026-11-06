using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Organization.Service.Entities
{
    [Table("OrgDesignation")]
    public class OrgDesignationLegacy
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("desgcode")]
        public string? desgcode { get; set; }

        [Column("stxt")]
        public string? stxt { get; set; }

        [Column("ltxt")]
        public string? ltxt { get; set; }

        [Column("blank")]
        public bool? blank { get; set; }

        [Column("clientid")]
        public int? clientid { get; set; }

        [Column("utblock")]
        public bool? utblock { get; set; }
    }
}
