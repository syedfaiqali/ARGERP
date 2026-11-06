using System.ComponentModel.DataAnnotations;

namespace HumanResource.Service.Entities
{
    public sealed class EmployeeDiseaseLegacy
    {
        public int Id { get; set; }
        public int? LineIndex { get; set; }
        public int? EmployeeId { get; set; }
        public int? DiseaseId { get; set; }
        public int? DiseaseCategoryId { get; set; }
        public int? DiseaseTypeId { get; set; }
        [MaxLength(150)]
        public string? Stxt { get; set; }
        [MaxLength(500)]
        public string? Ltxt { get; set; }
        public int? UtDocType { get; set; }
        public int? UtSubDocType { get; set; }
        public bool UtSysGen { get; set; }
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

        public EmployeeProfileLegacy? Employee { get; set; }
        public SMDiseaseHrLegacy? Disease { get; set; }
        public UtLovLegacy? DiseaseCategory { get; set; }
        public UtLovLegacy? DiseaseType { get; set; }
    }

    public sealed class SMDiseaseHrLegacy
    {
        public int Id { get; set; }
        [MaxLength(20)]
        public string? Code { get; set; }
        public int? DiseaseCategoryId { get; set; }
        public int? DiseaseTypeId { get; set; }
        [MaxLength(150)]
        public string? Stxt { get; set; }
        [MaxLength(500)]
        public string? Ltxt { get; set; }
        public int? ClientId { get; set; }
        public int? EntityId { get; set; }
        public bool UtBlock { get; set; }
    }
}
