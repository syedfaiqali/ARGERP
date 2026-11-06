using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Organization.Service.Entities
{
    [Table("SMAcademicregister")]
    public class AcademicRegisterLegacy
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

        [Column("clientid")]
        public int? clientid { get; set; }

        [Column("utsysgen")]
        public bool utsysgen { get; set; } = false;

        [Column("utblock")]
        public bool utblock { get; set; } = false;

        [Column("utispartial")]
        public bool utispartial { get; set; } = false;

        [Column("utrecordversion")]
        public int utrecordversion { get; set; } = 0;
    }
}
