using System.ComponentModel.DataAnnotations;

namespace HumanResource.Service.Entities
{
    /// <summary>
    /// Maps to the SMMachineRegistration table in OrganizationDB.
    /// </summary>
    public sealed class SMMachineRegistrationLegacy
    {
        public int Id { get; set; }
        public string? code { get; set; }
        public string? stxt { get; set; }        // DeviceName
        public string? ltxt { get; set; }
        public int? lovmachinetypeid { get; set; }
        public string? machineip { get; set; }   // DeviceIp
        public int? port { get; set; }           // MachinePort
        public bool inactive { get; set; }
        public int? entityid { get; set; }
        public int? clientid { get; set; }
        public bool utsysgen { get; set; }
        public bool utblock { get; set; }
        public bool utispartial { get; set; }
        public int utrecordversion { get; set; }
        public int? siteid { get; set; }
        public int? devicetypeid { get; set; }
        public int? convtytypeid { get; set; }   // ConnectivityTypeId
        public int? deviceid { get; set; }
    }

    /// <summary>
    /// Minimal site entity for lookups inside HumanResource.Service.
    /// Maps to the OrgSite table in OrganizationDB.
    /// </summary>
    public sealed class OrgsiteHrLegacy
    {
        public int Id { get; set; }
        public string? code { get; set; }
        public string? stxt { get; set; }
        public int? entityid { get; set; }
        public int? clientid { get; set; }
        public bool utblock { get; set; }
    }
}
