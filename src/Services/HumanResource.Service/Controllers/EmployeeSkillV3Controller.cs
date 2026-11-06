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
    [Route("api/HumanResource/EREmpSkill/v3")]
    [Route("api/EREmpSkill/v3")]
    public sealed class EmployeeSkillV3Controller : ControllerBase
    {
        private readonly HumanResourceDbContext _context;

        public EmployeeSkillV3Controller(HumanResourceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<EmployeeSkillV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] int? employeeId = null)
        {
            var query = ScopedSkills(GetClientId(), GetEntityId());
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

            return Ok(new PaginationResponse<EmployeeSkillV3Dto>
            {
                Items = entities.Select(Map).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeSkillV3Dto>> GetById(int id)
        {
            var entity = await ScopedSkills(GetClientId(), GetEntityId())
                .FirstOrDefaultAsync(x => x.Id == id);

            return entity == null ? NotFound() : Ok(Map(entity));
        }

        [HttpGet("lookups/skill-types")]
        public async Task<ActionResult<List<EmployeeSkillLookupV3Dto>>> GetSkillTypes()
        {
            var entityId = GetEntityId();
            var items = await _context.SkillTypes
                .AsNoTracking()
                .Where(x => !(x.UtBlock ?? false) && (!entityId.HasValue || x.EntityId == entityId.Value))
                .OrderBy(x => x.Stxt)
                .Select(x => new EmployeeSkillLookupV3Dto(x.Id, x.Code, x.Stxt ?? string.Empty, null, null))
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("lookups/skills")]
        public async Task<ActionResult<List<EmployeeSkillLookupV3Dto>>> GetSkills([FromQuery] int? skillTypeId = null)
        {
            var entityId = GetEntityId();
            var query = _context.Skills
                .AsNoTracking()
                .Where(x => !(x.UtBlock ?? false) && (!entityId.HasValue || x.EntityId == entityId.Value));

            if (skillTypeId.HasValue)
            {
                query = query.Where(x => x.SkillTypeId == skillTypeId.Value);
            }

            var items = await query
                .OrderBy(x => x.Stxt)
                .Select(x => new EmployeeSkillLookupV3Dto(x.Id, x.Code, x.Stxt ?? string.Empty, x.SkillTypeId, x.SkillRatingId))
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("lookups/ratings")]
        public async Task<ActionResult<List<EmployeeSkillLookupV3Dto>>> GetRatings([FromQuery] int? skillId = null, [FromQuery] int? ratingId = null)
        {
            var selectedRatingId = ratingId;
            if (!selectedRatingId.HasValue && skillId.HasValue)
            {
                selectedRatingId = await _context.Skills
                    .AsNoTracking()
                    .Where(x => x.Id == skillId.Value)
                    .Select(x => x.SkillRatingId)
                    .FirstOrDefaultAsync();
            }

            if (!selectedRatingId.HasValue)
            {
                return Ok(new List<EmployeeSkillLookupV3Dto>());
            }

            var items = await _context.SkillRatingDetails
                .AsNoTracking()
                .Where(x => !(x.UtBlock ?? false) && x.RatingId == selectedRatingId.Value)
                .OrderBy(x => x.LineIndex)
                .ThenBy(x => x.Stxt)
                .Select(x => new EmployeeSkillLookupV3Dto(x.Id, x.Code, x.Stxt ?? string.Empty, null, null))
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("lookups/period-units")]
        public async Task<ActionResult<List<EmployeeSkillLookupV3Dto>>> GetPeriodUnits()
        {
            var items = await _context.Lovs
                .AsNoTracking()
                .Where(x => !x.UtBlock && x.Vtype == "PUNIT")
                .OrderBy(x => x.Stxt)
                .Select(x => new EmployeeSkillLookupV3Dto(x.Id, x.Code, x.Stxt ?? string.Empty, null, null))
                .ToListAsync();

            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeSkillV3Dto>> Create([FromBody] UpsertEmployeeSkillV3Request request)
        {
            var clientId = GetClientId();
            var entityId = GetEntityId();
            var validation = await Validate(request, clientId, entityId);
            if (validation != null) return BadRequest(validation);

            var entity = new EmployeeSkillLegacy
            {
                Id = await NextId(),
                EmployeeId = request.EmployeeId,
                SkillTypeId = request.SkillTypeId,
                SkillId = request.SkillId,
                SkillRatingId = request.SkillRatingId ?? await GetSkillRatingId(request.SkillId),
                SkillRatingDetailId = request.SkillRatingDetailId,
                PeriodUnitId = request.PeriodUnitId,
                Period = request.Period,
                Remarks = Trim(request.Remarks, 500),
                UtSysGen = false,
                UtBlock1 = false,
                UtIsPartial = false,
                UtAddTime = DateTime.UtcNow,
                UtAddUser = GetUserName(),
                UtRecordVersion = 1,
                UtBlock = false
            };

            _context.EmployeeSkills.Add(entity);
            await _context.SaveChangesAsync();

            var created = await ScopedSkills(clientId, entityId).FirstAsync(x => x.Id == entity.Id);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, Map(created));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpsertEmployeeSkillV3Request request)
        {
            if (!request.Id.HasValue) return BadRequest("Id is required for update.");

            var clientId = GetClientId();
            var entityId = GetEntityId();
            var validation = await Validate(request, clientId, entityId);
            if (validation != null) return BadRequest(validation);

            var entity = await ScopedSkills(clientId, entityId).FirstOrDefaultAsync(x => x.Id == request.Id.Value);
            if (entity == null) return NotFound();

            entity.SkillTypeId = request.SkillTypeId;
            entity.SkillId = request.SkillId;
            entity.SkillRatingId = request.SkillRatingId ?? await GetSkillRatingId(request.SkillId);
            entity.SkillRatingDetailId = request.SkillRatingDetailId;
            entity.PeriodUnitId = request.PeriodUnitId;
            entity.Period = request.Period;
            entity.Remarks = Trim(request.Remarks, 500);
            entity.UtEditTime = DateTime.UtcNow;
            entity.UtEditUser = GetUserName();
            entity.UtRecordVersion += 1;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await ScopedSkills(GetClientId(), GetEntityId()).FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return NotFound();

            entity.UtBlock = true;
            entity.UtEditTime = DateTime.UtcNow;
            entity.UtEditUser = GetUserName();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private IQueryable<EmployeeSkillLegacy> ScopedSkills(int clientId, int? entityId)
        {
            var query = _context.EmployeeSkills
                .Include(x => x.Employee)
                .Include(x => x.SkillType)
                .Include(x => x.Skill)
                .Include(x => x.SkillRating)
                .Include(x => x.SkillRatingDetail)
                .Include(x => x.PeriodUnit)
                .Where(x => x.Employee != null && x.Employee.ClientId == clientId && !x.UtBlock && !(x.Employee.UtBlock ?? false));

            if (entityId.HasValue)
            {
                query = query.Where(x => x.Employee != null && x.Employee.EntityId == entityId.Value);
            }

            return query;
        }

        private async Task<string?> Validate(UpsertEmployeeSkillV3Request request, int clientId, int? entityId)
        {
            if (!await IsEmployeeInScope(request.EmployeeId, clientId, entityId))
            {
                return "Employee does not belong to the current client/entity.";
            }

            if (!request.SkillTypeId.HasValue) return "Skill Type is required.";
            if (!request.SkillId.HasValue) return "Skill is required.";
            if (!request.SkillRatingDetailId.HasValue) return "Rating is required.";

            return null;
        }

        private Task<int?> GetSkillRatingId(int? skillId)
        {
            if (!skillId.HasValue) return Task.FromResult<int?>(null);

            return _context.Skills
                .AsNoTracking()
                .Where(x => x.Id == skillId.Value)
                .Select(x => x.SkillRatingId)
                .FirstOrDefaultAsync();
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
            return (await _context.EmployeeSkills.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1;
        }

        private static EmployeeSkillV3Dto Map(EmployeeSkillLegacy entity)
        {
            return new EmployeeSkillV3Dto(
                entity.Id,
                entity.EmployeeId,
                entity.SkillTypeId,
                entity.SkillType?.Stxt,
                entity.SkillId,
                entity.Skill?.Stxt,
                entity.SkillRatingId,
                entity.SkillRating?.Stxt,
                entity.SkillRatingDetailId,
                entity.SkillRatingDetail?.Stxt,
                entity.PeriodUnitId,
                entity.PeriodUnit?.Stxt,
                entity.Period,
                entity.Remarks);
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
