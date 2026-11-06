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
    [Route("api/EREmployeedisease/v3")]
    [Route("EREmployeedisease/v3")]
    public sealed class EmployeeDiseaseV3Controller : ControllerBase
    {
        private readonly HumanResourceDbContext _context;

        public EmployeeDiseaseV3Controller(HumanResourceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<EmployeeDiseaseV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] int? employeeId = null)
        {
            var query = ScopedDiseases(GetClientId(), GetEntityId());
            if (employeeId.HasValue)
            {
                query = query.Where(x => x.EmployeeId == employeeId.Value);
            }

            var totalCount = await query.CountAsync();
            var entities = await query
                .OrderBy(x => x.LineIndex)
                .ThenBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new PaginationResponse<EmployeeDiseaseV3Dto>
            {
                Items = entities.Select(Map).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeDiseaseV3Dto>> GetById(int id)
        {
            var entity = await ScopedDiseases(GetClientId(), GetEntityId())
                .FirstOrDefaultAsync(x => x.Id == id);

            return entity == null ? NotFound() : Ok(Map(entity));
        }

        [HttpGet("lookups/categories")]
        public Task<List<EmployeeDiseaseLookupV3Dto>> GetCategories()
        {
            return GetLovLookup("DISCT");
        }

        [HttpGet("lookups/types")]
        public Task<List<EmployeeDiseaseLookupV3Dto>> GetTypes()
        {
            return GetLovLookup("DISTP");
        }

        [HttpGet("lookups/diseases")]
        public Task<List<EmployeeDiseaseLookupV3Dto>> GetDiseases()
        {
            var clientId = GetClientId();
            var entityId = GetEntityId();

            return _context.DiseaseMasters
                .AsNoTracking()
                .Where(x =>
                    !x.UtBlock &&
                    (x.ClientId == null || x.ClientId == 0 || x.ClientId == clientId) &&
                    (!entityId.HasValue || x.EntityId == null || x.EntityId == 0 || x.EntityId == entityId.Value))
                .OrderBy(x => x.Stxt)
                .Select(x => new EmployeeDiseaseLookupV3Dto(
                    x.Id,
                    x.Code,
                    x.Stxt ?? string.Empty,
                    x.DiseaseCategoryId,
                    x.DiseaseTypeId))
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeDiseaseV3Dto>> Create([FromBody] UpsertEmployeeDiseaseV3Request request)
        {
            var clientId = GetClientId();
            var entityId = GetEntityId();
            var validation = await Validate(request, clientId, entityId);
            if (validation != null) return BadRequest(validation);

            var resolved = await ResolveDisease(request, clientId, entityId);
            var entity = new EmployeeDiseaseLegacy
            {
                Id = await NextId(),
                LineIndex = await NextLineIndex(request.EmployeeId, clientId, entityId),
                EmployeeId = request.EmployeeId,
                DiseaseId = resolved.DiseaseId,
                DiseaseCategoryId = resolved.CategoryId,
                DiseaseTypeId = resolved.TypeId,
                Stxt = Trim(resolved.Name, 150),
                Ltxt = Trim(request.Description, 500),
                UtSysGen = false,
                UtBlock = false,
                UtIsPartial = false,
                UtAddTime = DateTime.UtcNow,
                UtAddUser = GetUserName(),
                UtRecordVersion = 1
            };

            _context.EmployeeDiseases.Add(entity);
            await _context.SaveChangesAsync();

            var created = await ScopedDiseases(clientId, entityId).FirstAsync(x => x.Id == entity.Id);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, Map(created));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpsertEmployeeDiseaseV3Request request)
        {
            if (!request.Id.HasValue) return BadRequest("Id is required for update.");

            var clientId = GetClientId();
            var entityId = GetEntityId();
            var validation = await Validate(request, clientId, entityId);
            if (validation != null) return BadRequest(validation);

            var entity = await ScopedDiseases(clientId, entityId).FirstOrDefaultAsync(x => x.Id == request.Id.Value);
            if (entity == null) return NotFound();

            var resolved = await ResolveDisease(request, clientId, entityId);
            entity.DiseaseId = resolved.DiseaseId;
            entity.DiseaseCategoryId = resolved.CategoryId;
            entity.DiseaseTypeId = resolved.TypeId;
            entity.Stxt = Trim(resolved.Name, 150);
            entity.Ltxt = Trim(request.Description, 500);
            entity.UtEditTime = DateTime.UtcNow;
            entity.UtEditUser = GetUserName();
            entity.UtRecordVersion += 1;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await ScopedDiseases(GetClientId(), GetEntityId()).FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return NotFound();

            entity.UtBlock = true;
            entity.UtEditTime = DateTime.UtcNow;
            entity.UtEditUser = GetUserName();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private IQueryable<EmployeeDiseaseLegacy> ScopedDiseases(int clientId, int? entityId)
        {
            var query = _context.EmployeeDiseases
                .Include(x => x.Employee)
                .Include(x => x.Disease)
                .Include(x => x.DiseaseCategory)
                .Include(x => x.DiseaseType)
                .Where(x => x.Employee != null && x.Employee.ClientId == clientId && !x.UtBlock && !(x.Employee.UtBlock ?? false));

            if (entityId.HasValue)
            {
                query = query.Where(x => x.Employee != null && x.Employee.EntityId == entityId.Value);
            }

            return query;
        }

        private async Task<string?> Validate(UpsertEmployeeDiseaseV3Request request, int clientId, int? entityId)
        {
            if (!await IsEmployeeInScope(request.EmployeeId, clientId, entityId))
            {
                return "Employee does not belong to the current client/entity.";
            }

            if (!request.DiseaseCategoryId.HasValue) return "Category is required.";
            if (!request.DiseaseTypeId.HasValue) return "Type is required.";
            if (!request.DiseaseId.HasValue && string.IsNullOrWhiteSpace(request.DiseaseName)) return "Disease Name is required.";

            return null;
        }

        private async Task<(int? DiseaseId, int? CategoryId, int? TypeId, string? Name)> ResolveDisease(
            UpsertEmployeeDiseaseV3Request request,
            int clientId,
            int? entityId)
        {
            if (!request.DiseaseId.HasValue)
            {
                return (null, request.DiseaseCategoryId, request.DiseaseTypeId, request.DiseaseName);
            }

            var disease = await _context.DiseaseMasters
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == request.DiseaseId.Value &&
                    !x.UtBlock &&
                    (x.ClientId == null || x.ClientId == 0 || x.ClientId == clientId) &&
                    (!entityId.HasValue || x.EntityId == null || x.EntityId == 0 || x.EntityId == entityId.Value));

            if (disease == null)
            {
                return (request.DiseaseId, request.DiseaseCategoryId, request.DiseaseTypeId, request.DiseaseName);
            }

            return (
                disease.Id,
                disease.DiseaseCategoryId ?? request.DiseaseCategoryId,
                disease.DiseaseTypeId ?? request.DiseaseTypeId,
                string.IsNullOrWhiteSpace(request.DiseaseName) ? disease.Stxt : request.DiseaseName);
        }

        private Task<bool> IsEmployeeInScope(int employeeId, int clientId, int? entityId)
        {
            return _context.EmployeeProfiles.AnyAsync(x =>
                x.Id == employeeId &&
                x.ClientId == clientId &&
                (!entityId.HasValue || x.EntityId == entityId.Value) &&
                !(x.UtBlock ?? false));
        }

        private Task<List<EmployeeDiseaseLookupV3Dto>> GetLovLookup(string vtype)
        {
            return _context.Lovs
                .AsNoTracking()
                .Where(x => !x.UtBlock && x.Vtype == vtype)
                .OrderBy(x => x.Stxt)
                .Select(x => new EmployeeDiseaseLookupV3Dto(x.Id, x.Code, x.Stxt ?? string.Empty, null, null))
                .ToListAsync();
        }

        private async Task<int> NextId()
        {
            return (await _context.EmployeeDiseases.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1;
        }

        private async Task<int> NextLineIndex(int employeeId, int clientId, int? entityId)
        {
            return ((await ScopedDiseases(clientId, entityId)
                .Where(x => x.EmployeeId == employeeId)
                .MaxAsync(x => (int?)x.LineIndex)) ?? 0) + 1;
        }

        private static EmployeeDiseaseV3Dto Map(EmployeeDiseaseLegacy entity)
        {
            var diseaseName = !string.IsNullOrWhiteSpace(entity.Stxt)
                ? entity.Stxt
                : entity.Disease?.Stxt;

            return new EmployeeDiseaseV3Dto(
                entity.Id,
                entity.EmployeeId,
                entity.DiseaseCategoryId,
                entity.DiseaseCategory?.Stxt,
                entity.DiseaseTypeId,
                entity.DiseaseType?.Stxt,
                entity.DiseaseId,
                entity.Disease?.Stxt,
                diseaseName,
                entity.Ltxt);
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
