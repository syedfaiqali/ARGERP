using System;
using System.Collections.Generic;

namespace HumanResource.Service.Entities
{
    public sealed class EPLoantranshdrLegacy
    {
        public int Id { get; set; }
        public string? loanno { get; set; }
        public DateTime? loandate { get; set; }
        public DateTime? postingdate { get; set; }
        public int? typeid { get; set; } // Loan Category
        public int? requestid { get; set; }
        public int? employeeid { get; set; }
        public int? loantypeid { get; set; }
        public double? appliedamount { get; set; }
        public double? loanamount { get; set; }
        public double? instalamt { get; set; }
        public int? noofinstal { get; set; }
        public int? recoveryyear { get; set; }
        public int? recoverymonth { get; set; }
        public int? dedtypeid { get; set; }
        public int? payperiodid { get; set; }
        public int? perioddtlid { get; set; }
        public string? reason { get; set; }
        public int? statusid { get; set; }
        public int TrusteeStatusId { get; set; }
        public int? entityid { get; set; }
        public int? clientid { get; set; }
        public int? utdoctype { get; set; }
        public int? utsubdoctype { get; set; }
        public bool utsysgen { get; set; }
        public bool utblock { get; set; }
        public bool utispartial { get; set; }
        public int utrecordversion { get; set; }
        public DateTime? utaddtime { get; set; }
        public string? utadduser { get; set; }

        public EmployeeProfileLegacy? Employee { get; set; }
        public EPLoanrequestLegacy? LoanRequest { get; set; }
        public UtLovLegacy? TrusteeStatus { get; set; }
        public ICollection<EPLoantransdtlLegacy> EPLoantransdtls { get; set; } = new List<EPLoantransdtlLegacy>();
    }

    public sealed class EPLoantransdtlLegacy
    {
        public int Id { get; set; }
        public int? loanid { get; set; }
        public int? year { get; set; }
        public int? monthid { get; set; }
        public double? installment { get; set; }
        public double? balance { get; set; }
        public bool recovered { get; set; }
        public bool merged { get; set; }
        public bool notdeduct { get; set; }
        public int? mergeloanid { get; set; }
        public string? reason { get; set; }
        public int? clientid { get; set; }
        public int? entityid { get; set; }
        public bool utsysgen { get; set; }
        public bool utblock { get; set; }
        public bool utispartial { get; set; }
        public int utrecordversion { get; set; }

        public EPLoantranshdrLegacy? LoanIssuance { get; set; }
    }
}
