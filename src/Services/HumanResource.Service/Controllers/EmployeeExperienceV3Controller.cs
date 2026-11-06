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
    [Route("api/HumanResource/EREmpExperience/v3")]
    [Route("api/EREmpExperience/v3")]
    [Route("EREmpExperience/v3")]
    public sealed class EmployeeExperienceV3Controller : ControllerBase
    {
        private readonly HumanResourceDbContext _context;

        public EmployeeExperienceV3Controller(HumanResourceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<EmployeeExperienceV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] int? employeeId = null)
        {
            var query = ScopedExperiences(GetClientId(), GetEntityId());
            if (employeeId.HasValue)
            {
                query = query.Where(x => x.EmployeeId == employeeId.Value);
            }

            var totalCount = await query.CountAsync();
            var entities = await query
                .OrderByDescending(x => x.StartDate)
                .ThenBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new PaginationResponse<EmployeeExperienceV3Dto>
            {
                Items = entities.Select(Map).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeExperienceV3Dto>> GetById(int id)
        {
            var entity = await ScopedExperiences(GetClientId(), GetEntityId())
                .FirstOrDefaultAsync(x => x.Id == id);

            return entity == null ? NotFound() : Ok(Map(entity));
        }

        [HttpGet("lookups/business-natures")]
        public async Task<ActionResult<List<EmployeeExperienceLookupV3Dto>>> GetBusinessNatures()
        {
            var items = await _context.Lovs
                .AsNoTracking()
                .Where(x => !x.UtBlock && x.Vtype == "BSNAT")
                .OrderBy(x => x.Stxt)
                .Select(x => new EmployeeExperienceLookupV3Dto(x.Id, x.Code, x.Stxt ?? string.Empty))
                .ToListAsync();

            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeExperienceV3Dto>> Create([FromBody] UpsertEmployeeExperienceV3Request request)
        {
            var clientId = GetClientId();
            var entityId = GetEntityId();
            var validation = await Validate(request, clientId, entityId);
            if (validation != null) return BadRequest(validation);

            var entity = new EmployeeExperienceLegacy
            {
                Id = await NextId(),
                Code = Trim(request.Code, 20),
                EmployeeId = request.EmployeeId,
                BusinessNatureId = request.BusinessNatureId,
                Company = Trim(request.Company, 150),
                Department = Trim(request.Department, 50),
                Designation = Trim(request.Designation, 50),
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Salary = request.Salary,
                Address = Trim(request.Address, 500),
                Phone = Trim(request.Phone, 15),
                Email = Trim(request.Email, 50),
                Web = Trim(request.Web, 50),
                Responsibility = Trim(request.Responsibility, 1500),
                ContactPerson = Trim(request.ContactPerson, 150),
                PersonDesignation = Trim(request.PersonDesignation, 150),
                PersonAddress = Trim(request.PersonAddress, 150),
                PersonPhone = Trim(request.PersonPhone, 20),
                PersonEmail = Trim(request.PersonEmail, 100),
                PersonWeb = Trim(request.PersonWeb, 100),
                UtSysGen = false,
                UtBlock = false,
                UtIsPartial = false,
                UtAddTime = DateTime.UtcNow,
                UtAddUser = GetUserName(),
                UtRecordVersion = 1
            };

            _context.EmployeeExperiences.Add(entity);
            await _context.SaveChangesAsync();

            var created = await ScopedExperiences(clientId, entityId).FirstAsync(x => x.Id == entity.Id);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, Map(created));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpsertEmployeeExperienceV3Request request)
        {
            if (!request.Id.HasValue) return BadRequest("Id is required for update.");

            var clientId = GetClientId();
            var entityId = GetEntityId();
            var validation = await Validate(request, clientId, entityId);
            if (validation != null) return BadRequest(validation);

            var entity = await ScopedExperiences(clientId, entityId).FirstOrDefaultAsync(x => x.Id == request.Id.Value);
            if (entity == null) return NotFound();

            entity.Code = Trim(request.Code, 20);
            entity.BusinessNatureId = request.BusinessNatureId;
            entity.Company = Trim(request.Company, 150);
            entity.Department = Trim(request.Department, 50);
            entity.Designation = Trim(request.Designation, 50);
            entity.StartDate = request.StartDate;
            entity.EndDate = request.EndDate;
            entity.Salary = request.Salary;
            entity.Address = Trim(request.Address, 500);
            entity.Phone = Trim(request.Phone, 15);
            entity.Email = Trim(request.Email, 50);
            entity.Web = Trim(request.Web, 50);
            entity.Responsibility = Trim(request.Responsibility, 1500);
            entity.ContactPerson = Trim(request.ContactPerson, 150);
            entity.PersonDesignation = Trim(request.PersonDesignation, 150);
            entity.PersonAddress = Trim(request.PersonAddress, 150);
            entity.PersonPhone = Trim(request.PersonPhone, 20);
            entity.PersonEmail = Trim(request.PersonEmail, 100);
            entity.PersonWeb = Trim(request.PersonWeb, 100);
            entity.UtEditTime = DateTime.UtcNow;
            entity.UtEditUser = GetUserName();
            entity.UtRecordVersion += 1;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await ScopedExperiences(GetClientId(), GetEntityId()).FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null) return NotFound();

            entity.UtBlock = true;
            entity.UtEditTime = DateTime.UtcNow;
            entity.UtEditUser = GetUserName();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private IQueryable<EmployeeExperienceLegacy> ScopedExperiences(int clientId, int? entityId)
        {
            var query = _context.EmployeeExperiences
                .Include(x => x.Employee)
                .Include(x => x.BusinessNature)
                .Where(x => x.Employee != null && x.Employee.ClientId == clientId && !x.UtBlock && !(x.Employee.UtBlock ?? false));

            if (entityId.HasValue)
            {
                query = query.Where(x => x.Employee != null && x.Employee.EntityId == entityId.Value);
            }

            return query;
        }

        private async Task<string?> Validate(UpsertEmployeeExperienceV3Request request, int clientId, int? entityId)
        {
            if (!await IsEmployeeInScope(request.EmployeeId, clientId, entityId))
            {
                return "Employee does not belong to the current client/entity.";
            }

            if (!request.BusinessNatureId.HasValue) return "Business Nature is required.";
            if (string.IsNullOrWhiteSpace(request.Company)) return "Company is required.";
            if (string.IsNullOrWhiteSpace(request.Department)) return "Department is required.";
            if (!request.StartDate.HasValue) return "Start Date is required.";
            if (!request.EndDate.HasValue) return "End Date is required.";
            if (!request.Salary.HasValue) return "Salary is required.";
            if (request.StartDate.Value.Date > request.EndDate.Value.Date)
                return "Start Date cannot be greater than End Date.";
            if (request.Salary.Value < 0) return "Salary cannot be negative.";

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
            return (await _context.EmployeeExperiences.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1;
        }

        private static EmployeeExperienceV3Dto Map(EmployeeExperienceLegacy entity)
        {
            return new EmployeeExperienceV3Dto(
                entity.Id,
                entity.Code,
                entity.EmployeeId,
                entity.BusinessNatureId,
                entity.BusinessNature?.Stxt,
                entity.Company,
                entity.Department,
                entity.Designation,
                entity.StartDate,
                entity.EndDate,
                entity.Salary,
                entity.Address,
                entity.Phone,
                entity.Email,
                entity.Web,
                entity.Responsibility,
                entity.ContactPerson,
                entity.PersonDesignation,
                entity.PersonAddress,
                entity.PersonPhone,
                entity.PersonEmail,
                entity.PersonWeb);
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
