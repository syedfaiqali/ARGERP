using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Route("api/Shift/v3")]
    [Authorize]
    public sealed class ShiftV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;

        public ShiftV3Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<ShiftV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] string? searchTerm = null)
        {
            // The Shift setup list is client-wide for department shared shifts.
            // Department matching is intentionally applied only by the Employee
            // Profile shift lookup, where an employee's department is known.
            var result = await _mediator.Send(new GetShiftsV3Query(pageNumber, pageSize, GetClientId(), GetEntityId(), searchTerm));
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ShiftHeaderV3Dto>> GetById(int id)
        {
            var result = await _mediator.Send(new GetShiftByIdV3Query(id, GetClientId(), GetEntityId()));
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ShiftHeaderV3Dto>> Create([FromBody] CreateShiftV3Request request)
        {
            try
            {
                var result = await _mediator.Send(new CreateShiftV3Command(request, GetClientId(), GetEntityId()));
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateShiftV3Request request)
        {
            try
            {
                var updated = await _mediator.Send(new UpdateShiftV3Command(request, GetClientId(), GetEntityId()));
                if (!updated)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _mediator.Send(new DeleteShiftV3Command(id, GetClientId(), GetEntityId()));
                if (!deleted)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private int GetClientId()
        {
            var clientIdClaim = User.Claims.FirstOrDefault(c => c.Type == "ClientId");
            if (clientIdClaim != null && int.TryParse(clientIdClaim.Value, out var claimClientId))
            {
                return claimClientId;
            }

            if (Request.Headers.TryGetValue("X-Client-Id", out var headerClientId)
                && int.TryParse(headerClientId, out var parsedHeaderClientId))
            {
                return parsedHeaderClientId;
            }

            throw new InvalidOperationException("ClientId not found in token or headers");
        }

        private int? GetEntityId()
        {
            var entityIdClaim = User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if (entityIdClaim != null && int.TryParse(entityIdClaim.Value, out var claimEntityId))
            {
                return claimEntityId;
            }

            if (Request.Headers.TryGetValue("X-Entity-Id", out var headerEntityId)
                && int.TryParse(headerEntityId, out var parsedHeaderEntityId))
            {
                return parsedHeaderEntityId;
            }

            return null;
        }

        private int? GetEmployeeId()
        {
            var employeeIdClaim = User.Claims.FirstOrDefault(c => c.Type is "EmployeeId" or "EmpId");
            if (employeeIdClaim != null && int.TryParse(employeeIdClaim.Value, out var claimEmployeeId))
            {
                return claimEmployeeId;
            }

            if (Request.Headers.TryGetValue("X-Employee-Id", out var headerEmployeeId)
                && int.TryParse(headerEmployeeId, out var parsedHeaderEmployeeId))
            {
                return parsedHeaderEmployeeId;
            }

            return null;
        }
    }
}
