namespace HumanResource.Service.Entities
{
    public sealed class SMCustomShiftHeaderLegacy
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Stxt { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? GraceTime { get; set; }
        public int? EntityId { get; set; }
        public int? ClientId { get; set; }
        public int? UtDocType { get; set; }
        public int? UtSubDocType { get; set; }
        public bool UtSysGen { get; set; }
        public bool UtIsPartial { get; set; }
        public DateTime? UtAddTime { get; set; }
        public string? UtAddUser { get; set; }
        public string? UtAddSystem { get; set; }
        public string? UtAddIp { get; set; }
        public string? UtAddAppVersion { get; set; }
        public string? UtAddDbVersion { get; set; }
        public long? UtAddId { get; set; }
        public int? UtAddSessionId { get; set; }
        public DateTime? UtEditTime { get; set; }
        public string? UtEditUser { get; set; }
        public string? UtEditSystem { get; set; }
        public string? UtEditIp { get; set; }
        public string? UtEditAppVersion { get; set; }
        public string? UtEditDbVersion { get; set; }
        public long? UtEditId { get; set; }
        public int? UtEditSessionId { get; set; }
        public int UtRecordVersion { get; set; }
        public bool UtBlock { get; set; }
        public int? ShiftTypeId { get; set; }
        public bool WoMonday { get; set; }
        public bool WoTuesday { get; set; }
        public bool WoWednesday { get; set; }
        public bool WoThuresday { get; set; }
        public bool WoFriday { get; set; }
        public bool WoSaturday { get; set; }
        public bool WoSunday { get; set; }
        public int? GraceTimeEd { get; set; }
    }

    public sealed class SMCustomShiftDetailLegacy
    {
        public int Id { get; set; }
        public int? ShiftId { get; set; }
        public int? LineIndex { get; set; }
        public string? Stxt { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public double? Duration { get; set; }
        public int? UtDocType { get; set; }
        public int? UtSubDocType { get; set; }
        public bool UtSysGen { get; set; }
        public bool UtIsPartial { get; set; }
        public DateTime? UtAddTime { get; set; }
        public string? UtAddUser { get; set; }
        public string? UtAddSystem { get; set; }
        public string? UtAddIp { get; set; }
        public string? UtAddAppVersion { get; set; }
        public string? UtAddDbVersion { get; set; }
        public long? UtAddId { get; set; }
        public int? UtAddSessionId { get; set; }
        public DateTime? UtEditTime { get; set; }
        public string? UtEditUser { get; set; }
        public string? UtEditSystem { get; set; }
        public string? UtEditIp { get; set; }
        public string? UtEditAppVersion { get; set; }
        public string? UtEditDbVersion { get; set; }
        public long? UtEditId { get; set; }
        public int? UtEditSessionId { get; set; }
        public int UtRecordVersion { get; set; }
        public bool UtBlock { get; set; }
    }
}
