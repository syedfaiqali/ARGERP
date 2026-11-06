using System;

namespace HumanResource.Service.DTOs
{
    public record LeaveTransactionV3Dto(
        int Id,
        string? TransactionNo,
        DateTime? TransactionDate,
        int? RequestId,
        string? RequestNo,
        int? EmployeeId,
        string? EmployeeCode,
        string? EmployeeName,
        string? Department,
        string? Designation,
        string? Site,
        int? LeaveUnitId,
        string? LeaveUnit,
        int? LeaveTypeId,
        string? LeaveType,
        double? Allocated,
        double? Availed,
        double? Balance,
        DateTime? FromDate,
        DateTime? TillDate,
        double? LeaveDays,
        bool? PaidLeave,
        int? StatusId,
        string? Status,
        int? AdmApproval,
        bool CeoApprovalRequired,
        bool LineManagerApprovalCompleted,
        bool CeoApprovalCompleted,
        bool IsFullyApproved,
        string? AttachmentDocumentName = null,
        string? AttachmentUploadPath = null,
        string? AttachmentExternalLink = null,
        List<LeaveRequestAttachmentV3Dto>? Attachments = null
    );

    public record LeaveTransactionScopeV3Dto(
        bool ShowRequestNo,
        string? Scope,
        int? ReferenceEmployeeId,
        int? EmployeeCategoryId,
        string? EmployeeCategory,
        int? DepartmentId,
        string? Department,
        int? DesignationId,
        string? Designation,
        bool IsAdmin,
        bool IsManagement,
        List<LeaveTransactionV3Dto> Items
    );

    public record CreateLeaveTransactionV3Dto(
        string? TransactionNo,
        DateTime? TransactionDate,
        int? RequestId,
        int? EmployeeId,
        int? LeaveUnitId,
        int? LeaveTypeId,
        DateTime? FromDate,
        DateTime? TillDate,
        double? LeaveDays,
        bool PaidLeave,
        int? StatusId,
        int? AdmApproval
    );
}
