using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace EDXS.Shared.Workflow;

public static class WorkflowJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);
    public static JsonElement Element(object? value) => JsonSerializer.SerializeToElement(value, Options);
    public static string Hash(object value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(value, Options))));
}

public sealed record WorkflowActor(int UserId, int ClientId, int? EntityId)
{
    public static WorkflowActor From(HttpContext context)
    {
        string? Claim(params string[] names) => context.User.Claims.FirstOrDefault(c => names.Contains(c.Type, StringComparer.OrdinalIgnoreCase))?.Value;
        if (context.User.Identity?.IsAuthenticated != true ||
            !int.TryParse(Claim("userId", "sub", ClaimTypes.NameIdentifier), out var userId) || userId <= 0 ||
            !int.TryParse(Claim("clientId", "client_id"), out var clientId) || clientId <= 0)
            throw new WorkflowException(401, "Unauthenticated", "A valid user and tenant identity are required.");
        var headerClient = context.Request.Headers["X-Client-Id"].FirstOrDefault();
        if (!string.IsNullOrEmpty(headerClient) && headerClient != clientId.ToString())
            throw new WorkflowException(403, "TenantMismatch", "The selected tenant does not match your identity.");
        var entityValue = context.Request.Headers["X-Entity-Id"].FirstOrDefault() ?? Claim("entityId", "entity_id");
        int? entityId = int.TryParse(entityValue, out var entity) && entity > 0 ? entity : null;
        return new(userId, clientId, entityId);
    }
}

public sealed class WorkflowException(int status, string code, string message, Guid? instanceId = null) : Exception(message)
{
    public int Status { get; } = status;
    public string Code { get; } = code;
    public Guid? InstanceId { get; } = instanceId;
}

public sealed record ActionRegistration(string ScreenKey, string Name, string Module, string Service,
    string ResourceType, bool EntityScoped, string Route, string[] PermissionControllers, int SchemaVersion = 1);

public static class PilotActions
{
    public static readonly ActionRegistration Author = new("SMAuthor", "Author", "Academics", "academics", "AcademicAuthor", true,
        "/academics/Author", ["SMAuthor"]);
    public static readonly ActionRegistration Department = new("Department", "Department", "Organization Setup", "organization", "Department", false,
        "/org/Department", ["OrgDepartment"]);
    public static readonly ActionRegistration VehicleType = new("Vehicletype", "Vehicle Type", "Transport", "transport", "VehicleType", false,
        "/transport/Vehicle Type", ["Vehicletype"]);
    // The legacy Line Manager/HR/COO loan stages are skipped for requests this workflow governs:
    // one screen carries one approval chain, the one configured in Workflow Management. Once it
    // approves, the request goes straight to the Finance disbursement queue.
    public static readonly ActionRegistration LoanRequest = new("EPLoanrequest", "Loan Request", "Human Resource", "humanresource", "LoanRequest", false,
        "/hr/LoanRequest", ["EPLoanrequest"]);
    // Human Resource setup screens. Their records belong to a campus, so the requester's campus
    // travels with the request and the approved record lands in that campus's list.
    public static readonly ActionRegistration Qualification = new("ERQualification", "Qualification", "Human Resource", "humanresource", "Qualification", true,
        "/hr/ERQualification", ["ERQualification"]);
    public static readonly ActionRegistration QualificationType = new("ERQualificationtype", "Qualification Type", "Human Resource", "humanresource", "QualificationType", true,
        "/hr/ERQualificationtype", ["ERQualificationtype"]);
    public static readonly ActionRegistration Skill = new("ERSkill", "Skill", "Human Resource", "humanresource", "Skill", true,
        "/hr/ERSkill", ["ERSkill"]);
    public static readonly ActionRegistration SkillType = new("ERSkilltype", "Skill Type", "Human Resource", "humanresource", "SkillType", true,
        "/hr/ERSkilltype", ["ERSkilltype"]);
    public static readonly ActionRegistration PositionType = new("ERPositiontype", "Position Type", "Human Resource", "humanresource", "PositionType", true,
        "/hr/ERPositiontype", ["ERPositiontype"]);
    // Document types are tenant-wide; their handlers ignore the campus.
    public static readonly ActionRegistration DocumentType = new("HRDocType", "Document Type", "Human Resource", "humanresource", "DocumentType", false,
        "/hr/HRDocType", ["HRDocType"]);
    // Payroll setup. Earning handlers validate within the requester's campus, so it travels with the
    // request; deduction and payroll period handlers do not use a campus and stay tenant-wide.
    public static readonly ActionRegistration EarningType = new("EPEarningtype", "Earning Type", "Human Resource", "humanresource", "EarningType", true,
        "/hr/EPEarningtype", ["EPEarningtype"]);
    public static readonly ActionRegistration EarningGroup = new("EPEarninggrphdr", "Earning Group", "Human Resource", "humanresource", "EarningGroup", true,
        "/hr/EPEarninggrphdr", ["EPEarninggrphdr"]);
    public static readonly ActionRegistration DeductionType = new("EPDeductiontype", "Deduction Type", "Human Resource", "humanresource", "DeductionType", false,
        "/hr/EPDeductiontype", ["EPDeductiontype"]);
    public static readonly ActionRegistration DeductionGroup = new("EPDeductiongrphdr", "Deduction Group", "Human Resource", "humanresource", "DeductionGroup", false,
        "/hr/EPDeductiongrphdr", ["EPDeductiongrphdr"]);
    public static readonly ActionRegistration PayrollPeriod = new("ETPayperioddtl", "Payroll Period", "Human Resource", "humanresource", "PayrollPeriod", false,
        "/hr/ETPayperioddtl", ["ETPayperioddtl"]);
    // Time management setup. Holiday types, device registrations and attendance policies are saved
    // against the requester's campus; attendance activities and device types have no campus at all.
    public static readonly ActionRegistration HolidayType = new("ETHolidaytypeList", "Holiday Type", "Human Resource", "humanresource", "HolidayType", true,
        "/hr/ETHolidaytypeList", ["ETHolidaytypeList"]);
    public static readonly ActionRegistration AttendanceActivity = new("HRAtnactivity", "Attendance Activity", "Human Resource", "humanresource", "AttendanceActivity", false,
        "/hr/HRAtnactivity", ["HRAtnactivity"]);
    public static readonly ActionRegistration DeviceType = new("DeviceType", "Device Type", "Human Resource", "humanresource", "DeviceType", false,
        "/hr/DeviceType", ["DeviceType"]);
    public static readonly ActionRegistration DeviceRegistration = new("SMMachineRegistration", "Device Registration", "Human Resource", "humanresource", "DeviceRegistration", true,
        "/hr/SMMachineRegistration", ["SMMachineRegistration"]);
    public static readonly ActionRegistration AttendancePolicy = new("ETPolicyhdr", "Attendance Policy", "Human Resource", "humanresource", "AttendancePolicy", true,
        "/hr/ETPolicyhdr", ["ETPolicyhdr"]);
    public static readonly ActionRegistration[] All = [Department,
        Qualification, QualificationType, Skill, SkillType, PositionType, DocumentType,
        EarningType, EarningGroup, DeductionType, DeductionGroup, PayrollPeriod,
        HolidayType, AttendanceActivity, DeviceType, DeviceRegistration, AttendancePolicy];
}

