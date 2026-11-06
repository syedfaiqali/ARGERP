using System;

namespace HumanResource.Service.Entities
{
    public sealed class EPLoanrequestLegacy
    {
        public int Id { get; set; }
        public string? requestno { get; set; }
        public DateTime? requestdate { get; set; }
        public DateTime? requiredate { get; set; }
        public int? employeeid { get; set; }
        public int? loannatureid { get; set; }
        public int? loantypeid { get; set; }
        public double? appliedamount { get; set; }
        public int? reasontypeid { get; set; }
        public string? reason { get; set; }
        public int? dedtypeid { get; set; }
        public double? instalamt { get; set; }
        public int? noofinstal { get; set; }
        public int approvenoofinstallments { get; set; }
        public double approveamounts { get; set; }
        public DateTime? installmentstartdate { get; set; }
        public string HRComments { get; set; } = string.Empty;
        public string LineManagerComments { get; set; } = string.Empty;
        public int? statusid { get; set; }
        public int? entityid { get; set; }
        public int? clientid { get; set; }
        public int? utdoctype { get; set; }
        public int? utsubdoctype { get; set; }
        public bool utsysgen { get; set; }
        public bool utblock { get; set; }
        public bool utispartial { get; set; }
        public int utrecordversion { get; set; }

        public EmployeeProfileLegacy? Employee { get; set; }
        public EPLoantypeLegacy? LoanType { get; set; }
    }
}
