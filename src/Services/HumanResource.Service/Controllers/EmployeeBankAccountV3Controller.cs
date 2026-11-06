using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/HumanResource/EREmpBank/v3")]
    [Route("api/EREmpBank/v3")]
    public sealed class EmployeeBankAccountV3Controller : ControllerBase
    {
        private readonly HumanResourceDbContext _context;

        public EmployeeBankAccountV3Controller(HumanResourceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<EmployeeBankAccountV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] int? employeeId = null)
        {
            var query = ScopedBankAccounts(GetClientId(), GetEntityId());

            if (employeeId.HasValue)
            {
                query = query.Where(x => x.EmployeeId == employeeId.Value);
            }

            var totalCount = await query.CountAsync();
            var entities = await query
                .OrderByDescending(x => x.PrimaryAccount)
                .ThenBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new PaginationResponse<EmployeeBankAccountV3Dto>
            {
                Items = entities.Select(Map).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeBankAccountV3Dto>> GetById(int id)
        {
            var entity = await ScopedBankAccounts(GetClientId(), GetEntityId())
                .FirstOrDefaultAsync(x => x.Id == id);

            return entity == null ? NotFound() : Ok(Map(entity));
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeBankAccountV3Dto>> Create([FromBody] UpsertEmployeeBankAccountV3Request request)
        {
            var clientId = GetClientId();
            var entityId = GetEntityId();

            var validation = await Validate(request, clientId, entityId);
            if (validation != null)
            {
                return BadRequest(validation);
            }

            if (request.PrimaryAccount)
            {
                await ClearPrimaryAccounts(request.EmployeeId);
            }

            var entity = new EmployeeBankAccountLegacy
            {
                Id = await NextId(),
                Code = request.Code?.Trim(),
                EmployeeId = request.EmployeeId,
                BankGroupId = request.BankGroupId,
                BranchCode = request.BranchCode?.Trim(),
                BranchName = request.BranchName?.Trim(),
                AccountNo = request.AccountNo?.Trim(),
                IbanNo = request.IbanNo?.Trim(),
                Title = request.Title?.Trim(),
                PrimaryAccount = request.PrimaryAccount,
                UtSysGen = false,
                UtBlock1 = false,
                UtIsPartial = false,
                UtAddTime = DateTime.UtcNow,
                UtAddUser = GetUserName(),
                UtRecordVersion = 1,
                UtBlock = false
            };

            _context.EmployeeBankAccounts.Add(entity);
            await _context.SaveChangesAsync();

            var created = await ScopedBankAccounts(clientId, entityId)
                .FirstAsync(x => x.Id == entity.Id);

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, Map(created));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpsertEmployeeBankAccountV3Request request)
        {
            if (!request.Id.HasValue)
            {
                return BadRequest("Id is required for update.");
            }

            var clientId = GetClientId();
            var entityId = GetEntityId();
            var validation = await Validate(request, clientId, entityId);
            if (validation != null)
            {
                return BadRequest(validation);
            }

            var entity = await ScopedBankAccounts(clientId, entityId)
                .FirstOrDefaultAsync(x => x.Id == request.Id.Value);

            if (entity == null)
            {
                return NotFound();
            }

            if (request.PrimaryAccount)
            {
                await ClearPrimaryAccounts(request.EmployeeId, entity.Id);
            }

            entity.Code = request.Code?.Trim();
            entity.BankGroupId = request.BankGroupId;
            entity.BranchCode = request.BranchCode?.Trim();
            entity.BranchName = request.BranchName?.Trim();
            entity.AccountNo = request.AccountNo?.Trim();
            entity.IbanNo = request.IbanNo?.Trim();
            entity.Title = request.Title?.Trim();
            entity.PrimaryAccount = request.PrimaryAccount;
            entity.UtEditTime = DateTime.UtcNow;
            entity.UtEditUser = GetUserName();
            entity.UtRecordVersion += 1;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await ScopedBankAccounts(GetClientId(), GetEntityId())
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return NotFound();
            }

            entity.UtBlock = true;
            entity.UtEditTime = DateTime.UtcNow;
            entity.UtEditUser = GetUserName();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("lookups/bank-groups")]
        public async Task<ActionResult<IEnumerable<LookupItemDto>>> GetBankGroups()
        {
            var clientId = GetClientId();
            var entityId = GetEntityId();

            var query = _context.BankGroups.AsNoTracking()
                .Where(x => !x.UtBlock && (x.ClientId == clientId || x.ClientId == null || x.ClientId == 0));

            if (entityId.HasValue)
            {
                query = query.Where(x => x.EntityId == entityId.Value || x.EntityId == null || x.EntityId == 0);
            }

            var items = await query
                .OrderBy(x => x.Stxt)
                .Select(x => new LookupItemDto(x.Id, x.Code, x.Stxt))
                .ToListAsync();

            return Ok(items);
        }

        private IQueryable<EmployeeBankAccountLegacy> ScopedBankAccounts(int clientId, int? entityId)
        {
            var query = _context.EmployeeBankAccounts
                .Include(x => x.Employee)
                .Include(x => x.BankGroup)
                .Where(x => x.Employee != null && x.Employee.ClientId == clientId && !x.UtBlock);

            if (entityId.HasValue)
            {
                query = query.Where(x => x.Employee != null && x.Employee.EntityId == entityId.Value);
            }

            return query;
        }

        private async Task<string?> Validate(UpsertEmployeeBankAccountV3Request request, int clientId, int? entityId)
        {
            if (!await IsEmployeeInScope(request.EmployeeId, clientId, entityId))
            {
                return "Employee does not belong to the current client/entity.";
            }

            if (!request.BankGroupId.HasValue || request.BankGroupId.Value <= 0)
            {
                return "Bank Group is required.";
            }

            if (!await IsBankGroupInScope(request.BankGroupId.Value, clientId, entityId))
            {
                return "Bank Group is invalid.";
            }

            if (string.IsNullOrWhiteSpace(request.BranchName))
            {
                return "Branch Name is required.";
            }

            if (string.IsNullOrWhiteSpace(request.AccountNo))
            {
                return "Account No is required.";
            }

            if (request.AccountNo.Any(character => !char.IsDigit(character)))
            {
                return "Account No must contain numeric values only.";
            }

            var accountNo = request.AccountNo.Trim();
            var duplicateAccountNo = await _context.EmployeeBankAccounts.AnyAsync(x =>
                x.EmployeeId == request.EmployeeId &&
                !x.UtBlock &&
                x.AccountNo == accountNo &&
                (!request.Id.HasValue || x.Id != request.Id.Value));

            if (duplicateAccountNo)
            {
                return $"Account No already exists: {accountNo}";
            }

            if (!string.IsNullOrWhiteSpace(request.IbanNo))
            {
                var ibanNo = request.IbanNo.Trim();
                var duplicateIbanNo = await _context.EmployeeBankAccounts.AnyAsync(x =>
                    x.EmployeeId == request.EmployeeId &&
                    !x.UtBlock &&
                    x.IbanNo == ibanNo &&
                    (!request.Id.HasValue || x.Id != request.Id.Value));

                if (duplicateIbanNo)
                {
                    return $"IBAN No already exists: {ibanNo}";
                }
            }

            if (string.IsNullOrWhiteSpace(request.Title))
            {
                return "Account Title is required.";
            }

            return null;
        }

        private async Task ClearPrimaryAccounts(int employeeId, int? exceptId = null)
        {
            var query = _context.EmployeeBankAccounts
                .Where(x => x.EmployeeId == employeeId && !x.UtBlock && (x.PrimaryAccount ?? false));

            if (exceptId.HasValue)
            {
                query = query.Where(x => x.Id != exceptId.Value);
            }

            var rows = await query.ToListAsync();
            foreach (var row in rows)
            {
                row.PrimaryAccount = false;
            }
        }

        private Task<bool> IsEmployeeInScope(int employeeId, int clientId, int? entityId)
        {
            return _context.EmployeeProfiles.AnyAsync(x =>
                x.Id == employeeId &&
                x.ClientId == clientId &&
                (!entityId.HasValue || x.EntityId == entityId.Value) &&
                !(x.UtBlock ?? false));
        }

        private Task<bool> IsBankGroupInScope(int bankGroupId, int clientId, int? entityId)
        {
            return _context.BankGroups.AnyAsync(x =>
                x.Id == bankGroupId &&
                !x.UtBlock &&
                (x.ClientId == clientId || x.ClientId == null || x.ClientId == 0) &&
                (!entityId.HasValue || x.EntityId == entityId.Value || x.EntityId == null || x.EntityId == 0));
        }

        private async Task<int> NextId()
        {
            return (await _context.EmployeeBankAccounts.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1;
        }

        private static EmployeeBankAccountV3Dto Map(EmployeeBankAccountLegacy entity)
        {
            return new EmployeeBankAccountV3Dto(
                entity.Id,
                entity.Code,
                entity.EmployeeId,
                entity.BankGroupId,
                entity.BankGroup?.Stxt,
                entity.BranchCode,
                entity.BranchName,
                entity.AccountNo,
                entity.IbanNo,
                entity.Title,
                entity.PrimaryAccount ?? false);
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

        private string? GetUserName()
        {
            return User.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value
                ?? User.Identity?.Name
                ?? Request.Headers["X-User-Name"].FirstOrDefault();
        }
    }
}
