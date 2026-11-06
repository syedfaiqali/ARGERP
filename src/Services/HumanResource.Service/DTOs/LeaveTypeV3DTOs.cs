using System;

namespace HumanResource.Service.DTOs
{
    public class LeaveTypeV3Dto
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? LeaveType { get; set; }
        public string? RenewOn { get; set; }
        public string? LeaveUnit { get; set; }
        public double Leaves { get; set; }
        public string? MaxAvailUnit { get; set; }
        public string? MaxAvail { get; set; }
        public string? MaritalStatus { get; set; }
        public string? CarryForwardVal { get; set; }
        public string? CarryForwardLookup { get; set; }
        public string? EncashmentVal { get; set; }
        public string? EncashmentLookup { get; set; }
        public string? Gender { get; set; }
        public string? EntitleOn { get; set; }
        public string? AccrualUnit { get; set; }
        public string? EntitleLeaves { get; set; }
        public bool AllowInProb { get; set; }
        public bool QuotaValidate { get; set; }
        public bool LateAdjustable { get; set; }
        public bool EarlyDepAdjustable { get; set; }
        public bool SpecialLeave { get; set; }
        public bool IncludeHolidays { get; set; }
        public bool ExcludeWeeklyHolidays { get; set; }
        public bool PaidLeave { get; set; }
        public string? RequestBefore { get; set; }
        public string? RequestUnit { get; set; }
        public string? NoOfMonth { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateLeaveTypeV3Request
    {
        public string? Code { get; set; }
        public string? LeaveType { get; set; }
        public string? RenewOn { get; set; }
        public string? LeaveUnit { get; set; }
        public double Leaves { get; set; }
        public string? MaxAvailUnit { get; set; }
        public string? MaxAvail { get; set; }
        public string? MaritalStatus { get; set; }
        public string? CarryForwardVal { get; set; }
        public string? CarryForwardLookup { get; set; }
        public string? EncashmentVal { get; set; }
        public string? EncashmentLookup { get; set; }
        public string? Gender { get; set; }
        public string? EntitleOn { get; set; }
        public string? AccrualUnit { get; set; }
        public string? EntitleLeaves { get; set; }
        public bool AllowInProb { get; set; }
        public bool QuotaValidate { get; set; }
        public bool LateAdjustable { get; set; }
        public bool EarlyDepAdjustable { get; set; }
        public bool SpecialLeave { get; set; }
        public bool IncludeHolidays { get; set; }
        public bool ExcludeWeeklyHolidays { get; set; }
        public bool PaidLeave { get; set; }
        public string? RequestBefore { get; set; }
        public string? RequestUnit { get; set; }
        public string? NoOfMonth { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateLeaveTypeV3Request : CreateLeaveTypeV3Request
    {
        public int Id { get; set; }
    }
}
