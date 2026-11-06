using System;
using System.Collections.Generic;

namespace HumanResource.Service.DTOs
{
    public class PaySlipV3Dto
    {
        public int Id { get; set; }
        public int? employeeid { get; set; }
        public string? Name { get; set; }
        public int? deptId { get; set; }
        public string? departmentstxt { get; set; }
        public int? designationid { get; set; }
        public string? designationstxt { get; set; }
        public int? days { get; set; }
        public int? present { get; set; }
        public int? absentscount { get; set; }
        public int? holidays { get; set; }
        public int? leaves { get; set; }
        public double? salary { get; set; }
        public double? allowance { get; set; }
        public double? loanddeduction { get; set; }
        public double? othded { get; set; }
        public double? fixedded { get; set; }
        public double? absded { get; set; }
        public double? overtime { get; set; }
        public double? feededuction { get; set; }
        public double? deduction { get; set; }
        public double? tax { get; set; }
        public double? arrears { get; set; }
        public double? grosssalary { get; set; }
        public string? remarks { get; set; }
        public int? statusid { get; set; }
        public int? entityid { get; set; }
        public int? registerid { get; set; }
        public int? periodid { get; set; }
        public int? perioddtlid { get; set; }
        public int? salarytypeid { get; set; }
        public int? processtypeid { get; set; }
    }

    public class PaySlipFetchRequestV3
    {
        public int registerid { get; set; }
        public int periodid { get; set; }
        public int? salarytypeid { get; set; }
        public int? deptid { get; set; }
        public int? desgid { get; set; }
        public int? employeeid { get; set; }
        public int? processtypeid { get; set; }
        public DateTime? fromdate { get; set; }
        public DateTime? tilldate { get; set; }
    }
}
