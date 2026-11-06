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
    [Route("api/HumanResource/EREmppayroll/v3")]
    [Route("api/EREmppayroll/v3")]
    public sealed class EmployeePayrollV3Controller : ControllerBase
    {
        private readonly HumanResourceDbContext _context;

        public EmployeePayrollV3Controller(HumanResourceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<EmployeePayrollV3Dto>> Get([FromQuery] int employeeId)
        {
            if (employeeId <= 0)
            {
                return BadRequest("employeeId is required.");
            }

            if (!await IsEmployeeInScope(employeeId, GetClientId(), GetEntityId()))
            {
                return NotFound();
            }

            var payroll = await PayrollQuery()
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId && !x.UtBlock);

            return Ok(payroll == null ? new EmployeePayrollV3Dto { EmployeeId = employeeId } : Map(payroll));
        }

        [HttpPost]
        public async Task<ActionResult<EmployeePayrollV3Dto>> Save([FromBody] EmployeePayrollV3Dto request)
        {
            var clientId = GetClientId();
            var entityId = GetEntityId();

            if (request.EmployeeId <= 0)
            {
                return BadRequest("Employee is required.");
            }

            if (!await IsEmployeeInScope(request.EmployeeId, clientId, entityId))
            {
                return BadRequest("Employee does not belong to the current client/entity.");
            }

            // Employee payroll stores one actual salary amount. The legacy table still
            // has a ToValue column, so persist the same amount in it rather than
            // exposing a salary range on the employee profile.
            foreach (var baseSalary in request.BaseSalaries)
            {
                baseSalary.ToValue = baseSalary.Value;
            }

            var validationError = Validate(request);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            var designationRangeError = await ValidateDesignationSalaryRange(request, clientId, entityId);
            if (designationRangeError != null)
            {
                return BadRequest(designationRangeError);
            }

            var loanTypeIds = request.LoanTypeIds.Distinct().Where(id => id > 0).ToList();
            if (loanTypeIds.Count != request.LoanTypeIds.Count)
            {
                return BadRequest("Select valid Loan Types.");
            }
            if (loanTypeIds.Count > 0)
            {
                var validLoanTypeCount = await _context.LoanTypes
                    .CountAsync(x => loanTypeIds.Contains(x.Id) && x.clientid == clientId && !x.utblock);
                if (validLoanTypeCount != loanTypeIds.Count)
                {
                    return BadRequest("One or more selected Loan Types are inactive or unavailable.");
                }
            }

            var payroll = await _context.EmployeePayrolls
                .Include(x => x.BaseSalaries)
                .Include(x => x.Allowances)
                .Include(x => x.Deductions)
                .Include(x => x.LoanTypes)
                .FirstOrDefaultAsync(x => x.EmployeeId == request.EmployeeId && !x.UtBlock);

            if (payroll == null)
            {
                payroll = new EmployeePayrollLegacy
                {
                    Id = (await _context.EmployeePayrolls.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1,
                    EmployeeId = request.EmployeeId,
                    UtBlock = false,
                    UtRecordVersion = 1
                };
                _context.EmployeePayrolls.Add(payroll);
            }

            payroll.IsLoanAllowed = request.LoanAllowed;
            payroll.IsAdvanceAllowed = request.AdvanceAllowed;
            payroll.PayFullSalary = request.PayFullSalary;
            payroll.IsOvertimeAllowed = request.OvertimeAllowed;
            payroll.UtRecordVersion = payroll.UtRecordVersion <= 0 ? 1 : payroll.UtRecordVersion + 1;

            _context.EmployeePayrollBaseSalaries.RemoveRange(payroll.BaseSalaries);
            _context.EmployeePayrollAllowances.RemoveRange(payroll.Allowances);
            _context.EmployeePayrollDeductions.RemoveRange(payroll.Deductions);
            _context.EmployeePayrollLoanTypes.RemoveRange(payroll.LoanTypes);

            var nextBaseId = await NextBaseSalaryId();
            var nextAllowanceId = await NextId(_context.EmployeePayrollAllowances);
            var nextDeductionId = await NextId(_context.EmployeePayrollDeductions);
            var nextLoanTypeId = (await _context.EmployeePayrollLoanTypes.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1;

            var baseSalaries = request.BaseSalaries.Select((line, index) => new EmployeePayrollBaseSalaryLegacy
            {
                Id = nextBaseId++,
                PayrollId = payroll.Id,
                LineIndex = index + 1,
                EarningTypeId = line.TypeId,
                StartDate = line.StartDate,
                EndDate = line.EndDate,
                Value = line.Value,
                ToValue = line.ToValue,
                UtBlock = false,
                UtRecordVersion = 1
            }).ToList();

            var allowances = request.Allowances.Select((line, index) => new EmployeePayrollAllowanceLegacy
            {
                Id = nextAllowanceId++,
                PayrollId = payroll.Id,
                LineIndex = index + 1,
                EarningTypeId = line.TypeId,
                StartDate = line.StartDate,
                EndDate = line.EndDate,
                // The checkbox determines whether Value stores a rate or fixed money.
                Value = line.Value,
                IsPercentage = line.IsPercentage,
                UtBlock = false,
                UtRecordVersion = 1
            }).ToList();

            var deductions = request.Deductions.Select((line, index) => new EmployeePayrollDeductionLegacy
            {
                Id = nextDeductionId++,
                PayrollId = payroll.Id,
                LineIndex = index + 1,
                DeductionTypeId = line.TypeId,
                StartDate = line.StartDate,
                EndDate = line.EndDate,
                Value = line.Value,
                UtBlock = false,
                UtRecordVersion = 1
            }).ToList();

            var loanTypes = loanTypeIds.Select(loanTypeId => new EmployeePayrollLoanTypeLegacy
                {
                    Id = nextLoanTypeId++,
                    PayrollId = payroll.Id,
                    LoanTypeId = loanTypeId,
                    UtBlock = false,
                    UtRecordVersion = 1
                })
                .ToList();

            payroll.BaseSalaries = baseSalaries;
            payroll.Allowances = allowances;
            payroll.Deductions = deductions;
            payroll.LoanTypes = loanTypes;
            _context.EmployeePayrollBaseSalaries.AddRange(baseSalaries);
            _context.EmployeePayrollAllowances.AddRange(allowances);
            _context.EmployeePayrollDeductions.AddRange(deductions);
            _context.EmployeePayrollLoanTypes.AddRange(loanTypes);

            await _context.SaveChangesAsync();

            var saved = await PayrollQuery().FirstAsync(x => x.Id == payroll.Id);
            return Ok(Map(saved));
        }

        [HttpGet("lookups/earning-types")]
        public async Task<ActionResult<IEnumerable<PayrollLookupItemDto>>> GetEarningTypes([FromQuery] bool? baseSalary = null)
        {
            var clientId = GetClientId();
            var query = _context.EarningTypes.AsNoTracking()
                .Where(x => (x.ClientId == clientId || x.ClientId == null || x.ClientId == 0) && !x.UtBlock);

            if (baseSalary.HasValue)
            {
                query = baseSalary.Value
                    ? query.Where(x => x.BsSalary == true)
                    : query.Where(x => x.BsSalary != true);
            }

            var items = await query
                .OrderBy(x => x.Stxt)
                .Select(x => new PayrollLookupItemDto(x.Id, x.Code, x.Stxt))
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("lookups/deduction-types")]
        public async Task<ActionResult<IEnumerable<PayrollLookupItemDto>>> GetDeductionTypes()
        {
            var clientId = GetClientId();
            var items = await _context.DeductionTypes.AsNoTracking()
                .Where(x => (x.ClientId == clientId || x.ClientId == null || x.ClientId == 0) && !x.UtBlock)
                .OrderBy(x => x.Stxt)
                .Select(x => new PayrollLookupItemDto(x.Id, x.Code, x.Stxt))
                .ToListAsync();

            return Ok(items);
        }

        private IQueryable<EmployeePayrollLegacy> PayrollQuery()
        {
            return _context.EmployeePayrolls
                .Include(x => x.BaseSalaries).ThenInclude(x => x.EarningType)
                .Include(x => x.Allowances).ThenInclude(x => x.EarningType)
                .Include(x => x.Deductions).ThenInclude(x => x.DeductionType)
                .Include(x => x.LoanTypes).ThenInclude(x => x.LoanType);
        }

        private static EmployeePayrollV3Dto Map(EmployeePayrollLegacy payroll)
        {
            return new EmployeePayrollV3Dto
            {
                Id = payroll.Id,
                EmployeeId = payroll.EmployeeId ?? 0,
                LoanAllowed = payroll.IsLoanAllowed ?? false,
                AdvanceAllowed = payroll.IsAdvanceAllowed ?? false,
                PayFullSalary = payroll.PayFullSalary,
                OvertimeAllowed = payroll.IsOvertimeAllowed ?? false,
                LoanTypeIds = payroll.LoanTypes
                    .Where(x => !x.UtBlock && x.LoanTypeId.HasValue)
                    .OrderBy(x => x.LoanType?.stxt)
                    .Select(x => x.LoanTypeId!.Value)
                    .ToList(),
                BaseSalaries = payroll.BaseSalaries
                    .Where(x => !x.UtBlock)
                    .OrderBy(x => x.LineIndex)
                    .Select(x => new EmployeePayrollEarningLineV3Dto
                    {
                        Id = checked((int)x.Id),
                        TypeId = x.EarningTypeId,
                        TypeName = x.EarningType?.Stxt,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        Value = x.Value,
                        ToValue = x.ToValue
                    })
                    .ToList(),
                Allowances = payroll.Allowances
                    .Where(x => !x.UtBlock)
                    .OrderBy(x => x.LineIndex)
                    .Select(x => new EmployeePayrollEarningLineV3Dto
                    {
                        Id = x.Id,
                        TypeId = x.EarningTypeId,
                        TypeName = x.EarningType?.Stxt,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        Value = x.Value,
                        IsPercentage = x.IsPercentage
                    })
                    .ToList(),
                Deductions = payroll.Deductions
                    .Where(x => !x.UtBlock)
                    .OrderBy(x => x.LineIndex)
                    .Select(x => new EmployeePayrollDeductionLineV3Dto
                    {
                        Id = x.Id,
                        TypeId = x.DeductionTypeId,
                        TypeName = x.DeductionType?.Stxt,
                        StartDate = x.StartDate,
                        EndDate = x.EndDate,
                        Value = x.Value
                    })
                    .ToList()
            };
        }

        private static string? Validate(EmployeePayrollV3Dto request)
        {
            var loanOnlySetup = request.LoanTypeIds.Any(id => id > 0)
                && request.BaseSalaries.Count == 0
                && request.Allowances.Count == 0
                && request.Deductions.Count == 0;
            if (loanOnlySetup)
            {
                return null;
            }

            if (request.BaseSalaries.Count == 0)
            {
                return "Base Salary cannot be empty.";
            }

            if (request.BaseSalaries.Count > 1)
            {
                return "Only one Base Salary row is allowed.";
            }

            // Base Salary earning type is optional; only the amount is required.
            if (request.BaseSalaries.Any(line => !line.Value.HasValue))
            {
                return "[Base Salary] Value cannot be empty.";
            }

            if (request.BaseSalaries.Any(line => !EmployeeAllowanceCalculation.IsValidAmount(line.Value)))
                return "[Base Salary] Enter a valid non-negative amount with at most 12 integer digits.";

            if (request.BaseSalaries.Any(line => !line.ToValue.HasValue))
                return "[Base Salary] To Amount cannot be empty.";

            if (request.BaseSalaries.Any(line => !EmployeeAllowanceCalculation.IsValidAmount(line.ToValue)))
                return "[Base Salary] To Amount must be a valid non-negative amount with at most 12 integer digits.";

            if (request.BaseSalaries.Any(line => line.ToValue < line.Value))
                return "[Base Salary] To Amount cannot be less than From Amount.";

            if (request.BaseSalaries.Any(line => line.IsPercentage))
                return "Base Salary must be a fixed amount.";

            foreach (var line in request.Allowances)
            {
                if (line.TypeId is null or <= 0) return "[Allowances] Earning Type cannot be empty.";
                if (line.IsPercentage)
                {
                    if (!EmployeeAllowanceCalculation.IsValidRate(line.Value))
                        return "[Allowances] Percentage must be between 0 and 100 with at most 2 decimal places.";
                }
                else if (!EmployeeAllowanceCalculation.IsValidAmount(line.Value))
                    return "[Allowances] Enter a valid non-negative amount with at most 12 integer digits.";
            }

            foreach (var line in request.Deductions)
            {
                var error = ValidateAmountLine(line.TypeId, line.Value, "Deduction", "Deduction Type");
                if (error != null) return error;
            }

            return null;
        }

        private async Task<string?> ValidateDesignationSalaryRange(
            EmployeePayrollV3Dto request,
            int clientId,
            int? entityId)
        {
            if (request.BaseSalaries.Count == 0)
            {
                return null;
            }

            var designationId = await _context.EmployeeProfiles
                .AsNoTracking()
                .Where(x =>
                    x.Id == request.EmployeeId &&
                    x.ClientId == clientId &&
                    (!entityId.HasValue || x.EntityId == entityId.Value) &&
                    !(x.UtBlock ?? false))
                .Select(x => x.DesignationId)
                .FirstOrDefaultAsync();

            if (!designationId.HasValue || designationId.Value <= 0)
            {
                return null;
            }

            var range = await _context.DesignationPayrollBaseSalaries
                .AsNoTracking()
                .Where(x =>
                    x.Payroll != null &&
                    x.Payroll.DesignationId == designationId.Value &&
                    x.Payroll.ClientId == clientId &&
                    !x.Payroll.UtBlock &&
                    !x.UtBlock)
                .OrderBy(x => x.LineIndex)
                .Select(x => new { x.Value, x.ToValue })
                .FirstOrDefaultAsync();

            if (range?.Value is not double minimumSalary || range.ToValue is not double maximumSalary)
            {
                return null;
            }

            var employeeSalary = request.BaseSalaries[0].Value;
            if (!employeeSalary.HasValue || employeeSalary.Value < minimumSalary || employeeSalary.Value > maximumSalary)
            {
                return $"Base Salary must be between {minimumSalary:N2} and {maximumSalary:N2} for the selected designation.";
            }

            return null;
        }

        private static string? ValidateAmountLine(int? typeId, double? value, string section, string typeLabel)
        {
            if (!typeId.HasValue) return $"[{section}] {typeLabel} cannot be empty.";
            if (!value.HasValue) return $"[{section}] Value cannot be empty.";
            return null;
        }

        private static async Task<int> NextId<TEntity>(DbSet<TEntity> set) where TEntity : EmployeePayrollAmountDetailLegacy
        {
            return (await set.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1;
        }

        private async Task<long> NextBaseSalaryId()
        {
            return (await _context.EmployeePayrollBaseSalaries.IgnoreQueryFilters().MaxAsync(x => (long?)x.Id) ?? 0L) + 1L;
        }

        private Task<bool> IsEmployeeInScope(int employeeId, int clientId, int? entityId)
        {
            return _context.EmployeeProfiles.AnyAsync(x =>
                x.Id == employeeId &&
                x.ClientId == clientId &&
                (!entityId.HasValue || x.EntityId == entityId.Value) &&
                !(x.UtBlock ?? false));
        }

        private int GetClientId()
        {
            var clientIdClaim = User.Claims.FirstOrDefault(c => c.Type == "ClientId");
            if (clientIdClaim != null && int.TryParse(clientIdClaim.Value, out var claimClientId)) return claimClientId;
            if (Request.Headers.TryGetValue("X-Client-Id", out var headerClientId) && int.TryParse(headerClientId, out var parsedHeaderClientId)) return parsedHeaderClientId;
            throw new InvalidOperationException("ClientId not found in token or headers");
        }

        private int? GetEntityId()
        {
            var entityIdClaim = User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if (entityIdClaim != null && int.TryParse(entityIdClaim.Value, out var claimEntityId)) return claimEntityId;
            if (Request.Headers.TryGetValue("X-Entity-Id", out var headerEntityId) && int.TryParse(headerEntityId, out var parsedHeaderEntityId)) return parsedHeaderEntityId;
            return null;
        }
    }
}
