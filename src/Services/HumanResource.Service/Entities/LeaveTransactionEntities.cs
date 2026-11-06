using System;

namespace HumanResource.Service.Entities
{
    public sealed class ETLeavetranLegacy
    {
        public int Id { get; set; }
        public string? TransNo { get; set; }
        public DateTime? TransDate { get; set; }
        public int? RequestId { get; set; }
        public int? EmployeeId { get; set; }
        public int? LeaveUnitId { get; set; }
        public int? LeaveTypeId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? TillDate { get; set; }
        public double? LeaveDays { get; set; }
        public int? EntityId { get; set; }
        public int? ClientId { get; set; }
        public bool? IsPaid { get; set; }
        public int? StatusId { get; set; }
        public string? Remarks { get; set; }
        public int? AdmApproval { get; set; }
        public bool UtBlock { get; set; }
        public bool UtSysGen { get; set; }
        public bool UtIsPartial { get; set; }
        public int UtRecordVersion { get; set; }
        public int? UtDocType { get; set; }
        public int? UtSubDocType { get; set; }

        public EmployeeProfileLegacy? Employee { get; set; }
        public ETLeaveRequestLegacy? Request { get; set; }
        public ETLeavetypeLegacy? LeaveType { get; set; }
    }
}

