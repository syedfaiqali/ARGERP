using System;

namespace HumanResource.Service.Entities
{
    public sealed class HRMonthlyAttendanceLegacy
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public int? RegisterId { get; set; }
        public int? PayPeriodId { get; set; }
        public int? PayPeriodDetailId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? DisbDate { get; set; }
        public int? EntityId { get; set; }
        public double? Days { get; set; }
        public double? PresentsCount { get; set; }
        public double? AbsentsCount { get; set; }
        public double? HalfDaysCount { get; set; }
        public double? LateCount { get; set; }
        public double? EarlyCounts { get; set; }
        public double? ActualDays { get; set; }
        public double? ActualPresent { get; set; }
        public double? Holidays { get; set; }
        public int? Leaves { get; set; }
        public int? Deptid { get; set; }
        public int? Desgid { get; set; }
        public double? Salary { get; set; }
        public double? Allowance { get; set; }
        public double? Deduction { get; set; }
        public double? Loanddeduction { get; set; }
        public double? Grosssalary { get; set; }
        public double? Tax { get; set; }
        public bool UtBlock { get; set; }

        public EmployeeProfileLegacy? Employee { get; set; }
        public OrgDepartmentLegacy? Department { get; set; }
        public OrgDesignationLegacy? Designation { get; set; }
    }

    public sealed class ETShiftdtlLegacy
    {
        public int Id { get; set; }
        public int? ShiftId { get; set; }
        public int? LineIndex { get; set; }
        public string? Stxt { get; set; }
        public int? DayId { get; set; }
        public DateTime? InTime { get; set; }
        public DateTime? OutTime { get; set; }
        public int? GraceTime { get; set; }
        public int? GraceTimeEnd { get; set; }
        public bool? LatePolicyApplicable { get; set; }
        public bool? EarlyPolicyApplicable { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? Duration { get; set; }
        public bool? UtBlock { get; set; }
    }

    public sealed class ETShiftsubdtlLegacy
    {
        public int Id { get; set; }
        public int? ShiftDtlId { get; set; }
        public int? LineIndex { get; set; }
        public string? Stxt { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double? Duration { get; set; }
        public bool? UtBlock { get; set; }
    }
}
