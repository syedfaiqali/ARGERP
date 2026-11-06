using System.ComponentModel.DataAnnotations;

namespace HumanResource.Service.Entities
{
    public sealed class SMSubjecttypeHrLegacy
    {
        public int Id { get; set; }
        [MaxLength(10)]
        public string? Code { get; set; }
        [MaxLength(200)]
        public string? Stxt { get; set; }
        [MaxLength(500)]
        public string? Ltxt { get; set; }
        public int? ClientId { get; set; }
        public bool? UtBlock { get; set; }
    }

    // SMCourse (a class), mapped only so Teaching Specialization can check class ids.
    public sealed class SMCourseHrLegacy
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public bool UtBlock { get; set; }
    }

    public sealed class EmployeeSubjectRatingLegacy
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public int? SubjectTypeId { get; set; }
        public int? RatingDetailId { get; set; }
        public int? RatingValue { get; set; }
        public int? UtDocType { get; set; }
        public int? UtSubDocType { get; set; }
        public bool UtSysGen { get; set; }
        public bool UtBlock { get; set; }
        public bool UtIsPartial { get; set; }
        public DateTime? UtAddTime { get; set; }
        [MaxLength(25)]
        public string? UtAddUser { get; set; }
        [MaxLength(25)]
        public string? UtAddSystem { get; set; }
        [MaxLength(25)]
        public string? UtAddIp { get; set; }
        [MaxLength(10)]
        public string? UtAddAppVersion { get; set; }
        [MaxLength(10)]
        public string? UtAddDbVersion { get; set; }
        public long? UtAddId { get; set; }
        public int? UtAddSessionId { get; set; }
        public DateTime? UtEditTime { get; set; }
        [MaxLength(25)]
        public string? UtEditUser { get; set; }
        [MaxLength(25)]
        public string? UtEditSystem { get; set; }
        [MaxLength(25)]
        public string? UtEditIp { get; set; }
        [MaxLength(10)]
        public string? UtEditAppVersion { get; set; }
        [MaxLength(10)]
        public string? UtEditDbVersion { get; set; }
        public long? UtEditId { get; set; }
        public int? UtEditSessionId { get; set; }
        public int UtRecordVersion { get; set; }

        public EmployeeProfileLegacy? Employee { get; set; }
        public SMSubjecttypeHrLegacy? SubjectType { get; set; }
        public ERSkillratingdtlLegacy? RatingDetail { get; set; }
    }
}
