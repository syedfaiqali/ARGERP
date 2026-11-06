using System.ComponentModel.DataAnnotations;

namespace HumanResource.Service.Entities
{
    public sealed class EmployeeExperienceLegacy
    {
        public int Id { get; set; }
        [MaxLength(20)]
        public string? Code { get; set; }
        public int? EmployeeId { get; set; }
        public int? BusinessNatureId { get; set; }
        [MaxLength(150)]
        public string? Company { get; set; }
        [MaxLength(50)]
        public string? Department { get; set; }
        [MaxLength(50)]
        public string? Designation { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Salary { get; set; }
        [MaxLength(500)]
        public string? Address { get; set; }
        [MaxLength(15)]
        public string? Phone { get; set; }
        [MaxLength(50)]
        public string? Email { get; set; }
        [MaxLength(50)]
        public string? Web { get; set; }
        [MaxLength(1500)]
        public string? Responsibility { get; set; }
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
        [MaxLength(150)]
        public string? ContactPerson { get; set; }
        [MaxLength(150)]
        public string? PersonDesignation { get; set; }
        [MaxLength(150)]
        public string? PersonAddress { get; set; }
        [MaxLength(20)]
        public string? PersonPhone { get; set; }
        [MaxLength(100)]
        public string? PersonEmail { get; set; }
        [MaxLength(100)]
        public string? PersonWeb { get; set; }

        public EmployeeProfileLegacy? Employee { get; set; }
        public UtLovLegacy? BusinessNature { get; set; }
    }
}
