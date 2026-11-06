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
    [Route("api/HumanResource/EREmpCards/v3")]
    [Route("api/EREmpCards/v3")]
    [Route("EREmpCards/v3")]
    public sealed class EmployeeCardV3Controller : ControllerBase
    {
        private readonly HumanResourceDbContext _context;

        public EmployeeCardV3Controller(HumanResourceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<EmployeeCardV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] int? employeeId = null)
        {
            var query = ScopedCards(GetClientId(), GetEntityId());
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

            return Ok(new PaginationResponse<EmployeeCardV3Dto>
            {
                Items = entities.Select(Map).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeCardV3Dto>> GetById(int id)
        {
            var entity = await ScopedCards(GetClientId(), GetEntityId())
                .FirstOrDefaultAsync(x => x.Id == id);

            return entity == null ? NotFound() : Ok(Map(entity));
        }

        [HttpGet("lookups/card-types")]
        public Task<List<EmployeeCardLookupV3Dto>> GetCardTypes()
        {
            return GetLovLookup("CRDTP");
        }

        [HttpGet("lookups/authorities")]
        public Task<List<EmployeeCardLookupV3Dto>> GetAuthorities()
        {
            return GetLovLookup("AUTHR");
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeCardV3Dto>> Create([FromBody] UpsertEmployeeCardV3Request request)
        {
            var clientId = GetClientId();
            var entityId = GetEntityId();
            var validation = await Validate(request, clientId, entityId);
            if (validation != null) return BadRequest(validation);

            var entity = new EmployeeCardLegacy
            {
                Id = await NextId(),
                Code = Trim(request.Code, 20),
                EmployeeId = request.EmployeeId,
                CardTypeId = request.CardTypeId,
                AuthorityId = request.AuthorityId,
                CardNo = Trim(request.CardNo, 25),
                ValidFrom = request.ValidFrom,
                ValidTill = request.ValidTill,
                UtSysGen = false,
                UtBlock1 = false,
                UtBlock = false,
                UtIsPartial = false,
                UtAddTime = DateTime.UtcNow,
                UtAddUser = GetUserName(),
                UtRecordVersion = 1
            };

            _context.EmployeeCards.Add(entity);
            await _context.SaveChangesAsync();

            var created = await ScopedCards(clientId, entityId).FirstAsync(x => x.Id == entity.Id);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, Map(created));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpsertEmployeeCardV3Request request)
        {
            if (!request.Id.HasValue) return BadRequest("Id is required for update.");

            var clientId = GetClientId();
            var entityId = GetEntityId();
            var validation = await Validate(request, clientId, entityId);
            if (validation != null) return BadRequest(validation);

            var entity = await ScopedCards(clientId, entityId).FirstOrDefaultAsync(x => x.Id == request.Id.Value);
            if (entity == null) return NotFound();

            entity.Code = Trim(request.Code, 20);
            entity.CardTypeId = request.CardTypeId;
            entity.AuthorityId = request.AuthorityId;
            entity.CardNo = Trim(request.CardNo, 25);
            entity.ValidFrom = request.ValidFrom;
            entity.ValidTill = request.ValidTill;
            entity.UtEditTime = DateTime.UtcNow;
            entity.UtEditUser = GetUserName();
            entity.UtRecordVersion += 1;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await ScopedCards(GetClientId(), GetEntityId()).FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return NotFound();

            entity.UtBlock = true;
            entity.UtEditTime = DateTime.UtcNow;
            entity.UtEditUser = GetUserName();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private IQueryable<EmployeeCardLegacy> ScopedCards(int clientId, int? entityId)
        {
            var query = _context.EmployeeCards
                .Include(x => x.Employee)
                .Include(x => x.CardType)
                .Include(x => x.Authority)
                .Where(x => x.Employee != null && x.Employee.ClientId == clientId && !x.UtBlock && !(x.Employee.UtBlock ?? false));

            if (entityId.HasValue)
            {
                query = query.Where(x => x.Employee != null && x.Employee.EntityId == entityId.Value);
            }

            return query;
        }

        private async Task<string?> Validate(UpsertEmployeeCardV3Request request, int clientId, int? entityId)
        {
            if (!await IsEmployeeInScope(request.EmployeeId, clientId, entityId))
            {
                return "Employee does not belong to the current client/entity.";
            }

            if (!request.CardTypeId.HasValue) return "Card Type is required.";
            if (string.IsNullOrWhiteSpace(request.CardNo)) return "Card No is required.";
            if (!request.ValidFrom.HasValue) return "Valid From is required.";
            if (!request.ValidTill.HasValue) return "Valid Till is required.";

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

        private Task<List<EmployeeCardLookupV3Dto>> GetLovLookup(string vtype)
        {
            return _context.Lovs
                .AsNoTracking()
                .Where(x => !x.UtBlock && x.Vtype == vtype)
                .OrderBy(x => x.Stxt)
                .Select(x => new EmployeeCardLookupV3Dto(x.Id, x.Code, x.Stxt ?? string.Empty))
                .ToListAsync();
        }

        private async Task<int> NextId()
        {
            return (await _context.EmployeeCards.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1;
        }

        private static EmployeeCardV3Dto Map(EmployeeCardLegacy entity)
        {
            return new EmployeeCardV3Dto(
                entity.Id,
                entity.Code,
                entity.EmployeeId,
                entity.CardTypeId,
                entity.CardType?.Stxt,
                entity.AuthorityId,
                entity.Authority?.Stxt,
                entity.CardNo,
                entity.ValidFrom,
                entity.ValidTill);
        }

        private static string? Trim(string? value, int maxLength)
        {
            var trimmed = value?.Trim();
            if (string.IsNullOrEmpty(trimmed)) return null;
            return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength];
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
