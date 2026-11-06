using System.ComponentModel.DataAnnotations;

namespace HumanResource.Service.Entities
{
    public sealed class EPDeductiongrphdrLegacy
    {
        public int Id { get; set; }
        [MaxLength(10)]
        public string? code { get; set; }
        [MaxLength(100)]
        public string? stxt { get; set; }
        public int? clientid { get; set; }
        public int? utdoctype { get; set; }
        public int? utsubdoctype { get; set; }
        public bool utsysgen { get; set; }
        public bool utblock { get; set; }
        public bool utispartial { get; set; }
        public int utrecordversion { get; set; }

        public ICollection<EPDeductiongrpdtlLegacy> details { get; set; } =
            new List<EPDeductiongrpdtlLegacy>();
    }

    public sealed class EPDeductiongrpdtlLegacy
    {
        public int Id { get; set; }
        public int? dedgrpid { get; set; }
        public int? lineindex { get; set; }
        public int? dedtypeid { get; set; }
        public int? ratebasisid { get; set; }
        public int? basisearntypeid { get; set; }
        public double? value { get; set; }
        public int? payslipindex { get; set; }
        [MaxLength(100)]
        public string? paysliptxt { get; set; }
        public int? statusid { get; set; }
        public bool utsysgen { get; set; }
        public bool utblock { get; set; }
        public bool utispartial { get; set; }
        public int utrecordversion { get; set; }

        public EPDeductiongrphdrLegacy? header { get; set; }
    }
}
