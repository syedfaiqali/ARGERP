using System.ComponentModel.DataAnnotations;

namespace HumanResource.Service.Entities
{
    public sealed class EPEarninggrphdrLegacy
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

        public ICollection<EPEarninggrpdtlLegacy> details { get; set; } =
            new List<EPEarninggrpdtlLegacy>();
    }

    public sealed class EPEarninggrpdtlLegacy
    {
        public int Id { get; set; }
        public int? earngrpid { get; set; }
        public int? lineindex { get; set; }
        public int? earntypeid { get; set; }
        public int? ratebasisid { get; set; }
        public int? basisearntypeid { get; set; }
        public double? value { get; set; }
        public bool absnoteffective { get; set; }
        public bool? levnoteffective { get; set; }
        public long? isotpart { get; set; }
        public int? payslipindex { get; set; }
        [MaxLength(100)]
        public string? paysliptxt { get; set; }
        public int? statusid { get; set; }
        public bool utsysgen { get; set; }
        public bool utblock { get; set; }
        public bool utispartial { get; set; }
        public int utrecordversion { get; set; }

        public EPEarninggrphdrLegacy? header { get; set; }
    }
}
