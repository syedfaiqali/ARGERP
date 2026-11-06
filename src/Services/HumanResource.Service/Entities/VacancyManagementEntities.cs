using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HumanResource.Service.Entities;

public interface IVacancyUtTracked
{
    int UtDocType { get; set; }
    int UtSubDocType { get; set; }
    bool UtSysGen { get; set; }
    bool UtBlock { get; set; }
    bool UtIsPartial { get; set; }
    DateTime UtAddTime { get; set; }
    string UtAddUser { get; set; }
    string UtAddSystem { get; set; }
    string UtAddIp { get; set; }
    string UtAddAppVersion { get; set; }
    string UtAddDbVersion { get; set; }
    long UtAddId { get; set; }
    int? UtAddSessionId { get; set; }
    DateTime UtEditTime { get; set; }
    string UtEditUser { get; set; }
    string UtEditSystem { get; set; }
    string UtEditIp { get; set; }
    string UtEditAppVersion { get; set; }
    string UtEditDbVersion { get; set; }
    long UtEditId { get; set; }
    int? UtEditSessionId { get; set; }
    int UtRecordVersion { get; set; }
}

[Table("HRVacancy")]
public sealed class HRVacancy : IVacancyUtTracked
{
    [Key]
    public int Id { get; set; }
    public int ClientId { get; set; }
    public int EntityId { get; set; }
    [MaxLength(20)] public string VacancyType { get; set; } = "NewHeadcount";
    [MaxLength(20)] public string? ReportsToType { get; set; }
    public int? ReportsToPositionId { get; set; }
    public int? ReplacementPositionId { get; set; }
    [MaxLength(30)] public string VacancyNumber { get; set; } = string.Empty;
    [MaxLength(200)] public string JobTitle { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public int DesignationId { get; set; }
    public int? GradeId { get; set; }
    public int? SiteId { get; set; }
    public int? EmploymentTypeId { get; set; }
    public int? HiringManagerEmployeeId { get; set; }
    public int? ApproverEmployeeId { get; set; }
    [MaxLength(20)] public string Priority { get; set; } = "Normal";
    public int RequestedOpenings { get; set; }
    public DateTime? TargetFillDate { get; set; }
    [Column(TypeName = "decimal(5,2)")] public decimal? MinimumExperienceYears { get; set; }
    [Column(TypeName = "decimal(5,2)")] public decimal? MaximumExperienceYears { get; set; }
    [MaxLength(4000)] public string? JobDescription { get; set; }
    [MaxLength(4000)] public string? Responsibilities { get; set; }
    [MaxLength(4000)] public string? Requirements { get; set; }
    [MaxLength(4000)] public string? RequiredQualifications { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal? SalaryMinimum { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal? SalaryMaximum { get; set; }
    [MaxLength(10)] public string? CurrencyCode { get; set; }
    [Column(TypeName = "decimal(18,2)")] public decimal? HiringBudget { get; set; }
    [MaxLength(20)] public string ApprovalStatus { get; set; } = "Draft";
    [MaxLength(20)] public string? LifecycleStatus { get; set; }
    public int RevisionNumber { get; set; } = 1;
    // Set once a job posting has been raised for this requisition on the One
    // Window Job Portal. The portal keys jobs by Guid, held here as text.
    [MaxLength(100)] public string? JobPortalJobId { get; set; }
    public DateTime? JobPortalPostedAtUtc { get; set; }
    public bool IsDeleted { get; set; }
    [MaxLength(100)] public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    [MaxLength(100)] public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    [Timestamp] public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public int UtDocType { get; set; }
    public int UtSubDocType { get; set; }
    public bool UtSysGen { get; set; }
    public bool UtBlock { get; set; }
    public bool UtIsPartial { get; set; }
    public DateTime UtAddTime { get; set; }
    [MaxLength(100)] public string UtAddUser { get; set; } = string.Empty;
    [MaxLength(100)] public string UtAddSystem { get; set; } = string.Empty;
    [MaxLength(100)] public string UtAddIp { get; set; } = string.Empty;
    [MaxLength(100)] public string UtAddAppVersion { get; set; } = string.Empty;
    [MaxLength(100)] public string UtAddDbVersion { get; set; } = string.Empty;
    public long UtAddId { get; set; }
    public int? UtAddSessionId { get; set; }
    public DateTime UtEditTime { get; set; }
    [MaxLength(50)] public string UtEditUser { get; set; } = string.Empty;
    [MaxLength(50)] public string UtEditSystem { get; set; } = string.Empty;
    [MaxLength(50)] public string UtEditIp { get; set; } = string.Empty;
    [MaxLength(50)] public string UtEditAppVersion { get; set; } = string.Empty;
    [MaxLength(50)] public string UtEditDbVersion { get; set; } = string.Empty;
    public long UtEditId { get; set; }
    public int? UtEditSessionId { get; set; }
    public int UtRecordVersion { get; set; }

    public OrgLegalentityLegacy? Entity { get; set; }
    public EmployeeProfileLegacy? HiringManager { get; set; }
    public EmployeeProfileLegacy? Approver { get; set; }
}

[Table("HRVacancyRecruiter")]
public sealed class HRVacancyRecruiter
{
    [Key] public int Id { get; set; }
    public int VacancyId { get; set; }
    public int EmployeeId { get; set; }
    [MaxLength(100)] public string AssignedBy { get; set; } = string.Empty;
    public DateTime AssignedAtUtc { get; set; }
    public bool IsActive { get; set; } = true;

    public EmployeeProfileLegacy? Employee { get; set; }
}

[Table("HRVacancyQualification")]
public sealed class HRVacancyQualification
{
    [Key] public int Id { get; set; }
    public int VacancyId { get; set; }
    public int QualificationId { get; set; }
    public bool IsMandatory { get; set; }
}

[Table("HRVacancySkill")]
public sealed class HRVacancySkill
{
    [Key] public int Id { get; set; }
    public int VacancyId { get; set; }
    public int SkillId { get; set; }
    [MaxLength(50)] public string? Proficiency { get; set; }
    public bool IsMandatory { get; set; }
}

[Table("HRVacancyRevision")]
public sealed class HRVacancyRevision
{
    [Key] public int Id { get; set; }
    public int VacancyId { get; set; }
    public int RevisionNumber { get; set; }
    [MaxLength(30)] public string EventType { get; set; } = string.Empty;
    public string SnapshotJson { get; set; } = string.Empty;
    [MaxLength(100)] public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}

[Table("HRVacancyPosition")]
public sealed class HRVacancyPosition : IVacancyUtTracked
{
    [Key] public int Id { get; set; }
    public int VacancyId { get; set; }
    public int RevisionId { get; set; }
    public int PositionId { get; set; }
    public int LineNumber { get; set; }
    [MaxLength(30)] public string PositionNumber { get; set; } = string.Empty;
    [MaxLength(30)] public string Status { get; set; } = "Vacant";
    public int? CurrentEmployeeId { get; set; }
    public bool NeedsReview { get; set; }
    [MaxLength(500)] public string? ReviewReason { get; set; }
    [MaxLength(100)] public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    [MaxLength(100)] public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    [Timestamp] public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public int UtDocType { get; set; }
    public int UtSubDocType { get; set; }
    public bool UtSysGen { get; set; }
    public bool UtBlock { get; set; }
    public bool UtIsPartial { get; set; }
    public DateTime UtAddTime { get; set; }
    [MaxLength(25)] public string UtAddUser { get; set; } = string.Empty;
    [MaxLength(25)] public string UtAddSystem { get; set; } = string.Empty;
    [MaxLength(25)] public string UtAddIp { get; set; } = string.Empty;
    [MaxLength(10)] public string UtAddAppVersion { get; set; } = string.Empty;
    [MaxLength(10)] public string UtAddDbVersion { get; set; } = string.Empty;
    public long UtAddId { get; set; }
    public int? UtAddSessionId { get; set; }
    public DateTime UtEditTime { get; set; }
    [MaxLength(25)] public string UtEditUser { get; set; } = string.Empty;
    [MaxLength(25)] public string UtEditSystem { get; set; } = string.Empty;
    [MaxLength(25)] public string UtEditIp { get; set; } = string.Empty;
    [MaxLength(10)] public string UtEditAppVersion { get; set; } = string.Empty;
    [MaxLength(10)] public string UtEditDbVersion { get; set; } = string.Empty;
    public long UtEditId { get; set; }
    public int? UtEditSessionId { get; set; }
    public int UtRecordVersion { get; set; }

    public EmployeeProfileLegacy? CurrentEmployee { get; set; }
}

[Table("HRVacancyPositionAssignment")]
public sealed class HRVacancyPositionAssignment : IVacancyUtTracked
{
    [Key] public int Id { get; set; }
    public int PositionId { get; set; }
    public int EmployeeId { get; set; }
    [MaxLength(30)] public string AssignmentType { get; set; } = string.Empty;
    [MaxLength(20)] public string Status { get; set; } = "Scheduled";
    public DateTime EffectiveDate { get; set; }
    public int? SourcePositionId { get; set; }
    [MaxLength(1500)] public string? Remarks { get; set; }
    [MaxLength(1500)] public string? FailureReason { get; set; }
    [MaxLength(100)] public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ActivatedAtUtc { get; set; }

    public int UtDocType { get; set; }
    public int UtSubDocType { get; set; }
    public bool UtSysGen { get; set; }
    public bool UtBlock { get; set; }
    public bool UtIsPartial { get; set; }
    public DateTime UtAddTime { get; set; }
    [MaxLength(25)] public string UtAddUser { get; set; } = string.Empty;
    [MaxLength(25)] public string UtAddSystem { get; set; } = string.Empty;
    [MaxLength(25)] public string UtAddIp { get; set; } = string.Empty;
    [MaxLength(10)] public string UtAddAppVersion { get; set; } = string.Empty;
    [MaxLength(10)] public string UtAddDbVersion { get; set; } = string.Empty;
    public long UtAddId { get; set; }
    public int? UtAddSessionId { get; set; }
    public DateTime UtEditTime { get; set; }
    [MaxLength(25)] public string UtEditUser { get; set; } = string.Empty;
    [MaxLength(25)] public string UtEditSystem { get; set; } = string.Empty;
    [MaxLength(25)] public string UtEditIp { get; set; } = string.Empty;
    [MaxLength(10)] public string UtEditAppVersion { get; set; } = string.Empty;
    [MaxLength(10)] public string UtEditDbVersion { get; set; } = string.Empty;
    public long UtEditId { get; set; }
    public int? UtEditSessionId { get; set; }
    public int UtRecordVersion { get; set; }

    public EmployeeProfileLegacy? Employee { get; set; }
}

[Table("HRVacancyNote")]
public sealed class HRVacancyNote
{
    [Key] public int Id { get; set; }
    public int VacancyId { get; set; }
    [MaxLength(2000)] public string Note { get; set; } = string.Empty;
    [MaxLength(100)] public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
}

[Table("HRVacancyAudit")]
public sealed class HRVacancyAudit : IVacancyUtTracked
{
    [Key] public long Id { get; set; }
    public int ClientId { get; set; }
    public int EntityId { get; set; }
    public int VacancyId { get; set; }
    public int? PositionId { get; set; }
    [MaxLength(50)] public string Action { get; set; } = string.Empty;
    [MaxLength(100)] public string Actor { get; set; } = string.Empty;
    public int? ActorEmployeeId { get; set; }
    [MaxLength(1500)] public string? Remarks { get; set; }
    public string? BeforeJson { get; set; }
    public string? AfterJson { get; set; }
    public DateTime OccurredAtUtc { get; set; }

    public int UtDocType { get; set; }
    public int UtSubDocType { get; set; }
    public bool UtSysGen { get; set; }
    public bool UtBlock { get; set; }
    public bool UtIsPartial { get; set; }
    public DateTime UtAddTime { get; set; }
    [MaxLength(100)] public string UtAddUser { get; set; } = string.Empty;
    [MaxLength(100)] public string UtAddSystem { get; set; } = string.Empty;
    [MaxLength(100)] public string UtAddIp { get; set; } = string.Empty;
    [MaxLength(100)] public string UtAddAppVersion { get; set; } = string.Empty;
    [MaxLength(100)] public string UtAddDbVersion { get; set; } = string.Empty;
    public long UtAddId { get; set; }
    public int? UtAddSessionId { get; set; }
    public DateTime UtEditTime { get; set; }
    [MaxLength(50)] public string UtEditUser { get; set; } = string.Empty;
    [MaxLength(50)] public string UtEditSystem { get; set; } = string.Empty;
    [MaxLength(50)] public string UtEditIp { get; set; } = string.Empty;
    [MaxLength(50)] public string UtEditAppVersion { get; set; } = string.Empty;
    [MaxLength(50)] public string UtEditDbVersion { get; set; } = string.Empty;
    public long UtEditId { get; set; }
    public int? UtEditSessionId { get; set; }
    public int UtRecordVersion { get; set; }

    public EmployeeProfileLegacy? ActorEmployee { get; set; }
}
