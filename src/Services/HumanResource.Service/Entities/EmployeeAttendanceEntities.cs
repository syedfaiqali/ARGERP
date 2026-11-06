namespace HumanResource.Service.Entities
{
    public sealed class HRAtnintoutLegacy
    {
        public int Id { get; set; }
        public int? EmpId { get; set; }
        public int? MachineId { get; set; }
        public int? AtnTypeId { get; set; }
        public int? AtnActivityId { get; set; }
        public DateTime? Dttime { get; set; }
        public DateTime? OrgDttime { get; set; }
        public int? ShiftId { get; set; }
        public int? ShiftDtlId { get; set; }
        public int? DayId { get; set; }
        public string? Remarks { get; set; }
        public string? FilePath { get; set; }
        public long? PostTransId { get; set; }
        public int? ClientId { get; set; }
        public int? EntityId { get; set; }
        public DateTime? ShiftIn { get; set; }
        public DateTime? ShiftOut { get; set; }
        public DateTime? SchTimeIn { get; set; }
        public DateTime? SchTimeOut { get; set; }
        public int? LateMin { get; set; }
        public int? LateCount { get; set; }
        public int? EdMin { get; set; }
        public int? EdCount { get; set; }
        public int? OtMin { get; set; }
        public int? OtCount { get; set; }
        public bool UtSysGen { get; set; }
        public bool UtIsPartial { get; set; }
        public int UtRecordVersion { get; set; }
        public bool UtBlock { get; set; }
    }

    public sealed class HRAtnactivityLegacy
    {
        public int Id { get; set; }
        public string? Stxt { get; set; }
        public int? Value { get; set; }
        public int? OrderId { get; set; }
        public int? ClientId { get; set; }
        public bool UtBlock { get; set; }
    }

    public sealed class OrgDepartmentLegacy
    {
        public int Id { get; set; }
        public string? DepCode { get; set; }
        public string? DepStxt { get; set; }
        public int? ClientId { get; set; }
        public int? OpUnitId { get; set; }
        public bool? UtBlock { get; set; }
    }

    public sealed class OrgDesignationLegacy
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Stxt { get; set; }
        public int? ClientId { get; set; }
        public bool? UtBlock { get; set; }
    }



    public sealed class HRDailyAttendanceLegacy
    {
        public long Id { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? AtnDate { get; set; }
        public DateTime? SchTimeIn { get; set; }
        public DateTime? SchTimeOut { get; set; }
        public DateTime? ActShiftIn { get; set; }
        public DateTime? ActShitOut { get; set; } // Matches legacy typo 'actshitout'
        public int? ShiftId { get; set; }
        public int? ShiftDtlId { get; set; }
        public int? DayId { get; set; }
        public string? Remarks { get; set; }
        public int? EntityId { get; set; }
        public int? LateMin { get; set; }
        public int? LateCount { get; set; }
        public int? LateHalfDays { get; set; }  // DB column: lathalfdays
        public int? LateAbsent { get; set; }     // DB column: lateabsent (int)
        public int? LateExcuse { get; set; }     // DB column: lateexcuse (int)
        public int? EdMin { get; set; }
        public int? EdCount { get; set; }
        public int? EdHalfDays { get; set; }
        public int? EdAbsent { get; set; }        // DB column: edabsent (int)
        public int? EdExcuse { get; set; }        // DB column: edexcuse (int)
        public int? OtMin { get; set; }
        public int? OtCount { get; set; }
        public int? OtHalfDays { get; set; }
        public int? DepId { get; set; }
        public int? DesignationId { get; set; }
        public bool IsAbsent { get; set; }
        public bool IsHoliday { get; set; }
        public bool UtSysGen { get; set; }
        public bool UtIsPartial { get; set; }
        public int UtRecordVersion { get; set; }
        public bool UtBlock { get; set; }
    }
}
