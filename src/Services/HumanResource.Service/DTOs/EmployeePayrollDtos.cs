namespace HumanResource.Service.DTOs
{
    public sealed class EmployeePayrollV3Dto
    {
        public int? Id { get; set; }
        public int EmployeeId { get; set; }
        public int DesignationId { get; set; }
        public bool LoanAllowed { get; set; }
        public bool AdvanceAllowed { get; set; }
        public bool PayFullSalary { get; set; }
        public bool OvertimeAllowed { get; set; }
        public List<int> LoanTypeIds { get; set; } = new();
        public List<EmployeePayrollEarningLineV3Dto> BaseSalaries { get; set; } = new();
        public List<EmployeePayrollEarningLineV3Dto> Allowances { get; set; } = new();
        public List<EmployeePayrollDeductionLineV3Dto> Deductions { get; set; } = new();
    }

    public sealed class EmployeePayrollEarningLineV3Dto
    {
        public int? Id { get; set; }
        public int? TypeId { get; set; }
        public string? TypeName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? Value { get; set; }
        public double? ToValue { get; set; }
        public bool IsPercentage { get; set; }
    }

    public sealed class EmployeePayrollDeductionLineV3Dto
    {
        public int? Id { get; set; }
        public int? TypeId { get; set; }
        public string? TypeName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public double? Value { get; set; }
    }

    public sealed record PayrollLookupItemDto(int Id, string? Code, string? Name);
}
