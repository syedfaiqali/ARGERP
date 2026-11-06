using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HumanResource.Service.Entities;

[Table("HRPosition")]
public sealed class HRPosition
{
    [Key] public int Id { get; set; }
    public int ClientId { get; set; }
    public int? EntityId { get; set; }
    [MaxLength(10)] public string ScopeType { get; set; } = "Entity";
    [MaxLength(20)] public string PositionKind { get; set; } = "Standard";
    public int? DepartmentId { get; set; }
    public int? DesignationId { get; set; }
    [MaxLength(30)] public string PositionNumber { get; set; } = string.Empty;
    [MaxLength(200)] public string Title { get; set; } = string.Empty;
    [MaxLength(20)] public string ReportsToType { get; set; } = "Unmapped";
    public int? ReportsToPositionId { get; set; }
    public int LovStatusId { get; set; }

    public int UtDocType { get; set; }
    public int UtSubDocType { get; set; }
    public bool? UtSysGen { get; set; }
    public bool? UtBlock { get; set; }
    public bool? UtIsPartial { get; set; }
    public DateTime UtAddTime { get; set; }
    [MaxLength(25)] public string UtAddUser { get; set; } = string.Empty;
    [MaxLength(25)] public string UtAddSystem { get; set; } = string.Empty;
    [MaxLength(25)] public string UtAddIp { get; set; } = string.Empty;
    [MaxLength(10)] public string UtAddAppVersion { get; set; } = string.Empty;
    [MaxLength(10)] public string UtAddDbVersion { get; set; } = string.Empty;
    public long UtAddId { get; set; }
    public int UtAddSessionId { get; set; }
    public DateTime UtEditTime { get; set; }
    [MaxLength(25)] public string UtEditUser { get; set; } = string.Empty;
    [MaxLength(25)] public string UtEditSystem { get; set; } = string.Empty;
    [MaxLength(25)] public string UtEditIp { get; set; } = string.Empty;
    [MaxLength(10)] public string UtEditAppVersion { get; set; } = string.Empty;
    [MaxLength(10)] public string UtEditDbVersion { get; set; } = string.Empty;
    public long UtEditId { get; set; }
    public int UtEditSessionId { get; set; }
    public int? UtRecordVersion { get; set; }
    public HRPosition? ReportsToPosition { get; set; }
    public OrgLegalentityLegacy? Entity { get; set; }
    public OrgDepartmentLegacy? Department { get; set; }
    public OrgDesignationLegacy? Designation { get; set; }
    public UtLovLegacy? StatusLov { get; set; }
}

[Table("HRPositionAssignment")]
public sealed class HRPositionAssignment
{
    [Key] public int Id { get; set; }
    public int PositionId { get; set; }
    public int EmployeeId { get; set; }
    [MaxLength(30)] public string AssignmentType { get; set; } = "Occupant";
    public int LovStatusId { get; set; }
    public DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    [MaxLength(1500)] public string? Remarks { get; set; }

    public int UtDocType { get; set; }
    public int UtSubDocType { get; set; }
    public bool? UtSysGen { get; set; }
    public bool? UtBlock { get; set; }
    public bool? UtIsPartial { get; set; }
    public DateTime UtAddTime { get; set; }
    [MaxLength(25)] public string UtAddUser { get; set; } = string.Empty;
    [MaxLength(25)] public string UtAddSystem { get; set; } = string.Empty;
    [MaxLength(25)] public string UtAddIp { get; set; } = string.Empty;
    [MaxLength(10)] public string UtAddAppVersion { get; set; } = string.Empty;
    [MaxLength(10)] public string UtAddDbVersion { get; set; } = string.Empty;
    public long UtAddId { get; set; }
    public int UtAddSessionId { get; set; }
    public DateTime UtEditTime { get; set; }
    [MaxLength(25)] public string UtEditUser { get; set; } = string.Empty;
    [MaxLength(25)] public string UtEditSystem { get; set; } = string.Empty;
    [MaxLength(25)] public string UtEditIp { get; set; } = string.Empty;
    [MaxLength(10)] public string UtEditAppVersion { get; set; } = string.Empty;
    [MaxLength(10)] public string UtEditDbVersion { get; set; } = string.Empty;
    public long UtEditId { get; set; }
    public int UtEditSessionId { get; set; }
    public int? UtRecordVersion { get; set; }
    public HRPosition? Position { get; set; }
    public EmployeeProfileLegacy? Employee { get; set; }
    public UtLovLegacy? StatusLov { get; set; }
}

[Table("HRPositionAudit")]
public sealed class HRPositionAudit
{
    [Key] public long Id { get; set; }
    public int ClientId { get; set; }
    public int? EntityId { get; set; }
    public int PositionId { get; set; }
    [MaxLength(50)] public string Action { get; set; } = string.Empty;
    [MaxLength(100)] public string Actor { get; set; } = string.Empty;
    public int? ActorEmployeeId { get; set; }
    [MaxLength(1500)] public string? Remarks { get; set; }
    public string? BeforeJson { get; set; }
    public string? AfterJson { get; set; }
    public DateTime OccurredAtUtc { get; set; }

    public int UtDocType { get; set; }
    public int UtSubDocType { get; set; }
    public bool? UtSysGen { get; set; }
    public bool? UtBlock { get; set; }
    public bool? UtIsPartial { get; set; }
    public DateTime UtAddTime { get; set; }
    [MaxLength(25)] public string UtAddUser { get; set; } = string.Empty;
    [MaxLength(25)] public string UtAddSystem { get; set; } = string.Empty;
    [MaxLength(25)] public string UtAddIp { get; set; } = string.Empty;
    [MaxLength(10)] public string UtAddAppVersion { get; set; } = string.Empty;
    [MaxLength(10)] public string UtAddDbVersion { get; set; } = string.Empty;
    public long UtAddId { get; set; }
    public int UtAddSessionId { get; set; }
    public DateTime UtEditTime { get; set; }
    [MaxLength(25)] public string UtEditUser { get; set; } = string.Empty;
    [MaxLength(25)] public string UtEditSystem { get; set; } = string.Empty;
    [MaxLength(25)] public string UtEditIp { get; set; } = string.Empty;
    [MaxLength(10)] public string UtEditAppVersion { get; set; } = string.Empty;
    [MaxLength(10)] public string UtEditDbVersion { get; set; } = string.Empty;
    public long UtEditId { get; set; }
    public int UtEditSessionId { get; set; }
    public int? UtRecordVersion { get; set; }

    public HRPosition? Position { get; set; }
    public EmployeeProfileLegacy? ActorEmployee { get; set; }
}
