namespace HumanResource.Service.Entities
{
    public sealed class ScheduleHeaderLegacy
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Stxt { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? TillDate { get; set; }
        public int? ScheduleFor { get; set; }
        public int? ShiftId { get; set; }
        public int? AssignTo { get; set; }
        public int? ClientId { get; set; }
        public int? EntityId { get; set; }
        public bool UtSysGen { get; set; } = false;
        public bool UtIsPartial { get; set; } = false;
        public int UtRecordVersion { get; set; } = 0;
        public bool? UtBlock { get; set; }
    }

    public sealed class ScheduleDetailLegacy
    {
        public int Id { get; set; }
        public int? ScheduleId { get; set; }
        public int? LineIndex { get; set; }
        public DateTime? SchDate { get; set; }
        public bool? Holiday { get; set; }
        public int? ShiftId { get; set; }
        public DateTime? TimeIn { get; set; }
        public DateTime? TimeOut { get; set; }
        public double? Duration { get; set; }
        public bool UtSysGen { get; set; } = false;
        public bool UtIsPartial { get; set; } = false;
        public int UtRecordVersion { get; set; } = 0;
        public bool? UtBlock { get; set; }
    }
}
