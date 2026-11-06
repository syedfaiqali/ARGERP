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
    [Route("api/EREmpPosition/v3")]
    [Route("api/HumanResource/EREmpPosition/v3")]
    public sealed class EREmpPositionV3Controller : ControllerBase
    {
        private readonly HumanResourceDbContext _context;

        public EREmpPositionV3Controller(HumanResourceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<EREmpPositionV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] int? employeeId = null
        )
        {
            var clientId = GetClientId();
            var headerEntityId = GetEntityId();

            var query = _context.EmployeePositions.AsNoTracking()
                .Include(x => x.Employee)
                .Include(x => x.Department)
                .Include(x => x.Designation)
                .Where(x => x.Employee != null && x.Employee.ClientId == clientId && !(x.UtBlock ?? false));

            if (headerEntityId.HasValue)
            {
                query = query.Where(x => x.Employee != null && x.Employee.EntityId == headerEntityId.Value);
            }

            if (employeeId.HasValue)
            {
                query = query.Where(x => x.EmployeeId == employeeId.Value);
            }

            var totalCount = await query.CountAsync();
            var items = await query.OrderByDescending(x => x.FromDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new EREmpPositionV3Dto(
                    x.Id,
                    x.EmployeeId,
                    x.DepartmentId,
                    x.Department != null ? x.Department.DepStxt : null,
                    x.DesignationId,
                    x.Designation != null ? x.Designation.Stxt : null,
                    x.GradeId,
                    _context.EmployeeGrades.Where(g => g.Id == x.GradeId && !(g.UtBlock)).Select(g => g.Code).FirstOrDefault(),
                    x.ActionId,
                    _context.Lovs.Where(l => l.Id == x.ActionId && l.Vtype == "EMACT" && !l.UtBlock).Select(l => l.Stxt ?? l.Code).FirstOrDefault(),
                    x.Remarks,
                    x.FromDate,
                    x.TillDate,
                    x.Responsibility
                ))
                .ToListAsync();

            return Ok(new PaginationResponse<EREmpPositionV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EREmpPositionV3Dto>> GetById(int id)
        {
            var clientId = GetClientId();
            var headerEntityId = GetEntityId();

            var entity = await _context.EmployeePositions.AsNoTracking()
                .Include(x => x.Employee)
                .Include(x => x.Department)
                .Include(x => x.Designation)
                .FirstOrDefaultAsync(x => x.Id == id && x.Employee != null && x.Employee.ClientId == clientId && (!headerEntityId.HasValue || x.Employee.EntityId == headerEntityId.Value) && !(x.UtBlock ?? false));

            if (entity == null) return NotFound();

            return Ok(new EREmpPositionV3Dto(
                entity.Id,
                entity.EmployeeId,
                entity.DepartmentId,
                entity.Department != null ? entity.Department.DepStxt : null,
                entity.DesignationId,
                entity.Designation != null ? entity.Designation.Stxt : null,
                entity.GradeId,
                await _context.EmployeeGrades.Where(g => g.Id == entity.GradeId && !(g.UtBlock)).Select(g => g.Code).FirstOrDefaultAsync(),
                entity.ActionId,
                await _context.Lovs.Where(l => l.Id == entity.ActionId && l.Vtype == "EMACT" && !l.UtBlock).Select(l => l.Stxt ?? l.Code).FirstOrDefaultAsync(),
                entity.Remarks,
                entity.FromDate,
                entity.TillDate,
                entity.Responsibility
            ));
        }

        [HttpPost]
        public async Task<ActionResult<EREmpPositionV3Dto>> Create([FromBody] UpsertEREmpPositionV3Request request)
        {
            var clientId = GetClientId();
            var entityId = GetEntityId() ?? 1;

            if (!await IsEmployeeInScope(request.EmployeeId, clientId, entityId))
            {
                return BadRequest("Employee does not belong to the current client/entity.");
            }

            if (request.ActionId.HasValue && !await IsEmploymentAction(request.ActionId.Value))
            {
                return BadRequest("Selected Employment Action is not valid.");
            }

            var nextId = (await _context.EmployeePositions.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1;

            var entity = new EREmpPositionLegacy
            {
                Id = nextId,
                EmployeeId = request.EmployeeId,
                DepartmentId = request.DepartmentId,
                DesignationId = request.DesignationId,
                GradeId = request.GradeId,
                ActionId = request.ActionId,
                Remarks = Trim(request.Remarks, 1500),
                FromDate = request.FromDate.Date,
                TillDate = request.TillDate ?? new DateTime(9998, 12, 31),
                Responsibility = Trim(request.Responsibility, 1500),
                UtBlock = false,
                UtRecordVersion = 1
            };

            _context.EmployeePositions.Add(entity);
            await ClosePreviousOpenPositionRows(request.EmployeeId, entity.Id, entity.FromDate.Value);
            await _context.SaveChangesAsync();
            await SyncCurrentEmployeePosition(request.EmployeeId);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, new EREmpPositionV3Dto(
                entity.Id,
                entity.EmployeeId,
                entity.DepartmentId,
                null,
                entity.DesignationId,
                null,
                entity.GradeId,
                null,
                entity.ActionId,
                null,
                entity.Remarks,
                entity.FromDate,
                entity.TillDate,
                entity.Responsibility
            ));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpsertEREmpPositionV3Request request)
        {
            if (!request.Id.HasValue) return BadRequest("Id is required for update.");

            var clientId = GetClientId();
            var entityId = GetEntityId();
            var entity = await _context.EmployeePositions
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.Id == request.Id.Value && x.Employee != null && x.Employee.ClientId == clientId && (!entityId.HasValue || x.Employee.EntityId == entityId.Value) && !(x.UtBlock ?? false));

            if (entity == null) return NotFound();

            if (request.ActionId.HasValue && !await IsEmploymentAction(request.ActionId.Value))
            {
                return BadRequest("Selected Employment Action is not valid.");
            }

            entity.DepartmentId = request.DepartmentId;
            entity.DesignationId = request.DesignationId;
            entity.GradeId = request.GradeId;
            entity.ActionId = request.ActionId;
            entity.Remarks = Trim(request.Remarks, 1500);
            entity.FromDate = request.FromDate.Date;
            entity.TillDate = request.TillDate ?? entity.TillDate;
            entity.Responsibility = Trim(request.Responsibility, 1500);
            entity.UtEditTime = DateTime.UtcNow;
            entity.UtRecordVersion++;

            await ClosePreviousOpenPositionRows(request.EmployeeId, entity.Id, entity.FromDate.Value);
            await _context.SaveChangesAsync();
            await SyncCurrentEmployeePosition(request.EmployeeId);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var clientId = GetClientId();
            var entityId = GetEntityId();
            var entity = await _context.EmployeePositions
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.Id == id && x.Employee != null && x.Employee.ClientId == clientId && (!entityId.HasValue || x.Employee.EntityId == entityId.Value) && !(x.UtBlock ?? false));

            if (entity == null) return NotFound();

            if (await HasAttendanceForPosition(entity))
            {
                return BadRequest("Attendance exists against this position assignment. Record cannot be deleted.");
            }

            entity.UtBlock = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private Task<bool> IsEmployeeInScope(int employeeId, int clientId, int? entityId)
        {
            return _context.EmployeeProfiles.AnyAsync(x =>
                x.Id == employeeId &&
                x.ClientId == clientId &&
                (!entityId.HasValue || x.EntityId == entityId.Value) &&
                !(x.UtBlock ?? false));
        }

        private Task<bool> IsEmploymentAction(int actionId) =>
            _context.Lovs.AnyAsync(x => x.Id == actionId && x.Vtype == "EMACT" && !x.UtBlock);

        private async Task ClosePreviousOpenPositionRows(int employeeId, int currentId, DateTime fromDate)
        {
            var previousTillDate = fromDate.Date.AddDays(-1);
            var rows = await _context.EmployeePositions
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    x.Id != currentId &&
                    !(x.UtBlock ?? false) &&
                    x.FromDate.HasValue &&
                    x.FromDate.Value.Date < fromDate.Date &&
                    (!x.TillDate.HasValue || x.TillDate.Value.Date >= fromDate.Date))
                .ToListAsync();

            foreach (var row in rows)
            {
                row.TillDate = previousTillDate;
                row.UtEditTime = DateTime.UtcNow;
                row.UtRecordVersion = row.UtRecordVersion <= 0 ? 1 : row.UtRecordVersion + 1;
            }
        }

        // The employee profile supplies the summary card, while position rows retain
        // the full employment history. Keep the profile fields aligned with the
        // position that is currently effective.
        private async Task SyncCurrentEmployeePosition(int employeeId)
        {
            var today = DateTime.UtcNow.Date;
            var currentPosition = await _context.EmployeePositions
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    !(x.UtBlock ?? false) &&
                    x.FromDate.HasValue &&
                    x.FromDate.Value.Date <= today &&
                    (!x.TillDate.HasValue || x.TillDate.Value.Date >= today))
                .OrderByDescending(x => x.FromDate)
                .ThenByDescending(x => x.Id)
                .FirstOrDefaultAsync();

            var employee = await _context.EmployeeProfiles
                .FirstOrDefaultAsync(x => x.Id == employeeId && !(x.UtBlock ?? false));

            if (employee == null || currentPosition == null) return;

            employee.DepartmentId = currentPosition.DepartmentId;
            employee.DesignationId = currentPosition.DesignationId;
            employee.GradeId = currentPosition.GradeId;
            employee.UtEditTime = DateTime.UtcNow;
            employee.UtRecordVersion = employee.UtRecordVersion <= 0
                ? 1
                : employee.UtRecordVersion + 1;
        }

        private Task<bool> HasAttendanceForPosition(EREmpPositionLegacy position)
        {
            if (!position.EmployeeId.HasValue || !position.FromDate.HasValue)
            {
                return Task.FromResult(false);
            }

            var fromDate = position.FromDate.Value.Date;
            var tillDate = (position.TillDate ?? new DateTime(9998, 12, 31)).Date;

            return _context.DailyAttendances.AnyAsync(x =>
                x.EmployeeId == position.EmployeeId.Value &&
                !(x.UtBlock) &&
                x.AtnDate.HasValue &&
                x.AtnDate.Value.Date >= fromDate &&
                x.AtnDate.Value.Date <= tillDate);
        }

        private static string? Trim(string? value, int maxLength)
        {
            var trimmed = value?.Trim();
            if (string.IsNullOrEmpty(trimmed)) return null;
            return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength];
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
