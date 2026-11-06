namespace HumanResource.Service.DTOs;

public sealed record VacancyRequirementItem(int Id, bool IsMandatory, string? Proficiency = null);

public sealed record VacancyUpsertRequest(
    string VacancyType,
    string? ReportsToType,
    int? ReportsToPositionId,
    int? ReplacementPositionId,
    string JobTitle,
    int DepartmentId,
    int DesignationId,
    int? GradeId,
    int? SiteId,
    int? EmploymentTypeId,
    int? HiringManagerEmployeeId,
    int? ApproverEmployeeId,
    string? Priority,
    int RequestedOpenings,
    DateTime? TargetFillDate,
    decimal? MinimumExperienceYears,
    decimal? MaximumExperienceYears,
    string? JobDescription,
    string? Responsibilities,
    string? Requirements,
    string? RequiredQualifications,
    decimal? SalaryMinimum,
    decimal? SalaryMaximum,
    string? CurrencyCode,
    decimal? HiringBudget,
    IReadOnlyCollection<int>? RecruiterEmployeeIds,
    IReadOnlyCollection<VacancyRequirementItem>? Qualifications,
    IReadOnlyCollection<VacancyRequirementItem>? Skills);

public sealed record VacancyListItemDto(
    int Id,
    string VacancyNumber,
    string VacancyType,
    string JobTitle,
    int DepartmentId,
    string? Department,
    int DesignationId,
    string? Designation,
    int RequestedOpenings,
    int AvailablePositions,
    int FilledPositions,
    int CancelledPositions,
    string ApprovalStatus,
    string? LifecycleStatus,
    string Priority,
    DateTime? TargetFillDate,
    DateTime CreatedAtUtc,
    string? JobPortalJobId,
    DateTime? JobPortalPostedAtUtc);

/// <summary>Records the Job Portal posting raised for a requisition.</summary>
public sealed record VacancyJobPostingRequest(string JobId, string? JobTitle);

/// <summary>
/// Outcome of linking a job posting: whether the requisition was approved on
/// the spot (no separate approval needed) and, when it was not, why.
/// </summary>
public sealed record VacancyJobPostingResultDto(bool Approved, string? Message);

public sealed record VacancyDetailDto(
    int Id,
    string VacancyNumber,
    string VacancyType,
    string? ReportsToType,
    int? ReportsToPositionId,
    int? ReplacementPositionId,
    string JobTitle,
    int DepartmentId,
    int DesignationId,
    int? GradeId,
    int? SiteId,
    int? EmploymentTypeId,
    int? HiringManagerEmployeeId,
    int? ApproverEmployeeId,
    string Priority,
    int RequestedOpenings,
    DateTime? TargetFillDate,
    decimal? MinimumExperienceYears,
    decimal? MaximumExperienceYears,
    string? JobDescription,
    string? Responsibilities,
    string? Requirements,
    string? RequiredQualifications,
    decimal? SalaryMinimum,
    decimal? SalaryMaximum,
    string? CurrencyCode,
    decimal? HiringBudget,
    string ApprovalStatus,
    string? LifecycleStatus,
    int RevisionNumber,
    IReadOnlyCollection<int> RecruiterEmployeeIds,
    IReadOnlyCollection<VacancyRequirementItem> Qualifications,
    IReadOnlyCollection<VacancyRequirementItem> Skills);

public sealed record VacancyWorkflowRequest(string? Remarks);
public sealed record VacancyStatusRequest(string Status, string? Remarks);
public sealed record VacancyRecruitersRequest(IReadOnlyCollection<int> EmployeeIds);
public sealed record VacancyNoteRequest(string Note);

public sealed record VacancyPositionDto(
    int Id,
    int PositionId,
    int VacancyId,
    int EntityId,
    string VacancyNumber,
    string JobTitle,
    string PositionNumber,
    int LineNumber,
    string Status,
    int? CurrentEmployeeId,
    string? CurrentEmployee,
    bool NeedsReview,
    string? ReviewReason,
    int DepartmentId,
    string? Department,
    int DesignationId,
    string? Designation,
    int? GradeId,
    int? EmploymentTypeId,
    string? Responsibilities);

public sealed record InternalPromotionRequest(
    int PositionId,
    int EmployeeId,
    DateTime EffectiveDate,
    string? Remarks);

