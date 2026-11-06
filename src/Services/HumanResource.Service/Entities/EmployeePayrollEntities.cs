namespace HumanResource.Service.Entities
{
    public sealed class EmployeePayrollLegacy
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public int? LoanAllowedId { get; set; }
        public int? AdvanceAllowedId { get; set; }
        public int? OvertimeAllowedId { get; set; }
        public int? OvertimePolicyId { get; set; }
        public bool PayFullSalary { get; set; }
        public bool UtSysGen { get; set; }
        public bool UtIsPartial { get; set; }
        public int UtRecordVersion { get; set; }
        public bool UtBlock { get; set; }
        public bool? IsLoanAllowed { get; set; }
        public bool? IsAdvanceAllowed { get; set; }
        public bool? IsOvertimeAllowed { get; set; }

        public EmployeeProfileLegacy? Employee { get; set; }
        public ICollection<EmployeePayrollBaseSalaryLegacy> BaseSalaries { get; set; } = new List<EmployeePayrollBaseSalaryLegacy>();
        public ICollection<EmployeePayrollAllowanceLegacy> Allowances { get; set; } = new List<EmployeePayrollAllowanceLegacy>();
        public ICollection<EmployeePayrollDeductionLegacy> Deductions { get; set; } = new List<EmployeePayrollDeductionLegacy>();
        public ICollection<EmployeePayrollLoanTypeLegacy> LoanTypes { get; set; } = new List<EmployeePayrollLoanTypeLegacy>();
    }

    public abstract class EmployeePayrollAmountDetailLegacy
    {
        public int Id { get; set; }
        public int? PayrollId { get; set; }
        public int? LineIndex { get; set; }
        public double? Value { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool UtSysGen { get; set; }
        public bool UtBlock { get; set; }
        public bool UtIsPartial { get; set; }
        public int UtRecordVersion { get; set; }
    }

    public sealed class EmployeePayrollBaseSalaryLegacy
    {
        // ER...basesalarydtl.Id is BIGINT in the deployed database.
        public long Id { get; set; }
        public int? PayrollId { get; set; }
        public int? LineIndex { get; set; }
        public int? EarningTypeId { get; set; }
        public double? Value { get; set; }
        public double? ToValue { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool UtSysGen { get; set; }
        public bool UtBlock { get; set; }
        public bool UtIsPartial { get; set; }
        public int UtRecordVersion { get; set; }
        public EmployeePayrollLegacy? Payroll { get; set; }
        public EPEarningtypeLegacy? EarningType { get; set; }
    }

    public sealed class EmployeePayrollAllowanceLegacy : EmployeePayrollAmountDetailLegacy
    {
        public int? EarningTypeId { get; set; }
        public bool IsPercentage { get; set; }
        public EmployeePayrollLegacy? Payroll { get; set; }
        public EPEarningtypeLegacy? EarningType { get; set; }
    }

    public sealed class EmployeePayrollDeductionLegacy : EmployeePayrollAmountDetailLegacy
    {
        public int? DeductionTypeId { get; set; }
        public EmployeePayrollLegacy? Payroll { get; set; }
        public EPDeductiontypeLegacy? DeductionType { get; set; }
    }

    public sealed class EmployeePayrollLoanTypeLegacy
    {
        public int Id { get; set; }
        public int? PayrollId { get; set; }
        public int? LoanTypeId { get; set; }
        public bool UtBlock { get; set; }
        public int UtRecordVersion { get; set; }

        public EmployeePayrollLegacy? Payroll { get; set; }
        public EPLoantypeLegacy? LoanType { get; set; }
    }
}
