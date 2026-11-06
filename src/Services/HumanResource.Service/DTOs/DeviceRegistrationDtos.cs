namespace HumanResource.Service.DTOs
{
    // ── Lookup DTOs ───────────────────────────────────────────────────────────

    public sealed class DeviceRegistrationLookupItemV3Dto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
    }

    public sealed class DeviceRegistrationLookupsV3Dto
    {
        public List<DeviceRegistrationLookupItemV3Dto> DeviceTypes { get; set; } = new();
        public List<DeviceRegistrationLookupItemV3Dto> Sites { get; set; } = new();
        public List<DeviceRegistrationLookupItemV3Dto> ConnectivityTypes { get; set; } = new();
    }

    // ── List / Detail DTO ────────────────────────────────────────────────────

    public sealed class DeviceRegistrationV3Dto
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public int DeviceTypeId { get; set; }
        public string DeviceTypeName { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public int SiteId { get; set; }
        public string SiteName { get; set; } = string.Empty;
        public string DeviceIp { get; set; } = string.Empty;
        public int MachinePort { get; set; }
        public int ConnectivityTypeId { get; set; }
        public string ConnectivityTypeName { get; set; } = string.Empty;
        public int? DeviceId { get; set; }
        public bool IsActive { get; set; }   // inverse of legacy `inactive`
    }

    // ── Upsert Request ────────────────────────────────────────────────────────

    public sealed class DeviceRegistrationUpsertV3Request
    {
        public string Code { get; set; } = string.Empty;
        public int DeviceTypeId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public int SiteId { get; set; }
        public string DeviceIp { get; set; } = string.Empty;
        public int MachinePort { get; set; }
        public int ConnectivityTypeId { get; set; }
        public int? DeviceId { get; set; }
        public bool IsActive { get; set; } = true;
        public int? EntityId { get; set; }   // optional override; resolved server-side
    }
}
