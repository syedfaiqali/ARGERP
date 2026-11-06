using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Organization.Service.Entities
{
    [Table("SMAcademicYear")]
    public class AcademicYearLegacy
    {
        [Key]
        public int Id { get; set; }
        public string? code { get; set; }
        public string? stxt { get; set; }
        public string? ltxt { get; set; }
        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set; }
        public int? clientid { get; set; }
        public bool active { get; set; }
        public int? nooftest { get; set; }
        public int? noofterms { get; set; }
        public bool sessionnotvisiable { get; set; }
        public int? registerid { get; set; }
        public int? semlovid { get; set; }
        public int? coursecatid { get; set; }
        public bool utsysgen { get; set; } = false;
        public bool utblock { get; set; } = false;
        public bool utispartial { get; set; } = false;
        public int utrecordversion { get; set; } = 0;

        [ForeignKey("registerid")]
        public virtual AcademicRegisterLegacy? SMAcademicregister { get; set; }
        
        public virtual ICollection<AcademicYearDtlLegacy> SMAcademicYeardtls { get; set; } = new List<AcademicYearDtlLegacy>();
    }

    [Table("SMAcademicYeardtl")]
    public class AcademicYearDtlLegacy
    {
        [Key]
        public int Id { get; set; }
        public int acadyrid { get; set; }
        public int? lineindex { get; set; }
        public int? year { get; set; }
        public int? monthid { get; set; }
        public string? periodstxt { get; set; }
        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set; }
        public bool utsysgen { get; set; } = false;
        public bool utblock { get; set; } = false;
        public bool utispartial { get; set; } = false;
        public int utrecordversion { get; set; } = 0;

        [ForeignKey("acadyrid")]
        public virtual AcademicYearLegacy? AcademicYear { get; set; }
    }
}
