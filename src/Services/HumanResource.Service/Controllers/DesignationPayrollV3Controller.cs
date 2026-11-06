using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/HumanResource/DesignationPayroll/v3")]
    [Route("api/DesignationPayroll/v3")]
    public sealed class DesignationPayrollV3Controller : ControllerBase
    {
        private readonly HumanResourceDbContext _context;

        public DesignationPayrollV3Controller(HumanResourceDbContext context) => _context = context;

        [HttpGet]
        public async Task<ActionResult<EmployeePayrollV3Dto>> Get([FromQuery] int designationId)
        {
            if (designationId <= 0) return BadRequest("designationId is required.");
            if (!await IsDesignationInScope(designationId, GetClientId())) return NotFound();

            var payroll = await PayrollQuery()
                .FirstOrDefaultAsync(x => x.DesignationId == designationId && !x.UtBlock);
            return Ok(payroll == null
                ? new EmployeePayrollV3Dto { DesignationId = designationId }
                : Map(payroll));
        }

        [HttpPost]
        public async Task<ActionResult<EmployeePayrollV3Dto>> Save([FromBody] EmployeePayrollV3Dto request)
        {
            var clientId = GetClientId();
            if (request.DesignationId <= 0) return BadRequest("Designation is required.");
            if (!await IsDesignationInScope(request.DesignationId, clientId))
                return BadRequest("Designation does not belong to the current client.");

            var validationError = Validate(request);
            if (validationError != null) return BadRequest(validationError);

            var loanTypeIds = request.LoanTypeIds.Distinct().Where(id => id > 0).ToList();
            if (loanTypeIds.Count != request.LoanTypeIds.Count)
                return BadRequest("Select valid Loan Types.");
            if (loanTypeIds.Count > 0)
            {
                var validLoanTypeCount = await _context.LoanTypes
                    .CountAsync(x => loanTypeIds.Contains(x.Id) && x.clientid == clientId && !x.utblock);
                if (validLoanTypeCount != loanTypeIds.Count)
                    return BadRequest("One or more selected Loan Types are inactive or unavailable.");
            }

            var payroll = await _context.DesignationPayrolls
                .Include(x => x.BaseSalaries)
                .Include(x => x.Allowances)
                .Include(x => x.Deductions)
                .Include(x => x.LoanTypes)
                .FirstOrDefaultAsync(x => x.DesignationId == request.DesignationId && x.ClientId == clientId && !x.UtBlock);

            if (payroll == null)
            {
                payroll = new DesignationPayrollLegacy
                {
                    Id = (await _context.DesignationPayrolls.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1,
                    DesignationId = request.DesignationId,
                    ClientId = clientId,
                    UtBlock = false,
                    UtRecordVersion = 1,
                };
                _context.DesignationPayrolls.Add(payroll);
            }

            payroll.LoanAllowed = request.LoanAllowed;
            payroll.AdvanceAllowed = request.AdvanceAllowed;
            payroll.PayFullSalary = request.PayFullSalary;
            payroll.OvertimeAllowed = request.OvertimeAllowed;
            payroll.UtRecordVersion = payroll.UtRecordVersion <= 0 ? 1 : payroll.UtRecordVersion + 1;

            _context.DesignationPayrollBaseSalaries.RemoveRange(payroll.BaseSalaries);
            _context.DesignationPayrollAllowances.RemoveRange(payroll.Allowances);
            _context.DesignationPayrollDeductions.RemoveRange(payroll.Deductions);
            _context.DesignationPayrollLoanTypes.RemoveRange(payroll.LoanTypes);

            var nextBaseId = await NextId(_context.DesignationPayrollBaseSalaries);
            var nextAllowanceId = await NextId(_context.DesignationPayrollAllowances);
            var nextDeductionId = await NextId(_context.DesignationPayrollDeductions);
            var nextLoanTypeId = (await _context.DesignationPayrollLoanTypes.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1;

            var baseSalaries = request.BaseSalaries.Select((line, index) => new DesignationPayrollBaseSalaryLegacy
            {
                Id = nextBaseId++, PayrollId = payroll.Id, LineIndex = index + 1,
                EarningTypeId = line.TypeId, StartDate = line.StartDate, EndDate = line.EndDate,
                Value = line.Value, ToValue = line.ToValue, UtBlock = false, UtRecordVersion = 1,
            }).ToList();
            var allowances = request.Allowances.Select((line, index) => new DesignationPayrollAllowanceLegacy
            {
                Id = nextAllowanceId++, PayrollId = payroll.Id, LineIndex = index + 1,
                EarningTypeId = line.TypeId, StartDate = line.StartDate, EndDate = line.EndDate,
                Value = line.Value, IsPercentage = line.IsPercentage, UtBlock = false, UtRecordVersion = 1,
            }).ToList();
            var deductions = request.Deductions.Select((line, index) => new DesignationPayrollDeductionLegacy
            {
                Id = nextDeductionId++, PayrollId = payroll.Id, LineIndex = index + 1,
                DeductionTypeId = line.TypeId, StartDate = line.StartDate, EndDate = line.EndDate,
                Value = line.Value, UtBlock = false, UtRecordVersion = 1,
            }).ToList();
            var loanTypes = loanTypeIds.Select(loanTypeId => new DesignationPayrollLoanTypeLegacy
            {
                Id = nextLoanTypeId++, PayrollId = payroll.Id, LoanTypeId = loanTypeId,
                UtBlock = false, UtRecordVersion = 1,
            }).ToList();

            payroll.BaseSalaries = baseSalaries;
            payroll.Allowances = allowances;
            payroll.Deductions = deductions;
            payroll.LoanTypes = loanTypes;
            _context.DesignationPayrollBaseSalaries.AddRange(baseSalaries);
            _context.DesignationPayrollAllowances.AddRange(allowances);
            _context.DesignationPayrollDeductions.AddRange(deductions);
            _context.DesignationPayrollLoanTypes.AddRange(loanTypes);
            await _context.SaveChangesAsync();

            var saved = await PayrollQuery().FirstAsync(x => x.Id == payroll.Id);
            return Ok(Map(saved));
        }

        private IQueryable<DesignationPayrollLegacy> PayrollQuery() => _context.DesignationPayrolls
            .Include(x => x.BaseSalaries).ThenInclude(x => x.EarningType)
            .Include(x => x.Allowances).ThenInclude(x => x.EarningType)
            .Include(x => x.Deductions).ThenInclude(x => x.DeductionType)
            .Include(x => x.LoanTypes).ThenInclude(x => x.LoanType);

        private static EmployeePayrollV3Dto Map(DesignationPayrollLegacy payroll) => new()
        {
            Id = payroll.Id,
            DesignationId = payroll.DesignationId ?? 0,
            LoanAllowed = payroll.LoanAllowed,
            AdvanceAllowed = payroll.AdvanceAllowed,
            PayFullSalary = payroll.PayFullSalary,
            OvertimeAllowed = payroll.OvertimeAllowed,
            LoanTypeIds = payroll.LoanTypes.Where(x => !x.UtBlock && x.LoanTypeId.HasValue)
                .Select(x => x.LoanTypeId!.Value).OrderBy(id => id).ToList(),
            BaseSalaries = payroll.BaseSalaries.Where(x => !x.UtBlock).OrderBy(x => x.LineIndex)
                .Select(x => new EmployeePayrollEarningLineV3Dto { Id = x.Id, TypeId = x.EarningTypeId, TypeName = x.EarningType?.Stxt, StartDate = x.StartDate, EndDate = x.EndDate, Value = x.Value, ToValue = x.ToValue }).ToList(),
            Allowances = payroll.Allowances.Where(x => !x.UtBlock).OrderBy(x => x.LineIndex)
                .Select(x => new EmployeePayrollEarningLineV3Dto { Id = x.Id, TypeId = x.EarningTypeId, TypeName = x.EarningType?.Stxt, StartDate = x.StartDate, EndDate = x.EndDate, Value = x.Value, IsPercentage = x.IsPercentage }).ToList(),
            Deductions = payroll.Deductions.Where(x => !x.UtBlock).OrderBy(x => x.LineIndex)
                .Select(x => new EmployeePayrollDeductionLineV3Dto { Id = x.Id, TypeId = x.DeductionTypeId, TypeName = x.DeductionType?.Stxt, StartDate = x.StartDate, EndDate = x.EndDate, Value = x.Value }).ToList(),
        };

        private static string? Validate(EmployeePayrollV3Dto request)
        {
            var hasSalaryAmounts = request.Allowances.Count > 0 || request.Deductions.Count > 0;
            if (request.BaseSalaries.Count == 0 && request.LoanTypeIds.Count > 0 && !hasSalaryAmounts) return null;
            if (request.BaseSalaries.Count != 1) return request.BaseSalaries.Count == 0 ? "Base Salary cannot be empty." : "Only one Base Salary row is allowed.";
            if (request.BaseSalaries.Any(line => !line.Value.HasValue)) return "[Base Salary] Value cannot be empty.";
            if (request.BaseSalaries.Any(line => !EmployeeAllowanceCalculation.IsValidAmount(line.Value))) return "[Base Salary] Enter a valid non-negative amount with at most 12 integer digits.";
            if (request.BaseSalaries.Any(line => !line.ToValue.HasValue)) return "[Base Salary] To Amount cannot be empty.";
            if (request.BaseSalaries.Any(line => !EmployeeAllowanceCalculation.IsValidAmount(line.ToValue))) return "[Base Salary] To Amount must be a valid non-negative amount with at most 12 integer digits.";
            if (request.BaseSalaries.Any(line => line.ToValue < line.Value)) return "[Base Salary] To Amount cannot be less than From Amount.";
            if (request.BaseSalaries.Any(line => line.IsPercentage)) return "Base Salary must be a fixed amount.";

            foreach (var line in request.Allowances)
            {
                if (line.TypeId is null or <= 0) return "[Allowances] Earning Type cannot be empty.";
                if (line.IsPercentage && !EmployeeAllowanceCalculation.IsValidRate(line.Value)) return "[Allowances] Percentage must be between 0 and 100 with at most 2 decimal places.";
                if (!line.IsPercentage && !EmployeeAllowanceCalculation.IsValidAmount(line.Value)) return "[Allowances] Enter a valid non-negative amount with at most 12 integer digits.";
            }
            foreach (var line in request.Deductions)
            {
                if (line.TypeId is null or <= 0) return "[Deduction] Deduction Type cannot be empty.";
                if (!EmployeeAllowanceCalculation.IsValidAmount(line.Value)) return "[Deduction] Enter a valid non-negative amount with at most 12 integer digits.";
            }
            return null;
        }

        private static async Task<int> NextId<TEntity>(DbSet<TEntity> set) where TEntity : DesignationPayrollAmountDetailLegacy =>
            (await set.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1;

        private Task<bool> IsDesignationInScope(int designationId, int clientId) => _context.Designations.AnyAsync(x =>
            x.Id == designationId && x.ClientId == clientId && !(x.UtBlock ?? false));

        private int GetClientId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == "ClientId");
            if (claim != null && int.TryParse(claim.Value, out var clientId)) return clientId;
            if (Request.Headers.TryGetValue("X-Client-Id", out var header) && int.TryParse(header, out var headerClientId)) return headerClientId;
            throw new InvalidOperationException("ClientId not found in token or headers");
        }
    }
}
