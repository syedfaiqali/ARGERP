using System.ComponentModel.DataAnnotations;

namespace HumanResource.Service.Entities
{
    public class ERPositiontypeLegacy
    {
        public int Id { get; set; }
        [MaxLength(10)]
        public string? Code { get; set; }
        public int? EmpTypeId { get; set; }
        [MaxLength(150)]
        public string? Stxt { get; set; }
        [MaxLength(500)]
        public string? Ltxt { get; set; }
        public int? EntityId { get; set; }
        public int? ClientId { get; set; }
        public bool? UtBlock { get; set; }
    }

    public class EREmploymenttypeLegacy
    {
        public int Id { get; set; }
        [MaxLength(10)]
        public string? Code { get; set; }
        [MaxLength(150)]
        public string? Stxt { get; set; }
        [MaxLength(500)]
        public string? Ltxt { get; set; }
    }

    public class EREmpPositionLegacy
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public int? DepartmentId { get; set; }
        public int? DesignationId { get; set; }
        public int? GradeId { get; set; }
        public int? ActionId { get; set; }
        [MaxLength(1500)]
        public string? Remarks { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? TillDate { get; set; }
        [MaxLength(1500)]
        public string? Responsibility { get; set; }
        public bool? UtBlock { get; set; }
        public DateTime? UtEditTime { get; set; }
        public int UtRecordVersion { get; set; }

        public virtual EmployeeProfileLegacy? Employee { get; set; }
        public virtual OrgDepartmentLegacy? Department { get; set; }
        public virtual OrgDesignationLegacy? Designation { get; set; }
    }
}
