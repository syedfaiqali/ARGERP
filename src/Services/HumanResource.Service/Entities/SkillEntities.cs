using System.ComponentModel.DataAnnotations;

namespace HumanResource.Service.Entities
{
    public class ERSkilltypeLegacy
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
        public bool? UtBlock { get; set; }
    }

    public class ERSkillLegacy
    {
        public int Id { get; set; }
        [MaxLength(10)]
        public string? Code { get; set; }
        public int? SkillTypeId { get; set; }
        [MaxLength(150)]
        public string? Stxt { get; set; }
        [MaxLength(500)]
        public string? Ltxt { get; set; }
        public int? EntityId { get; set; }
        public int? ClientId { get; set; }
        public int? SkillRatingId { get; set; }
        public bool? UtBlock { get; set; }
    }

    public class EmployeeSkillLegacy
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public int? SkillTypeId { get; set; }
        public int? SkillId { get; set; }
        [MaxLength(500)]
        public string? Institute { get; set; }
        public int? LevelId { get; set; }
        public int? Rating { get; set; }
        public int? PeriodUnitId { get; set; }
        public int? Period { get; set; }
        public int? Year { get; set; }
        public bool UtSysGen { get; set; }
        public bool UtBlock1 { get; set; }
        public bool UtIsPartial { get; set; }
        public DateTime? UtAddTime { get; set; }
        [MaxLength(25)]
        public string? UtAddUser { get; set; }
        public DateTime? UtEditTime { get; set; }
        [MaxLength(25)]
        public string? UtEditUser { get; set; }
        public int UtRecordVersion { get; set; }
        public bool UtBlock { get; set; }
        public int? SkillRatingId { get; set; }
        [MaxLength(500)]
        public string? Remarks { get; set; }
        public int? SkillRatingDetailId { get; set; }

        public EmployeeProfileLegacy? Employee { get; set; }
        public ERSkilltypeLegacy? SkillType { get; set; }
        public ERSkillLegacy? Skill { get; set; }
        public ERSkillratinghdrLegacy? SkillRating { get; set; }
        public ERSkillratingdtlLegacy? SkillRatingDetail { get; set; }
        public UtLovLegacy? PeriodUnit { get; set; }
    }
}
