using System.ComponentModel.DataAnnotations;

namespace HumanResource.Service.Entities
{
    public class HRDocTypeLegacy
    {
        public int Id { get; set; }
        [MaxLength(10)]
        public string? Code { get; set; }
        [MaxLength(150)]
        public string? Stxt { get; set; }
        [MaxLength(500)]
        public string? Ltxt { get; set; }
        public int? ClientId { get; set; }
        public int? UtDocType { get; set; }
        public int? UtSubDocType { get; set; }
        public bool? UtBlock { get; set; }
    }
}
