using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Route("api/UploadAttendance/v3")]
    [Authorize]
    public sealed class UploadAttendanceV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;

        public UploadAttendanceV3Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("lookups")]
        public async Task<ActionResult<UploadAttendanceLookupsV3Dto>> GetLookups()
        {
            var result = await _mediator.Send(new GetUploadAttendanceLookupsV3Query(GetClientId()));
            return Ok(result);
        }

        [HttpGet("employees")]
        public async Task<ActionResult<IReadOnlyList<UploadAttendanceEmployeeV3Dto>>> GetEmployees()
        {
            var result = await _mediator.Send(new GetUploadAttendanceEmployeesV3Query(GetClientId(), GetEntityId()));
            return Ok(result);
        }

        [HttpPost("process")]
        public async Task<ActionResult<UploadAttendanceProcessResultV3Dto>> Process([FromBody] UploadAttendanceProcessV3Request request)
        {
            try
            {
                var result = await _mediator.Send(new ProcessUploadAttendanceV3Command(request, GetClientId(), GetEntityId()));
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private int GetClientId()
        {
            var clientIdClaim = User.Claims.FirstOrDefault(c =>
                c.Type == "ClientId" || c.Type == "clientId" || c.Type == "client_id");
            if (clientIdClaim != null && int.TryParse(clientIdClaim.Value, out var claimClientId)) return claimClientId;

            if (Request.Headers.TryGetValue("X-Client-Id", out var headerClientId) &&
                int.TryParse(headerClientId, out var parsedHeaderClientId))
            {
                return parsedHeaderClientId;
            }

            throw new InvalidOperationException("ClientId not found in token or headers");
        }

        private int? GetEntityId()
        {
            var entityIdClaim = User.Claims.FirstOrDefault(c =>
                c.Type == "EntityId" || c.Type == "entityId" || c.Type == "entity_id");
            if (entityIdClaim != null && int.TryParse(entityIdClaim.Value, out var claimEntityId)) return claimEntityId;

            if (Request.Headers.TryGetValue("X-Entity-Id", out var headerEntityId) &&
                int.TryParse(headerEntityId, out var parsedHeaderEntityId))
            {
                return parsedHeaderEntityId;
            }

            return null;
        }
    }
}
