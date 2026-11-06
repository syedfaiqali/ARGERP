using System.ComponentModel.DataAnnotations;

namespace HumanResource.Service.Entities
{
    public sealed class EmployeeCardLegacy
    {
        public int Id { get; set; }
        [MaxLength(20)]
        public string? Code { get; set; }
        public int? EmployeeId { get; set; }
        public int? CardTypeId { get; set; }
        public int? AuthorityId { get; set; }
        [MaxLength(25)]
        public string? CardNo { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTill { get; set; }
        public int? UtDocType { get; set; }
        public int? UtSubDocType { get; set; }
        public bool UtSysGen { get; set; }
        public bool UtBlock1 { get; set; }
        public bool UtIsPartial { get; set; }
        public DateTime? UtAddTime { get; set; }
        [MaxLength(25)]
        public string? UtAddUser { get; set; }
        [MaxLength(25)]
        public string? UtAddSystem { get; set; }
        [MaxLength(25)]
        public string? UtAddIp { get; set; }
        [MaxLength(10)]
        public string? UtAddAppVersion { get; set; }
        [MaxLength(10)]
        public string? UtAddDbVersion { get; set; }
        public long? UtAddId { get; set; }
        public int? UtAddSessionId { get; set; }
        public DateTime? UtEditTime { get; set; }
        [MaxLength(25)]
        public string? UtEditUser { get; set; }
        [MaxLength(25)]
        public string? UtEditSystem { get; set; }
        [MaxLength(25)]
        public string? UtEditIp { get; set; }
        [MaxLength(10)]
        public string? UtEditAppVersion { get; set; }
        [MaxLength(10)]
        public string? UtEditDbVersion { get; set; }
        public long? UtEditId { get; set; }
        public int? UtEditSessionId { get; set; }
        public int UtRecordVersion { get; set; }
        public bool UtBlock { get; set; }
        public int? CrdTypeId { get; set; }
        public int? AuthTypeId { get; set; }
        public int? AuthId { get; set; }

        public EmployeeProfileLegacy? Employee { get; set; }
        public UtLovLegacy? CardType { get; set; }
        public UtLovLegacy? Authority { get; set; }
    }
}
