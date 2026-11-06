using System.ComponentModel.DataAnnotations.Schema;

namespace Organization.Service.Entities
{
    [Table("SMPublishdocumenthdr")]
    public class PublishDocumentHdrLegacy
    {
        public int Id { get; set; }
        public string? code { get; set; }
        public int? lovsharedid { get; set; }
        public int? doctypeid { get; set; }
        public string? stxt { get; set; }
        public int? clientid { get; set; }
        public DateTime? publishdate { get; set; }
        public DateTime? validtill { get; set; }
        public DateTime? validfrom { get; set; }
        public int? subjectid { get; set; }
        public int? courseid { get; set; }
        public int? acdyearid { get; set; }
        public int? lovdoccategoryid { get; set; }
        
        public bool utsysgen { get; set; }
        public bool utblock { get; set; }
        public bool utispartial { get; set; }
        public int utrecordversion { get; set; } = 1;

        public virtual ICollection<PublishDocumentDtlLegacy> SMPublishdocumentdtls { get; set; } = new List<PublishDocumentDtlLegacy>();
    }

    [Table("SMPublishdocumentdtl")]
    public class PublishDocumentDtlLegacy
    {
        public int Id { get; set; }
        public int? publishid { get; set; }
        public int? lovshareid { get; set; }
        public int? pclientid { get; set; }
        public int? pentityid { get; set; }
        public int? regionid { get; set; }
        public int? camplvelid { get; set; }
        public int? courseid { get; set; }
        
        public bool utsysgen { get; set; }
        public bool utblock { get; set; }
        public bool utispartial { get; set; }
        public int utrecordversion { get; set; } = 1;

        [ForeignKey("publishid")]
        public virtual PublishDocumentHdrLegacy PublishDocumentHdr { get; set; }
    }
}
