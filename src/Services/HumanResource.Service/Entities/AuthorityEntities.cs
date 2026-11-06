using System.ComponentModel.DataAnnotations;

namespace HumanResource.Service.Entities
{
    public class EREmpAuthorityLegacy
    {
        public int Id { get; set; }
        [MaxLength(10)]
        public string? Code { get; set; }
        public int? AuthTypeId { get; set; }
        [MaxLength(150)]
        public string? Stxt { get; set; }
        [MaxLength(500)]
        public string? Ltxt { get; set; }
        [MaxLength(150)]
        public string? Email { get; set; }
        [MaxLength(50)]
        public string? ContactNo { get; set; }
        [MaxLength(50)]
        public string? Phone { get; set; }
        [MaxLength(150)]
        public string? Web { get; set; }
        [MaxLength(50)]
        public string? Fax { get; set; }
        public int? EntityId { get; set; }
        public int? ClientId { get; set; }
        public bool? UtBlock { get; set; }
    }
}
