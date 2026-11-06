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
    [Route("api/HumanResource/EREmpaddress/v3")]
    [Route("api/EREmpaddress/v3")]
    public sealed class EmployeeAddressV3Controller : ControllerBase
    {
        private readonly HumanResourceDbContext _context;

        public EmployeeAddressV3Controller(HumanResourceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<EmployeeAddressV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] int? employeeId = null)
        {
            var query = ScopedAddresses(GetClientId(), GetEntityId());

            if (employeeId.HasValue)
            {
                query = query.Where(x => x.EmployeeId == employeeId.Value);
            }

            var totalCount = await query.CountAsync();
            var entities = await query
                .OrderByDescending(x => x.PrimaryAddress)
                .ThenBy(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new PaginationResponse<EmployeeAddressV3Dto>
            {
                Items = entities.Select(Map).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeAddressV3Dto>> GetById(int id)
        {
            var entity = await ScopedAddresses(GetClientId(), GetEntityId())
                .FirstOrDefaultAsync(x => x.Id == id);

            return entity == null ? NotFound() : Ok(Map(entity));
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeAddressV3Dto>> Create([FromBody] UpsertEmployeeAddressV3Request request)
        {
            var clientId = GetClientId();
            var entityId = GetEntityId();
            var validation = await Validate(request, clientId, entityId);
            if (validation != null) return BadRequest(validation);

            if (request.PrimaryAddress)
            {
                await ClearPrimaryAddresses(request.EmployeeId);
            }

            var entity = new EmployeeAddressLegacy
            {
                Id = await NextId(),
                Code = Trim(request.Code, 10),
                EmployeeId = request.EmployeeId,
                Ltxt = Trim(request.Name, 50),
                AddressTypeId = request.AddressTypeId,
                CountryId = request.CountryId,
                StateId = request.StateId,
                CityId = request.CityId,
                Address = Trim(request.Address, 500),
                PrimaryAddress = request.PrimaryAddress,
                ContactName = Trim(request.ContactName, 50),
                Landline = Trim(request.Landline, 15),
                Ext = Trim(request.Ext, 5),
                Mobile = Trim(request.Mobile, 15),
                UtBlock = false,
                UtBlock1 = false
            };

            _context.EmployeeAddresses.Add(entity);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, Map(entity));
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpsertEmployeeAddressV3Request request)
        {
            if (!request.Id.HasValue) return BadRequest("Id is required for update.");

            var clientId = GetClientId();
            var entityId = GetEntityId();
            var validation = await Validate(request, clientId, entityId);
            if (validation != null) return BadRequest(validation);

            var entity = await ScopedAddresses(clientId, entityId)
                .FirstOrDefaultAsync(x => x.Id == request.Id.Value);

            if (entity == null) return NotFound();

            if (request.PrimaryAddress)
            {
                await ClearPrimaryAddresses(request.EmployeeId, entity.Id);
            }

            entity.Code = Trim(request.Code, 10);
            entity.Ltxt = Trim(request.Name, 50);
            entity.AddressTypeId = request.AddressTypeId;
            entity.CountryId = request.CountryId;
            entity.StateId = request.StateId;
            entity.CityId = request.CityId;
            entity.Address = Trim(request.Address, 500);
            entity.PrimaryAddress = request.PrimaryAddress;
            entity.ContactName = Trim(request.ContactName, 50);
            entity.Landline = Trim(request.Landline, 15);
            entity.Ext = Trim(request.Ext, 5);
            entity.Mobile = Trim(request.Mobile, 15);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await ScopedAddresses(GetClientId(), GetEntityId())
                .FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null) return NotFound();

            entity.UtBlock = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private IQueryable<EmployeeAddressLegacy> ScopedAddresses(int clientId, int? entityId)
        {
            var query = _context.EmployeeAddresses
                .Where(x => !(x.UtBlock ?? false))
                .Join(
                    _context.EmployeeProfiles,
                    address => address.EmployeeId,
                    employee => employee.Id,
                    (address, employee) => new { address, employee })
                .Where(x =>
                    x.employee.ClientId == clientId &&
                    !(x.employee.UtBlock ?? false) &&
                    (!entityId.HasValue || x.employee.EntityId == entityId.Value))
                .Select(x => x.address);

            return query;
        }

        private async Task<string?> Validate(UpsertEmployeeAddressV3Request request, int clientId, int? entityId)
        {
            if (!await IsEmployeeInScope(request.EmployeeId, clientId, entityId))
            {
                return "Employee does not belong to the current client/entity.";
            }

            if (!request.AddressTypeId.HasValue) return "Address Type is required.";
            if (string.IsNullOrWhiteSpace(request.Name)) return "Name is required.";
            if (!request.CountryId.HasValue) return "Country is required.";
            if (!request.StateId.HasValue) return "State is required.";
            if (!request.CityId.HasValue) return "City is required.";
            if (string.IsNullOrWhiteSpace(request.Address)) return "Address is required.";
            if (string.IsNullOrWhiteSpace(request.ContactName)) return "Contact Person is required.";
            if (!string.IsNullOrWhiteSpace(request.Landline) && request.Landline.Any(character => !char.IsDigit(character)))
                return "Landline must contain numeric values only.";

            return null;
        }

        private Task ClearPrimaryAddresses(int employeeId, int? exceptId = null)
        {
            var query = _context.EmployeeAddresses
                .Where(x => x.EmployeeId == employeeId && !(x.UtBlock ?? false) && (x.PrimaryAddress ?? false));

            if (exceptId.HasValue)
            {
                query = query.Where(x => x.Id != exceptId.Value);
            }

            return query.ExecuteUpdateAsync(x => x.SetProperty(row => row.PrimaryAddress, false));
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
            return (await _context.EmployeeAddresses.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id) ?? 0) + 1;
        }

        private static EmployeeAddressV3Dto Map(EmployeeAddressLegacy entity)
        {
            return new EmployeeAddressV3Dto(
                entity.Id,
                entity.Code,
                entity.EmployeeId,
                entity.Ltxt,
                entity.AddressTypeId,
                entity.CountryId,
                entity.StateId,
                entity.CityId,
                entity.Address,
                entity.PrimaryAddress ?? false,
                entity.ContactName,
                entity.Landline,
                entity.Ext,
                entity.Mobile);
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
