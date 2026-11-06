using System;

namespace HumanResource.Service.DTOs
{
    public sealed record LeaveRequestV3Dto(
        int Id,
        string? RequestNo,
        DateTime? RequestDate,
        int? EmployeeId,
        string? EmployeeCode,
        string? EmployeeName,
        int? LeaveTypeId,
        string? LeaveTypeTxt,
        int? LeaveUnitId,
        string? LeaveUnitTxt,
        double? LeaveDays,
        DateTime? ChildBirthDate,
        DateTime? InLieuDate,
        DateTime? FromDate,
        DateTime? TillDate,
        DateTime? FromTime,
        DateTime? TillTime,
        string? Reason,
        string? Remarks,
        int? StatusId,
        string? StatusTxt,
        DateTime? ApprovalDate,
        int? ApprovedById,
        string? AttachmentDocumentName = null,
        string? AttachmentUploadPath = null,
        string? AttachmentExternalLink = null,
        List<LeaveRequestAttachmentV3Dto>? Attachments = null
    );

    public sealed record LeaveRequestAttachmentV3Dto(
        int? Id,
        string? AttachmentNo,
        int? DocumentTypeId,
        string? DocumentType,
        string? DocumentName,
        string? UploadPath,
        string? ExternalLink,
        string? DownloadUrl = null,
        string? FileName = null,
        string? FileContentBase64 = null
    );

    public sealed record CreateLeaveRequestV3Request(
        int? EmployeeId,
        int? LeaveTypeId,
        int? LeaveUnitId,
        double? LeaveDays,
        DateTime? ChildBirthDate,
        DateTime? InLieuDate,
        DateTime? FromDate,
        DateTime? TillDate,
        DateTime? FromTime,
        DateTime? TillTime,
        string? Reason,
        string? Remarks,
        string? AttachmentDocumentName = null,
        string? AttachmentUploadPath = null,
        string? AttachmentExternalLink = null,
        int? AttachmentDocumentTypeId = null,
        List<LeaveRequestAttachmentV3Dto>? Attachments = null
    );

    public sealed record UpdateLeaveRequestV3Request(
        int Id,
        int? EmployeeId,
        int? LeaveTypeId,
        int? LeaveUnitId,
        double? LeaveDays,
        DateTime? ChildBirthDate,
        DateTime? InLieuDate,
        DateTime? FromDate,
        DateTime? TillDate,
        DateTime? FromTime,
        DateTime? TillTime,
        string? Reason,
        string? Remarks,
        int? StatusId,
        string? AttachmentDocumentName = null,
        string? AttachmentUploadPath = null,
        string? AttachmentExternalLink = null,
        int? AttachmentDocumentTypeId = null,
        List<LeaveRequestAttachmentV3Dto>? Attachments = null
    );
}