public sealed record AssigneeSelector(string Kind, string Id); // Group, Role, User
// RelatedUser names a relationship of the requester (see RelatedUsers), resolved when the step starts.
// When it resolves, those users are the step's assignees and Assignees is only the fallback.
// EndOnPositiveDecision: on an Approval step, approval completes the workflow and skips the later steps;
// a rejection still follows NegativeBehavior (hard rejects, soft passes the request to the next step).
// Both are optional so step snapshots stored before they existed still deserialize unchanged.
public sealed record WorkflowStep(string Name, string DecisionKind, string Quorum, string NegativeBehavior,
    string AssigneeScope, bool AllowSelfAction, bool RequirePositiveComment, bool RequireNegativeComment,
    List<AssigneeSelector> Assignees, string? RelatedUser = null, bool EndOnPositiveDecision = false);

/// <summary>Relationships a step can route to, read from the requester's own records in the directory.</summary>
public static class RelatedUsers
{
    public const string Parent = "Parent", ReportTo = "ReportTo", Teacher = "Teacher";
    public static readonly string[] All = [Parent, ReportTo, Teacher];
    public static string Label(string kind) => kind == ReportTo ? "Report To" : kind;
}
public sealed record DefinitionDraft(string ScreenKey, string ActionKey, string Name, List<WorkflowStep> Steps);
// With a screen and operation the user's role must grant that right. With RequireRight false a missing right
// is reported as CanSubmit = false instead, for a caller that only needs to know whether the user holds it.
public sealed record IdentityRequest(WorkflowActor Actor, string? ScreenKey = null, string? Operation = null, bool RequireRight = true);
public sealed record WorkflowIdentity(int UserId, int ClientId, string Name, bool IsPlatformAdmin,
    bool IsTenantAdmin, int[] EntityIds, bool CanSubmit = false);
// With RelatedUser set, the directory resolves that relationship of the requester and ignores Selectors.
public sealed record ResolveUsersRequest(int ClientId, int? EntityId, string Scope, int RequesterId,
    bool AllowSelfAction, List<AssigneeSelector> Selectors, string? RelatedUser = null);
