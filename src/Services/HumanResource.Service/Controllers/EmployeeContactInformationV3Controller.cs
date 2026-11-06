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
    [Route("api/EREmpRelation/v3")]
    [Route("api/HumanResource/EREmpRelation/v3")]
    public sealed class EmployeeContactInformationV3Controller : ControllerBase
    {
        private readonly HumanResourceDbContext _context;

        public EmployeeContactInformationV3Controller(HumanResourceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<EmployeeContactInformationV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] int? employeeId = null)
        {
            var query = ScopedContacts(GetClientId(), GetEntityId());
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

            return Ok(new PaginationResponse<EmployeeContactInformationV3Dto>
            {
                Items = entities.Select(Map).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeContactInformationV3Dto>> GetById(int id)
        {
            var entity = await ScopedContacts(GetClientId(), GetEntityId())
                .FirstOrDefaultAsync(x => x.Id == id);

            return entity == null ? NotFound() : Ok(Map(entity));
        }

        [HttpGet("lookups/relation-types")]
        [HttpGet("/api/HumanResource/Relationtype/v3")]
        [HttpGet("/api/HumanResource/Relationtype/v3/lookups")]
        [HttpGet("/api/Relationtype/v3")]
        [HttpGet("/api/Relationtype/v3/lookups")]
        public async Task<ActionResult<List<ContactLookupV3Dto>>> GetRelationTypes()
        {
            var items = await _context.RelationTypes
                .AsNoTracking()
                .Where(x => x.UtBlock == null || x.UtBlock == false)
                .OrderBy(x => x.Stxt ?? x.Ltxt ?? x.RelationTypeNo)
                .Select(x => new ContactLookupV3Dto(
                    x.Id,
                    x.RelationTypeNo,
                    x.Stxt ?? x.Ltxt ?? x.RelationTypeNo ?? x.Id.ToString()))
                .ToListAsync();

            return Ok(items);
        }

        [HttpGet("lookups/genders")]
        public async Task<ActionResult<List<ContactLookupV3Dto>>> GetGenders()
        {
            var items = await _context.Lovs
                .AsNoTracking()
                .Where(x => !x.UtBlock && x.Vtype != null && (x.Vtype == "GEND" || x.Vtype == "GTYPE"))
                .OrderBy(x => x.Stxt)
                .Select(x => new ContactLookupV3Dto(x.Id, x.Code, x.Stxt ?? string.Empty))
                .ToListAsync();

            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeContactInformationV3Dto>> Create([FromBody] UpsertEmployeeContactInformationV3Request request)
        {
            var clientId = GetClientId();
            var entityId = GetEntityId();
            var validation = await Validate(request, clientId, entityId);
            if (validation != null) return BadRequest(validation);

            var entity = new EmployeeContactInformationLegacy
            {
                Id = await NextId(),
                Code = Trim(request.Code, 20),
                EmployeeId = request.EmployeeId,
                RelationTypeId = request.RelationTypeId,
                GenderId = request.GenderId,
                FirstName = Trim(request.FirstName, 50),
                LastName = Trim(request.LastName, 50),
                Address = Trim(request.Address, 500),
                PhoneNo = Trim(request.PhoneNo, 15),
                MobileNo = Trim(request.MobileNo, 15),
                Email = Trim(request.Email, 100),
                Emergency = request.Emergency ?? false,
                NextOfKin = request.NextOfKin ?? false,
                UtSysGen = false,
                UtBlock1 = false,
                UtIsPartial = false,
                UtAddTime = DateTime.UtcNow,
                UtAddUser = GetUserName(),
                UtRecordVersion = 1,
                UtBlock = false
            };

            _context.EmployeeContacts.Add(entity);
            await _context.SaveChangesAsync();

            var created = await ScopedContacts(clientId, entityId).FirstAsync(x => x.Id == entity.Id);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, Map(created));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpsertEmployeeContactInformationV3Request request)
        {
            if (!request.Id.HasValue) return BadRequest("Id is required for update.");

            var clientId = GetClientId();
            var entityId = GetEntityId();
            var validation = await Validate(request, clientId, entityId);
            if (validation != null) return BadRequest(validation);

            var entity = await ScopedContacts(clientId, entityId).FirstOrDefaultAsync(x => x.Id == request.Id.Value);
            if (entity == null) return NotFound();

            entity.Code = Trim(request.Code, 20);
            entity.RelationTypeId = request.RelationTypeId;
            entity.GenderId = request.GenderId;
            entity.FirstName = Trim(request.FirstName, 50);
            entity.LastName = Trim(request.LastName, 50);
            entity.Address = Trim(request.Address, 500);
            entity.PhoneNo = Trim(request.PhoneNo, 15);
            entity.MobileNo = Trim(request.MobileNo, 15);
            entity.Email = Trim(request.Email, 100);
            entity.Emergency = request.Emergency ?? false;
            entity.NextOfKin = request.NextOfKin ?? false;
            entity.UtEditTime = DateTime.UtcNow;
            entity.UtEditUser = GetUserName();
            entity.UtRecordVersion += 1;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await ScopedContacts(GetClientId(), GetEntityId()).FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return NotFound();

            entity.UtBlock = true;
            entity.UtEditTime = DateTime.UtcNow;
            entity.UtEditUser = GetUserName();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private IQueryable<EmployeeContactInformationLegacy> ScopedContacts(int clientId, int? entityId)
        {
            var query = _context.EmployeeContacts
                .Include(x => x.Employee)
                .Include(x => x.RelationType)
                .Include(x => x.Gender)
                .Where(x => x.Employee != null && x.Employee.ClientId == clientId && !x.UtBlock && !(x.Employee.UtBlock ?? false));

            if (entityId.HasValue)
            {
                query = query.Where(x => x.Employee != null && x.Employee.EntityId == entityId.Value);
            }

            return query;
        }

        private async Task<string?> Validate(UpsertEmployeeContactInformationV3Request request, int clientId, int? entityId)
        {
            if (!await IsEmployeeInScope(request.EmployeeId, clientId, entityId))
            {
                return "Employee does not belong to the current client/entity.";
            }

            if (!request.RelationTypeId.HasValue) return "Relation Type is required.";
            if (string.IsNullOrWhiteSpace(request.FirstName)) return "First Name is required.";
            if (string.IsNullOrWhiteSpace(request.MobileNo)) return "Mobile No is required.";

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
            return (await _context.EmployeeContacts.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1;
        }

        private static EmployeeContactInformationV3Dto Map(EmployeeContactInformationLegacy entity)
        {
            return new EmployeeContactInformationV3Dto(
                entity.Id,
                entity.Code,
                entity.EmployeeId,
                entity.RelationTypeId,
                entity.RelationType?.Stxt,
                entity.GenderId,
                entity.Gender?.Stxt,
                entity.FirstName,
                entity.LastName,
                entity.Address,
                entity.PhoneNo,
                entity.MobileNo,
                entity.Email,
                entity.Emergency ?? false,
                entity.NextOfKin ?? false);
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
