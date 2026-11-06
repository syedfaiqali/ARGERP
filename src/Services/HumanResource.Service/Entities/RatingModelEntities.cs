using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HumanResource.Service.Entities
{
    public class ERSkillratinghdrLegacy
    {
        public int Id { get; set; }
        [MaxLength(10)]
        public string? Code { get; set; }
        [MaxLength(150)]
        public string? Stxt { get; set; }
        public int? EntityId { get; set; }
        public int? ClientId { get; set; }
        public bool? UtBlock { get; set; }
        
        public ICollection<ERSkillratingdtlLegacy> Details { get; set; } = new List<ERSkillratingdtlLegacy>();
    }

    public class ERSkillratingdtlLegacy
    {
        public int Id { get; set; }
        public int? RatingId { get; set; }
        public int? LineIndex { get; set; }
        [MaxLength(200)]
        public string? Code { get; set; }
        [MaxLength(100)]
        public string? Key { get; set; }
        [MaxLength(255)]
        public string? Stxt { get; set; }
        public double? Factor { get; set; }
        public bool? UtBlock { get; set; }
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public bool? Delete { get; set; }
        public ERSkillratinghdrLegacy? Header { get; set; }
    }
}