public sealed record ApproverUser(int UserId, string Name, int? RoleId, string? RoleName, bool IsTenantAdmin, int[] EntityIds);
public sealed record WorkflowLookup(int Id, string Name);
// One configurable screen of the installation, named by its legacy menu controller.
public sealed record WorkflowScreen(string ControllerId, string Name, string Module);
public sealed record WorkflowDirectory(List<ApproverUser> Users, List<WorkflowLookup> Roles, List<WorkflowLookup> Entities);
public sealed record UserNamesRequest(int ClientId, int[] UserIds);
public sealed record WorkflowUser(int UserId, string Name, string? RoleName);

// Domain adapters construct these values. The browser cannot choose a service, scope,
// executable payload, fingerprint or preview URL through a generic submission endpoint.
public sealed record PreparedAction(string ScreenKey, string Operation, int ClientId, int? EntityId,
    int UserId, string? RecordKey, string Subject, string? ExpectedHash, JsonElement Payload,
    JsonElement Before, JsonElement After, int SchemaVersion = 1);
public sealed record SubmitAction(PreparedAction Action, Guid IdempotencyKey, string RequestHash, Guid? PreviousInstanceId = null);
public sealed record SubmissionResult(bool Pending, Guid? InstanceId = null, string? Status = null, int Version = 0)
{
    public string Disposition => Pending ? "PendingApproval" : "ExecuteImmediately";
}
public sealed record ExecuteAction(Guid ExecutionId, PreparedAction Action, string RequestHash);
public sealed record ExecutionResult(int StatusCode, JsonElement? Body, string? RecordKey);
public sealed record DecisionRequest(string Decision, string? Comment, int ExpectedVersion, Guid IdempotencyKey);
// New values for a request still awaiting approval, from a user whose role may edit its screen. The owning
// service re-validates them against the original submission, which keeps its requester, campus and record
// (see IWorkflowActionAdapter.AmendAsync).
public sealed record AmendRequest(JsonElement Payload, int ExpectedVersion, Guid IdempotencyKey);
public sealed record AmendAction(PreparedAction Original, JsonElement Payload);
public sealed record InstanceCommand(int ExpectedVersion, string? Comment);

public static class DefinitionRules
{
    public static string[] Validate(DefinitionDraft draft, bool global)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(draft.Name) || draft.Name.Length > 150) errors.Add("A workflow name of at most 150 characters is required.");
        if (draft.ActionKey is not ("Save" or "Delete")) errors.Add("Choose Save or Delete.");
        if (draft.Steps.Count is < 1 or > 30) errors.Add("A workflow needs between 1 and 30 steps.");
        foreach (var step in draft.Steps)
        {
            if (string.IsNullOrWhiteSpace(step.Name) || step.Name.Length > 150) errors.Add("Every step needs a name of at most 150 characters.");
            if (step.DecisionKind is not ("Approval" or "Recommendation")) errors.Add("Invalid decision kind.");
            if (step.Quorum is not ("Any" or "All")) errors.Add("Choose Any or All quorum.");
            if (step.NegativeBehavior is not ("Continue" or "Terminate")) errors.Add("Choose soft or hard rejection.");
            if (step.AssigneeScope is not ("Tenant" or "Campus")) errors.Add("Choose tenant or campus assignees.");
            if (step.RelatedUser != null && !RelatedUsers.All.Contains(step.RelatedUser)) errors.Add("Choose Parent, Report To or Teacher as the related user.");
            // A related user may stand alone; its group/role/user selectors are then an optional fallback.
            if (step.Assignees.Count > 100 || step.Assignees.Count == 0 && step.RelatedUser == null)
                errors.Add("Every step needs a related user or between 1 and 100 assignee selectors.");
            // Ending early must still mean the request was approved, never merely recommended.
            if (step.EndOnPositiveDecision && step.DecisionKind != "Approval") errors.Add("Only an Approval step can end the workflow on a positive decision.");
            foreach (var selector in step.Assignees)
            {
                if (selector.Kind is not ("Group" or "Role" or "User") || string.IsNullOrWhiteSpace(selector.Id)) errors.Add("Invalid assignee selector.");
                if (global && selector.Kind != "Group") errors.Add("Global workflows must use logical approver groups.");
                if (selector.Kind is "Role" or "User" && (!int.TryParse(selector.Id, out var id) || id <= 0)) errors.Add("Invalid user or role identifier.");
            }
        }
        if (draft.Steps.LastOrDefault() is not { DecisionKind: "Approval", NegativeBehavior: "Terminate" })
            errors.Add("The final step must be an Approval step with hard rejection.");
        return errors.Distinct().ToArray();
    }
}
