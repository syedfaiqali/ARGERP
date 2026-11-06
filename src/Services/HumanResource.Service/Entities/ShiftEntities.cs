namespace HumanResource.Service.Entities
{
    public sealed class SmShiftLegacy
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Stxt { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? GraceTime { get; set; }
        public int? EntityId { get; set; }
        public int? ClientId { get; set; }
        public bool UtSysGen { get; set; }
        public bool UtIsPartial { get; set; }
        public int UtRecordVersion { get; set; }
        public bool UtBlock { get; set; }
        public bool WoMonday { get; set; }
        public bool WoTuesday { get; set; }
        public bool WoWednesday { get; set; }
        public bool WoThuresday { get; set; }
        public bool WoFriday { get; set; }
        public bool WoSaturday { get; set; }
        public bool WoSunday { get; set; }
    }

    public sealed class ETShiftshareddtlLegacy
    {
        public int Id { get; set; }
        public int? LineIndex { get; set; }
        public int? ShiftId { get; set; }
        public int? LovSharedId { get; set; }
        public int? PClientId { get; set; }
        public int? PEntityId { get; set; }
        public int? PDepartId { get; set; }
        public int? PEmpId { get; set; }
        public bool? UtBlock { get; set; }
    }
}
