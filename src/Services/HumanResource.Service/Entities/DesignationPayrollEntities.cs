namespace HumanResource.Service.Entities
{
    public sealed class DesignationPayrollLegacy
    {
        public int Id { get; set; }
        public int? DesignationId { get; set; }
        public int? ClientId { get; set; }
        public bool LoanAllowed { get; set; }
        public bool AdvanceAllowed { get; set; }
        public bool PayFullSalary { get; set; }
        public bool OvertimeAllowed { get; set; }
        public bool UtBlock { get; set; }
        public int UtRecordVersion { get; set; }
        public ICollection<DesignationPayrollBaseSalaryLegacy> BaseSalaries { get; set; } = new List<DesignationPayrollBaseSalaryLegacy>();
        public ICollection<DesignationPayrollAllowanceLegacy> Allowances { get; set; } = new List<DesignationPayrollAllowanceLegacy>();
        public ICollection<DesignationPayrollDeductionLegacy> Deductions { get; set; } = new List<DesignationPayrollDeductionLegacy>();
        public ICollection<DesignationPayrollLoanTypeLegacy> LoanTypes { get; set; } = new List<DesignationPayrollLoanTypeLegacy>();
    }

    public abstract class DesignationPayrollAmountDetailLegacy
    {
        public int Id { get; set; }
        public int? PayrollId { get; set; }
        public int? LineIndex { get; set; }
        public double? Value { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool UtBlock { get; set; }
        public int UtRecordVersion { get; set; }
    }

    public sealed class DesignationPayrollBaseSalaryLegacy : DesignationPayrollAmountDetailLegacy
    {
        public int? EarningTypeId { get; set; }
        public double? ToValue { get; set; }
        public DesignationPayrollLegacy? Payroll { get; set; }
        public EPEarningtypeLegacy? EarningType { get; set; }
    }

    public sealed class DesignationPayrollAllowanceLegacy : DesignationPayrollAmountDetailLegacy
    {
        public int? EarningTypeId { get; set; }
        public bool IsPercentage { get; set; }
        public DesignationPayrollLegacy? Payroll { get; set; }
        public EPEarningtypeLegacy? EarningType { get; set; }
    }

    public sealed class DesignationPayrollDeductionLegacy : DesignationPayrollAmountDetailLegacy
    {
        public int? DeductionTypeId { get; set; }
        public DesignationPayrollLegacy? Payroll { get; set; }
        public EPDeductiontypeLegacy? DeductionType { get; set; }
    }

    public sealed class DesignationPayrollLoanTypeLegacy
    {
        public int Id { get; set; }
        public int? PayrollId { get; set; }
        public int? LoanTypeId { get; set; }
        public bool UtBlock { get; set; }
        public int UtRecordVersion { get; set; }
        public DesignationPayrollLegacy? Payroll { get; set; }
        public EPLoantypeLegacy? LoanType { get; set; }
    }
}
