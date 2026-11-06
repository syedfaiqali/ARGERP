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
    [Route("api/HumanResource/ERAcademic/v3")]
    [Route("api/ERAcademic/v3")]
    public sealed class EmployeeEducationV3Controller : ControllerBase
    {
        private readonly HumanResourceDbContext _context;

        public EmployeeEducationV3Controller(HumanResourceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<EmployeeEducationV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] int? employeeId = null)
        {
            var query = ScopedEducations(GetClientId(), GetEntityId());

            if (employeeId.HasValue)
            {
                query = query.Where(x => x.EmployeeId == employeeId.Value);
            }

            var totalCount = await query.CountAsync();
            var entities = await query
                .OrderBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new PaginationResponse<EmployeeEducationV3Dto>
            {
                Items = entities.Select(Map).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeEducationV3Dto>> GetById(int id)
        {
            var entity = await ScopedEducations(GetClientId(), GetEntityId())
                .FirstOrDefaultAsync(x => x.Id == id);

            return entity == null ? NotFound() : Ok(Map(entity));
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeEducationV3Dto>> Create([FromBody] UpsertEmployeeEducationV3Request request)
        {
            var clientId = GetClientId();
            var entityId = GetEntityId();
            var validation = await Validate(request, clientId, entityId);
            if (validation != null) return BadRequest(validation);

            var entity = new EmployeeEducationLegacy
            {
                Id = await NextId(),
                EmployeeId = request.EmployeeId,
                QualificationTypeId = request.QualificationTypeId,
                QualificationId = request.QualificationId,
                Institute = Trim(request.Institute, 50),
                Year = request.PassingYear,
                Cgpa = request.Cgpa,
                Percentage = request.Percentage,
                Grade = Trim(request.Grade, 3),
                Division = Trim(request.Division, 3),
                UtSysGen = false,
                UtIsPartial = false,
                UtAddTime = DateTime.UtcNow,
                UtAddUser = GetUserName(),
                UtRecordVersion = 1,
                UtBlock = false
            };

            _context.EmployeeEducations.Add(entity);
            await _context.SaveChangesAsync();

            var created = await ScopedEducations(clientId, entityId).FirstAsync(x => x.Id == entity.Id);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, Map(created));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpsertEmployeeEducationV3Request request)
        {
            if (!request.Id.HasValue) return BadRequest("Id is required for update.");

            var clientId = GetClientId();
            var entityId = GetEntityId();
            var validation = await Validate(request, clientId, entityId);
            if (validation != null) return BadRequest(validation);

            var entity = await ScopedEducations(clientId, entityId)
                .FirstOrDefaultAsync(x => x.Id == request.Id.Value);

            if (entity == null) return NotFound();

            entity.QualificationTypeId = request.QualificationTypeId;
            entity.QualificationId = request.QualificationId;
            entity.Institute = Trim(request.Institute, 50);
            entity.Year = request.PassingYear;
            entity.Cgpa = request.Cgpa;
            entity.Percentage = request.Percentage;
            entity.Grade = Trim(request.Grade, 3);
            entity.Division = Trim(request.Division, 3);
            entity.UtEditTime = DateTime.UtcNow;
            entity.UtEditUser = GetUserName();
            entity.UtRecordVersion += 1;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await ScopedEducations(GetClientId(), GetEntityId())
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return NotFound();

            entity.UtBlock = true;
            entity.UtEditTime = DateTime.UtcNow;
            entity.UtEditUser = GetUserName();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private IQueryable<EmployeeEducationLegacy> ScopedEducations(int clientId, int? entityId)
        {
            var query = _context.EmployeeEducations
                .Include(x => x.Employee)
                .Include(x => x.QualificationType)
                .Include(x => x.Qualification)
                .Where(x => x.Employee != null && x.Employee.ClientId == clientId && !x.UtBlock && !(x.Employee.UtBlock ?? false));

            if (entityId.HasValue)
            {
                query = query.Where(x => x.Employee != null && x.Employee.EntityId == entityId.Value);
            }

            return query;
        }

        private async Task<string?> Validate(UpsertEmployeeEducationV3Request request, int clientId, int? entityId)
        {
            if (!await IsEmployeeInScope(request.EmployeeId, clientId, entityId))
            {
                return "Employee does not belong to the current client/entity.";
            }

            if (!request.QualificationTypeId.HasValue) return "Qualification Type is required.";
            if (!request.QualificationId.HasValue) return "Qualification is required.";
            if (string.IsNullOrWhiteSpace(request.Institute)) return "Institute is required.";
            if (!request.PassingYear.HasValue) return "Passing Year is required.";
            if (request.Cgpa.HasValue && request.Cgpa.Value > 100) return "CGPA cannot be greater than 100.";
            if (request.Percentage.HasValue && request.Percentage.Value > 100) return "Percentage cannot be greater than 100.";

            return null;
        }

        private Task<bool> IsEmployeeInScope(int employeeId, int clientId, int? entityId)
        {
            return _context.EmployeeProfiles.AnyAsync(x =>
                x.Id == employeeId &&
                x.ClientId == clientId &&
                (!entityId.HasValue || x.EntityId == entityId.Value) &&
                !(x.UtBlock ?? false));
        }

        private async Task<int> NextId()
        {
            return (await _context.EmployeeEducations.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1;
        }

        private static EmployeeEducationV3Dto Map(EmployeeEducationLegacy entity)
        {
            return new EmployeeEducationV3Dto(
                entity.Id,
                entity.EmployeeId,
                entity.QualificationTypeId,
                entity.QualificationType?.Stxt,
                entity.QualificationId,
                entity.Qualification?.stxt,
                entity.Institute,
                entity.Cgpa,
                entity.Percentage,
                entity.Grade,
                entity.Division,
                entity.Year);
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

        private string? GetUserName()
        {
            return User.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value
                ?? User.Identity?.Name
                ?? Request.Headers["X-User-Name"].FirstOrDefault();
        }
    }
}
