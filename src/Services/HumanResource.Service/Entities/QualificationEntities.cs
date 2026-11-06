using System.ComponentModel.DataAnnotations;

namespace HumanResource.Service.Entities
{
    public class ERQualificationLegacy
    {
        public int Id { get; set; }
        [MaxLength(10)]
        public string? code { get; set; }
        public int? quatypeid { get; set; }
        [MaxLength(150)]
        public string? stxt { get; set; }
        [MaxLength(500)]
        public string? ltxt { get; set; }
        public int? entityid { get; set; }
        public int? clientid { get; set; }
        public int? lovtypeid { get; set; }
        public bool? utblock { get; set; }
    }

    public class ERQualificationtypeLegacy
    {
        public int Id { get; set; }
        [MaxLength(10)]
        public string? Code { get; set; }
        [MaxLength(150)]
        public string? Stxt { get; set; }
        [MaxLength(500)]
        public string? Ltxt { get; set; }
        public int? EntityId { get; set; }
        public int? ClientId { get; set; }
        public int? LovtypeId { get; set; }
        public bool? UtBlock { get; set; }
    }

}
