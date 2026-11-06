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
    [Route("api/HumanResource/ERSEmployeesubjectskill/v3")]
    [Route("api/HumanResource/EREmployeesubjectskill/v3")]
    [Route("api/ERSEmployeesubjectskill/v3")]
    [Route("api/EREmployeesubjectskill/v3")]
    [Route("ERSEmployeesubjectskill/v3")]
    [Route("EREmployeesubjectskill/v3")]
    public sealed class EmployeeSubjectRatingV3Controller : ControllerBase
    {
        private readonly HumanResourceDbContext _context;

        public EmployeeSubjectRatingV3Controller(HumanResourceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<EmployeeSubjectRatingV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] int? employeeId = null)
        {
            var query = ScopedSubjectRatings(GetClientId(), GetEntityId());
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

            return Ok(new PaginationResponse<EmployeeSubjectRatingV3Dto>
            {
                Items = entities.Select(Map).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeSubjectRatingV3Dto>> GetById(int id)
        {
            var entity = await ScopedSubjectRatings(GetClientId(), GetEntityId())
                .FirstOrDefaultAsync(x => x.Id == id);

            return entity == null ? NotFound() : Ok(Map(entity));
        }

        [HttpGet("lookups/subject-types")]
        public async Task<ActionResult<List<EmployeeSubjectRatingLookupV3Dto>>> GetSubjectTypes()
        {
            var clientId = GetClientId();
            var items = await _context.SubjectTypes
                .AsNoTracking()
                .Where(x => !(x.UtBlock ?? false) && x.ClientId == clientId)
                .OrderBy(x => x.Stxt)
                .Select(x => new EmployeeSubjectRatingLookupV3Dto(x.Id, x.Code, x.Stxt ?? string.Empty, null, null, null))
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("lookups/ratings")]
        public async Task<ActionResult<List<EmployeeSubjectRatingLookupV3Dto>>> GetRatings()
        {
            var clientId = GetClientId();
            var entityId = GetEntityId();

            var query =
                from detail in _context.SkillRatingDetails.AsNoTracking()
                join header in _context.SkillRatings.AsNoTracking()
                    on detail.RatingId equals header.Id
                where !(detail.UtBlock ?? false)
                    && !(header.UtBlock ?? false)
                    && header.ClientId == clientId
                    && (!entityId.HasValue || header.EntityId == entityId.Value)
                orderby header.Stxt, detail.LineIndex, detail.Stxt
                select new EmployeeSubjectRatingLookupV3Dto(
                    detail.Id,
                    detail.Code,
                    detail.Stxt ?? string.Empty,
                    detail.RatingId,
                    header.Stxt,
                    detail.Factor.HasValue ? (int?)Convert.ToInt32(detail.Factor.Value) : null);

            return Ok(await query.ToListAsync());
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeSubjectRatingV3Dto>> Create([FromBody] UpsertEmployeeSubjectRatingV3Request request)
        {
            var clientId = GetClientId();
            var entityId = GetEntityId();
            var validation = await Validate(request, clientId, entityId);
            if (validation != null) return BadRequest(validation);

            var entity = new EmployeeSubjectRatingLegacy
            {
                Id = await NextId(),
                EmployeeId = request.EmployeeId,
                SubjectTypeId = request.SubjectTypeId,
                RatingDetailId = request.RatingDetailId,
                RatingValue = request.RatingValue ?? await GetRatingValue(request.RatingDetailId),
                UtSysGen = false,
                UtBlock = false,
                UtIsPartial = false,
                UtAddTime = DateTime.UtcNow,
                UtAddUser = GetUserName(),
                UtRecordVersion = 1
            };

            _context.EmployeeSubjectRatings.Add(entity);
            await _context.SaveChangesAsync();

            var created = await ScopedSubjectRatings(clientId, entityId).FirstAsync(x => x.Id == entity.Id);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, Map(created));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpsertEmployeeSubjectRatingV3Request request)
        {
            if (!request.Id.HasValue) return BadRequest("Id is required for update.");

            var clientId = GetClientId();
            var entityId = GetEntityId();
            var validation = await Validate(request, clientId, entityId);
            if (validation != null) return BadRequest(validation);

            var entity = await ScopedSubjectRatings(clientId, entityId).FirstOrDefaultAsync(x => x.Id == request.Id.Value);
            if (entity == null) return NotFound();

            entity.SubjectTypeId = request.SubjectTypeId;
            entity.RatingDetailId = request.RatingDetailId;
            entity.RatingValue = request.RatingValue ?? await GetRatingValue(request.RatingDetailId);
            entity.UtEditTime = DateTime.UtcNow;
            entity.UtEditUser = GetUserName();
            entity.UtRecordVersion += 1;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await ScopedSubjectRatings(GetClientId(), GetEntityId()).FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return NotFound();

            entity.UtBlock = true;
            entity.UtEditTime = DateTime.UtcNow;
            entity.UtEditUser = GetUserName();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private IQueryable<EmployeeSubjectRatingLegacy> ScopedSubjectRatings(int clientId, int? entityId)
        {
            var query = _context.EmployeeSubjectRatings
                .Include(x => x.Employee)
                .Include(x => x.SubjectType)
                .Include(x => x.RatingDetail)
                    .ThenInclude(x => x!.Header)
                .Where(x => x.Employee != null && x.Employee.ClientId == clientId && !x.UtBlock && !(x.Employee.UtBlock ?? false));

            if (entityId.HasValue)
            {
                query = query.Where(x => x.Employee != null && x.Employee.EntityId == entityId.Value);
            }

            return query;
        }

        private async Task<string?> Validate(UpsertEmployeeSubjectRatingV3Request request, int clientId, int? entityId)
        {
            if (!await IsEmployeeInScope(request.EmployeeId, clientId, entityId))
            {
                return "Employee does not belong to the current client/entity.";
            }

            if (!request.SubjectTypeId.HasValue) return "Subject Type is required.";
            if (!request.RatingDetailId.HasValue) return "Rating is required.";

            var duplicateExists = await _context.EmployeeSubjectRatings.AnyAsync(x =>
                x.EmployeeId == request.EmployeeId &&
                x.SubjectTypeId == request.SubjectTypeId &&
                x.Id != (request.Id ?? 0) &&
                !x.UtBlock);

            if (duplicateExists) return "Subject Type already exists.";

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

        private async Task<int?> GetRatingValue(int? ratingDetailId)
        {
            if (!ratingDetailId.HasValue) return null;

            var factor = await _context.SkillRatingDetails
                .AsNoTracking()
                .Where(x => x.Id == ratingDetailId.Value)
                .Select(x => x.Factor)
                .FirstOrDefaultAsync();

            return factor.HasValue ? Convert.ToInt32(factor.Value) : null;
        }

        private async Task<int> NextId()
        {
            return (await _context.EmployeeSubjectRatings.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1;
        }

        private static EmployeeSubjectRatingV3Dto Map(EmployeeSubjectRatingLegacy entity)
        {
            return new EmployeeSubjectRatingV3Dto(
                entity.Id,
                entity.EmployeeId,
                entity.SubjectTypeId,
                entity.SubjectType?.Stxt,
                entity.RatingDetailId,
                entity.RatingDetail?.Stxt,
                entity.RatingDetail?.Header?.Stxt,
                entity.RatingValue);
        }

        private int GetClientId()
        {
            var clientIdClaim = User.Claims.FirstOrDefault(c => c.Type is "ClientId" or "clientId" or "client_id");
            if (clientIdClaim != null && int.TryParse(clientIdClaim.Value, out var claimClientId)) return claimClientId;
            if (Request.Headers.TryGetValue("X-Client-Id", out var headerClientId) && int.TryParse(headerClientId, out var parsedHeaderClientId)) return parsedHeaderClientId;
            throw new InvalidOperationException("ClientId not found in token or headers");
        }

        private int? GetEntityId()
        {
            var entityIdClaim = User.Claims.FirstOrDefault(c => c.Type is "EntityId" or "entityId" or "entity_id");
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
