using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Route("api/EmployeeAttendance/v3")]
    [Authorize]
    public sealed class EmployeeAttendanceV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeeAttendanceV3Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<EmployeeAttendanceV3Dto>>> GetAll(
            [FromQuery] int? employeeId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? tillDate = null)
        {
            var entityId = GetEntityId();
            var result = await _mediator.Send(new GetEmployeeAttendanceV3Query(GetClientId(), entityId, employeeId, fromDate, tillDate));
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeAttendanceV3Dto>> GetById(int id)
        {
            var entityId = GetEntityId();
            var result = await _mediator.Send(new GetEmployeeAttendanceByIdV3Query(id, GetClientId(), entityId));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("employees")]
        public async Task<ActionResult<IReadOnlyList<EmployeeAttendanceEmployeeDto>>> GetEmployees()
        {
            var entityId = GetEntityId();
            var result = await _mediator.Send(new GetEmployeeAttendanceEmployeesV3Query(GetClientId(), entityId));
            return Ok(result);
        }

        [HttpGet("activities")]
        public async Task<ActionResult<IReadOnlyList<EmployeeAttendanceActivityDto>>> GetActivities()
        {
            var result = await _mediator.Send(new GetEmployeeAttendanceActivitiesV3Query(GetClientId()));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeAttendanceV3Dto>> Create([FromBody] CreateEmployeeAttendanceV3Request request)
        {
            var entityId = GetEntityId();
            try
            {
                var result = await _mediator.Send(new CreateEmployeeAttendanceV3Command(request, GetClientId(), entityId));
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeAttendanceV3Request request)
        {
            if (id != request.Id) return BadRequest("Invalid attendance id.");
            var entityId = GetEntityId();
            try
            {
                var updated = await _mediator.Send(new UpdateEmployeeAttendanceV3Command(request, GetClientId(), entityId));
                if (!updated) return NotFound();
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
            var entityId = GetEntityId();
            var deleted = await _mediator.Send(new DeleteEmployeeAttendanceV3Command(id, GetClientId(), entityId));
            if (!deleted) return NotFound();
            return NoContent();
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
            if (Request.Headers.TryGetValue("X-Campus-Id", out var campusId)
                && int.TryParse(campusId, out var parsedCampusId)) return parsedCampusId;
            if (Request.Headers.TryGetValue("Campus-Id", out var campusIdAlias)
                && int.TryParse(campusIdAlias, out var parsedCampusIdAlias)) return parsedCampusIdAlias;
            var entityIdClaim = User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if (entityIdClaim != null && int.TryParse(entityIdClaim.Value, out var claimEntityId)) return claimEntityId;
            if (Request.Headers.TryGetValue("X-Entity-Id", out var headerEntityId) && int.TryParse(headerEntityId, out var parsedHeaderEntityId)) return parsedHeaderEntityId;
            return null;
        }
    }
}
