using System;
using System.Collections.Generic;

namespace HumanResource.Service.Entities
{
    public sealed class HRpayrollprocessLegacy
    {
        public int Id { get; set; }
        public string? transno { get; set; }
        public DateTime? transdate { get; set; }
        public int? registerid { get; set; }
        public int? periodid { get; set; }
        public int? salarytypeid { get; set; }
        public int? processtypeid { get; set; }
        public int? employeeid { get; set; }
        public int? deptid { get; set; }
        public int? desgid { get; set; }
        public int? statusid { get; set; }
        public int? days { get; set; }
        public int? present { get; set; }
        public int? absent { get; set; }
        public int? leaves { get; set; }
        public int? holidays { get; set; }
        public double? salary { get; set; }
        public double? allowance { get; set; }
        public double? deduction { get; set; }
        public double? tax { get; set; }
        public double? grosssalary { get; set; }
        public string? remarks { get; set; }
        public int? entityid { get; set; }
        public int? clientid { get; set; }
        public int? perioddtlid { get; set; }
        public double? loanddeduction { get; set; }
        public double? latededuction { get; set; }
        public double? fixedded { get; set; }
        public double? absded { get; set; }
        public double? overtime { get; set; }
        public double? feededuction { get; set; }
        public double? arrears { get; set; }
        public int? othded { get; set; }
        public DateTime? tilldate { get; set; }
        public DateTime? fromdate { get; set; }

        public bool utsysgen { get; set; }
        public bool utblock { get; set; }
        public bool utispartial { get; set; }
        public int utrecordversion { get; set; }

        public EmployeeProfileLegacy? Employee { get; set; }
        public ICollection<HRpayrollprocessdtlLegacy> HRpayrollprocessdtls { get; set; } = new List<HRpayrollprocessdtlLegacy>();
    }

    public sealed class HRpayrollprocessdtlLegacy
    {
        public int Id { get; set; }
        public int? processid { get; set; }
        public int? lovtypeid { get; set; } // 1079: Base Salary, 1080: Allowance, 1081: Deduction
        public int? typeid { get; set; }
        public DateTime? fromdate { get; set; }
        public DateTime? tilldate { get; set; }
        public double? value { get; set; }

        public bool utsysgen { get; set; }
        public bool utblock { get; set; }
        public bool utispartial { get; set; }
        public int utrecordversion { get; set; }

        public HRpayrollprocessLegacy? PayrollProcess { get; set; }
    }
}
