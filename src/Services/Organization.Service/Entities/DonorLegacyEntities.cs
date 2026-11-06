using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Organization.Service.Entities
{
    [Table("OrgDonor")]
    public class OrgDonorLegacy
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("code")]
        public string? code { get; set; }

        [Column("stxt")]
        public string? stxt { get; set; }

        [Column("ltxt")]
        public string? ltxt { get; set; }

        [Column("contactno")]
        public string? contactno { get; set; }

        [Column("clientid")]
        public int? clientid { get; set; }

        [Column("entityid")]
        public int? entityid { get; set; }

        [Column("utblock")]
        public bool? utblock { get; set; }
    }
}