public sealed record VacancyDashboardDto(
    int TotalVacancies,
    int OpenVacancies,
    int FilledVacancies,
    int ClosedVacancies,
    int PendingApprovals,
    int VacantPositions,
    int ReservedPositions,
    int FilledPositions,
    int CancelledPositions,
    IReadOnlyCollection<GenderDistributionItemDto> GenderDistribution,
    IReadOnlyCollection<VacancyListItemDto> RecentVacancies);

public sealed record GenderDistributionItemDto(string Gender, int Count);

public sealed record DepartmentPositionSummaryDto(
    int DepartmentId,
    string Department,
    int VacantPositions,
    int ReservedPositions,
    int FilledPositions,
    int TotalActivePositions,
    decimal FillRate);

public sealed record DepartmentPositionDetailDto(
    int PositionId,
    int DepartmentId,
    string Department,
    string VacancyNumber,
    string PositionNumber,
    string JobTitle,
    string? Designation,
    string? Grade,
    string? Site,
    string? EmploymentType,
    string Priority,
    string ApprovalStatus,
    string? LifecycleStatus,
    string PositionStatus,
    int? EmployeeId,
    string? EmployeeCode,
    string? Employee,
    string? AssignmentType,
    DateTime? AssignmentEffectiveDate,
    DateTime? TargetFillDate,
    DateTime PositionCreatedAtUtc,
    DateTime? PositionUpdatedAtUtc);

public sealed record VacancyLookupItemDto(int Id, string Name, string? Code = null);

public sealed record VacancyLookupsDto(
    IReadOnlyCollection<VacancyLookupItemDto> Departments,
    IReadOnlyCollection<VacancyLookupItemDto> Designations,
    IReadOnlyCollection<VacancyLookupItemDto> Grades,
    IReadOnlyCollection<VacancyLookupItemDto> Sites,
    IReadOnlyCollection<VacancyLookupItemDto> EmploymentTypes,
    IReadOnlyCollection<VacancyLookupItemDto> Employees,
    IReadOnlyCollection<VacancyLookupItemDto> Qualifications,
    IReadOnlyCollection<VacancyLookupItemDto> Skills,
    IReadOnlyCollection<PositionLookupDto> ReportsToPositions,
    IReadOnlyCollection<PositionLookupDto> ReplacementPositions);

public sealed record PositionLookupDto(
    int Id,
    string PositionNumber,
    string Title,
    int? DepartmentId,
    int? DesignationId,
    string OccupancyStatus);

public sealed record OrganogramEmployeeDto(int Id, int? EntityId, string Code, string Name, string? ReportingToName = null);
public sealed record OrganogramRootDto(int EmployeeId, string EmployeeCode, string Name, string? Designation);
public sealed record OrganogramEmployeeNodeDto(
    int Id, string Code, string Name, int? ReportingToEmployeeId, string? Designation, string? Department,
    string? PositionNumber, bool HasActivePosition);
public sealed record OrganogramPositionDto(
    int Id,
    int? EntityId,
    string ScopeType,
    string PositionKind,
    int? DepartmentId,
    string? Department,
    int? DesignationId,
    string? Designation,
    string PositionNumber,
    string Title,
    string ReportsToType,
    int? ReportsToPositionId,
    string Status,
    int? EmployeeId,
    string? EmployeeCode,
    string? Employee,
    bool IsVacant);
public sealed record OrganogramWarningDto(string Code, string Message, int? PositionId = null, int? EmployeeId = null);
public sealed record OrganogramPermissionsDto(bool CanView, bool CanEdit, bool CanReconcileReporting, bool CanManageLeadership, bool IsSuperAdmin);
public sealed record OrganogramDto(
    OrganogramRootDto? SuperAdmin,
    IReadOnlyCollection<OrganogramEmployeeNodeDto> EmployeeHierarchy,
    IReadOnlyCollection<OrganogramPositionDto> Positions,
    IReadOnlyCollection<OrganogramEmployeeDto> UnassignedEmployees,
    IReadOnlyCollection<OrganogramEmployeeDto> Employees,
    IReadOnlyCollection<OrganogramWarningDto> Warnings,
    OrganogramPermissionsDto Permissions);
public sealed record PositionParentRequest(string ReportsToType, int? ReportsToPositionId, string? Remarks);
public sealed record LeadershipRequest(int CeoEmployeeId, string? Remarks);
