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
    [Route("api/HumanResource/EREmproaster/v3")]
    [Route("api/EREmproaster/v3")]
    public sealed class EmployeeRosterV3Controller : ControllerBase
    {
        private static readonly DateTime MaxDate = new(9998, 12, 31);
        private readonly HumanResourceDbContext _context;

        public EmployeeRosterV3Controller(HumanResourceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<EmployeeRosterV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] int? employeeId = null)
        {
            var clientId = GetClientId();
            var entityId = GetEntityId();

            var query = ScopedRosters(clientId, employeeId.HasValue ? null : entityId);

            if (employeeId.HasValue)
            {
                query = query.Where(x => x.EmployeeId == employeeId.Value);
            }

            var totalCount = await query.CountAsync();
            var entities = await query
                .OrderByDescending(x => x.StartDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            var items = entities.Select(Map).ToList();

            return Ok(new PaginationResponse<EmployeeRosterV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeRosterV3Dto>> GetById(int id)
        {
            var entity = await ScopedRosters(GetClientId(), null)
                .FirstOrDefaultAsync(x => x.Id == id);

            return entity == null ? NotFound() : Ok(Map(entity));
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeRosterV3Dto>> Create([FromBody] UpsertEmployeeRosterV3Request request)
        {
            var clientId = GetClientId();
            var entityId = GetEntityId();

            if (!await IsEmployeeInScope(request.EmployeeId, clientId))
            {
                return BadRequest("Employee does not belong to the current client.");
            }

            var nextId = (await _context.EmployeeRosters.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1;
            var entity = new EmployeeRosterLegacy
            {
                Id = nextId,
                EmployeeId = request.EmployeeId,
                ShiftId = request.ShiftId,
                LeaveGroupId = request.LeaveGroupId,
                LateAllowed = request.LateAllowed,
                EarlyDeparture = request.EarlyDeparture,
                OvertimeAllowed = request.OvertimeAllowed,
                StartDate = request.StartDate?.Date,
                EndDate = request.EndDate ?? MaxDate,
                TaxId = request.TaxId,
                TaxExempt = request.TaxExempt,
                UtBlock = false
            };

            _context.EmployeeRosters.Add(entity);
            if (entity.StartDate.HasValue)
            {
                await ClosePreviousOpenRosterRows(request.EmployeeId, entity.Id, entity.StartDate.Value);
            }
            await _context.SaveChangesAsync();

            var created = await ScopedRosters(clientId, null)
                .FirstAsync(x => x.Id == entity.Id);

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, Map(created));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpsertEmployeeRosterV3Request request)
        {
            if (!request.Id.HasValue)
            {
                return BadRequest("Id is required for update.");
            }

            var entity = await ScopedRosters(GetClientId(), null)
                .FirstOrDefaultAsync(x => x.Id == request.Id.Value);

            if (entity == null)
            {
                return NotFound();
            }

            entity.ShiftId = request.ShiftId;
            entity.LeaveGroupId = request.LeaveGroupId;
            entity.LateAllowed = request.LateAllowed;
            entity.EarlyDeparture = request.EarlyDeparture;
            entity.OvertimeAllowed = request.OvertimeAllowed;
            entity.StartDate = request.StartDate?.Date;
            entity.EndDate = request.EndDate ?? entity.EndDate;
            entity.TaxId = request.TaxId;
            entity.TaxExempt = request.TaxExempt;

            if (entity.StartDate.HasValue && entity.EmployeeId.HasValue)
            {
                await ClosePreviousOpenRosterRows(entity.EmployeeId.Value, entity.Id, entity.StartDate.Value);
            }
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await ScopedRosters(GetClientId(), null)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
            {
                return NotFound();
            }

            if (await HasAttendanceForRoster(entity))
            {
                return BadRequest("Attendance exists against this employee roster. Record cannot be deleted.");
            }

            entity.UtBlock = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("lookups/taxes")]
        public async Task<ActionResult<IEnumerable<LookupItemDto>>> GetTaxLookup()
        {
            var clientId = GetClientId();
            var items = await _context.IncomeTaxes.AsNoTracking()
                .Where(x => (x.ClientId == clientId || x.ClientId == null || x.ClientId == 0) && !x.UtBlock)
                .OrderBy(x => x.Stxt)
                .Select(x => new LookupItemDto(x.Id, x.Code, x.Stxt))
                .ToListAsync();

            return Ok(items);
        }

        private IQueryable<EmployeeRosterLegacy> ScopedRosters(int clientId, int? entityId)
        {
            var query = _context.EmployeeRosters
                .Include(x => x.Employee)
                .Include(x => x.Shift)
                .Include(x => x.LeaveGroup)
                .Include(x => x.Tax)
                .Where(x => x.Employee != null && x.Employee.ClientId == clientId && !(x.UtBlock ?? false));

            if (entityId.HasValue)
            {
                query = query.Where(x => x.Employee != null && x.Employee.EntityId == entityId.Value);
            }

            return query;
        }

        private Task<bool> IsEmployeeInScope(int employeeId, int clientId)
        {
            return _context.EmployeeProfiles.AnyAsync(x =>
                x.Id == employeeId &&
                x.ClientId == clientId &&
                !(x.UtBlock ?? false));
        }

        private async Task ClosePreviousOpenRosterRows(int employeeId, int currentId, DateTime startDate)
        {
            var previousEndDate = startDate.Date.AddDays(-1);
            var rows = await _context.EmployeeRosters
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.Id != currentId &&
                    !(x.UtBlock ?? false) &&
                    x.StartDate.HasValue &&
                    x.StartDate.Value.Date < startDate.Date &&
                    (!x.EndDate.HasValue || x.EndDate.Value.Date >= startDate.Date))
                .ToListAsync();

            foreach (var row in rows)
            {
                row.EndDate = previousEndDate;
            }
        }

        private Task<bool> HasAttendanceForRoster(EmployeeRosterLegacy roster)
        {
            if (!roster.EmployeeId.HasValue || !roster.StartDate.HasValue)
            {
                return Task.FromResult(false);
            }

            var startDate = roster.StartDate.Value.Date;
            var endDate = (roster.EndDate ?? MaxDate).Date;

            return _context.DailyAttendances.AnyAsync(x =>
                x.EmployeeId == roster.EmployeeId.Value &&
                !x.UtBlock &&
                x.AtnDate.HasValue &&
                x.AtnDate.Value.Date >= startDate &&
                x.AtnDate.Value.Date <= endDate);
        }

        private static EmployeeRosterV3Dto Map(EmployeeRosterLegacy entity)
        {
            return new EmployeeRosterV3Dto(
                entity.Id,
                entity.EmployeeId,
                entity.ShiftId,
                entity.Shift?.Stxt,
                entity.LeaveGroupId,
                entity.LeaveGroup?.Stxt,
                entity.LateAllowed ?? false,
                entity.EarlyDeparture ?? false,
                entity.OvertimeAllowed ?? false,
                entity.StartDate,
                entity.EndDate,
                entity.TaxId,
                entity.Tax?.Stxt,
                entity.TaxExempt ?? false);
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
            if (Request.Headers.TryGetValue("X-Campus-Id", out var campusId)
                && int.TryParse(campusId, out var parsedCampusId)) return parsedCampusId;
            if (Request.Headers.TryGetValue("Campus-Id", out var campusIdAlias)
                && int.TryParse(campusIdAlias, out var parsedCampusIdAlias)) return parsedCampusIdAlias;
            var entityIdClaim = User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if (entityIdClaim != null && int.TryParse(entityIdClaim.Value, out var claimEntityId)) return claimEntityId;
            if (Request.Headers.TryGetValue("X-Entity-Id", out var headerEntityId) && int.TryParse(headerEntityId, out var parsedHeaderEntityId)) return parsedHeaderEntityId;
            return null;
        }
    }
}
