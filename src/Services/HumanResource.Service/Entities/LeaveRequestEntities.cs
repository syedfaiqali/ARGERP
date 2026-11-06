using System;

namespace HumanResource.Service.Entities
{
    public sealed class ETLeaveRequestLegacy
    {
        public int Id { get; set; }
        public string? RequestNo { get; set; }
        public DateTime? RequestDate { get; set; }
        public int? EmployeeId { get; set; }
        public int? LeaveTypeId { get; set; }
        public int? LeaveUnitId { get; set; }
        public double? LeaveDays { get; set; }
        public DateTime? DOB { get; set; }
        public DateTime? InLieuDate { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? TillDate { get; set; }
        public DateTime? FromTime { get; set; }
        public DateTime? TillTime { get; set; }
        public string? Reason { get; set; }
        public string? Remarks { get; set; }
        public int? StatusId { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public int? ApprovedById { get; set; }
        public int? EntityId { get; set; }
        public int? ClientId { get; set; }
        public bool UtBlock { get; set; }
        public bool UtSysGen { get; set; } = false;
        public bool UtIsPartial { get; set; } = false;
        public int UtRecordVersion { get; set; } = 0;

        // Navigation properties (if needed)
        public EmployeeProfileLegacy? Employee { get; set; }
        public ETLeavetypeLegacy? LeaveType { get; set; }
    }
}
