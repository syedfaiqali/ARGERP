using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/HumanResource/DeviceRegistration/v3")]
    [Route("api/DeviceRegistration/v3")]
    [EDXS.Shared.Workflow.WorkflowAction("SMMachineRegistration")]
    public sealed class DeviceRegistrationV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DeviceRegistrationV3Controller> _logger;

        public DeviceRegistrationV3Controller(
            IMediator mediator,
            ILogger<DeviceRegistrationV3Controller> logger
        )
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet("lookups")]
        public async Task<ActionResult<DeviceRegistrationLookupsV3Dto>> GetLookups([FromQuery] int? entityId = null)
        {
            try
            {
                var result = await _mediator.Send(
                    new GetDeviceRegistrationLookupsV3Query(GetClientId(), ResolveWriteEntityId(entityId))
                );
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving device registration lookups");
                return StatusCode(500, ToErrorPayload(ex));
            }
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<DeviceRegistrationV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? entityId = null
        )
        {
            try
            {
                var result = await _mediator.Send(
                    new GetDeviceRegistrationsV3Query(pageNumber, pageSize, GetClientId(), ResolveWriteEntityId(entityId), searchTerm)
                );
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving device registrations");
                return StatusCode(500, ToErrorPayload(ex));
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DeviceRegistrationV3Dto>> GetById(
            int id,
            [FromQuery] int? entityId = null
        )
        {
            try
            {
                var result = await _mediator.Send(
                    new GetDeviceRegistrationByIdV3Query(id, GetClientId(), ResolveWriteEntityId(entityId))
                );
                if (result == null)
                {
                    return NotFound(new { error = "Record not found" });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving device registration by id");
                return StatusCode(500, ToErrorPayload(ex));
            }
        }

        [HttpPost]
        public async Task<ActionResult<DeviceRegistrationV3Dto>> Create(
            [FromBody] DeviceRegistrationUpsertV3Request? request,
            [FromQuery] int? entityId = null
        )
        {
            try
            {
                if (request == null)
                    return BadRequest(new { error = "Request body is required" });

                var body = request;
                var resolvedEntityId = ResolveWriteEntityId(entityId, body.EntityId);
                var result = await _mediator.Send(
                    new CreateDeviceRegistrationV3Command(
                        body,
                        GetClientId(),
                        resolvedEntityId <= 0 ? null : resolvedEntityId
                    )
                );
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating device registration");
                return StatusCode(500, ToErrorPayload(ex));
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<DeviceRegistrationV3Dto>> Update(
            int id,
            [FromBody] DeviceRegistrationUpsertV3Request? request,
            [FromQuery] int? entityId = null
        )
        {
            try
            {
                if (request == null)
                    return BadRequest(new { error = "Request body is required" });

                var body = request;
                var resolvedEntityId = ResolveWriteEntityId(entityId, body.EntityId);
                var result = await _mediator.Send(
                    new UpdateDeviceRegistrationV3Command(
                        id,
                        body,
                        GetClientId(),
                        resolvedEntityId <= 0 ? null : resolvedEntityId
                    )
                );
                if (result == null)
                {
                    return NotFound(new { error = "Record not found" });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating device registration");
                return StatusCode(500, ToErrorPayload(ex));
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(
            int id,
            [FromQuery] int? entityId = null
        )
        {
            try
            {
                var ok = await _mediator.Send(
                    new DeleteDeviceRegistrationV3Command(id, GetClientId(), ResolveWriteEntityId(entityId))
                );
                if (!ok)
                {
                    return NotFound(new { error = "Record not found" });
                }

                return Ok(true);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting device registration");
                return StatusCode(500, ToErrorPayload(ex));
            }
        }

        private static object ToErrorPayload(Exception ex)
        {
            // Other v3 controllers return the raw exception message; keep the same behavior here
            // but wrap it in `{ error }` to preserve this controller's response shape.
            var inner = ex.InnerException?.Message;
            return inner == null
                ? new { error = ex.Message }
                : new { error = ex.Message, detail = inner };
        }

        private int GetClientId()
        {
            var clientIdClaim = User.Claims.FirstOrDefault(c => c.Type == "ClientId");
            if (clientIdClaim != null && int.TryParse(clientIdClaim.Value, out var claimClientId))
            {
                return claimClientId;
            }

            var clientIdClaimAlt = User.Claims.FirstOrDefault(c =>
                c.Type == "clientId" || c.Type == "client_id");
            if (clientIdClaimAlt != null && int.TryParse(clientIdClaimAlt.Value, out var claimClientIdAlt))
            {
                return claimClientIdAlt;
            }

            if (Request.Headers.TryGetValue("X-Client-Id", out var headerClientId)
                && int.TryParse(headerClientId, out var parsedHeaderClientId))
            {
                return parsedHeaderClientId;
            }

            // Align with other v3 controllers: missing ClientId is a caller/config issue and should not return 500.
            throw new InvalidOperationException("ClientId not found in token or headers");
        }

        private int? GetEntityId()
        {
            var claimEntityId = User.FindFirst("EntityId")?.Value
                ?? User.FindFirst("entityId")?.Value
                ?? User.FindFirst("entity_id")?.Value;

            int? claimEntityIdParsed = null;
            if (int.TryParse(claimEntityId, out var parsedClaimEntityId) && parsedClaimEntityId > 0)
                claimEntityIdParsed = parsedClaimEntityId;

            int? headerEntityIdParsed = null;
            if (Request.Headers.TryGetValue("X-Entity-Id", out var xEntityId)
                && int.TryParse(xEntityId.ToString(), out var parsedXEntityId) && parsedXEntityId > 0)
            {
                headerEntityIdParsed = parsedXEntityId;
            }
            else if (Request.Headers.TryGetValue("Entity-Id", out var entityId)
                && int.TryParse(entityId.ToString(), out var parsedEntityId) && parsedEntityId > 0)
            {
                headerEntityIdParsed = parsedEntityId;
            }

            if (claimEntityIdParsed.HasValue && headerEntityIdParsed.HasValue && headerEntityIdParsed.Value != claimEntityIdParsed.Value)
                throw new InvalidOperationException("Entity scope header does not match authenticated token");

            return claimEntityIdParsed ?? headerEntityIdParsed;
        }

        private int ResolveWriteEntityId(int? queryEntityId = null, int? bodyEntityId = null)
        {
            var authEntityId = GetEntityId();
            var requestedEntityId = bodyEntityId ?? queryEntityId;

            if (requestedEntityId.HasValue && requestedEntityId.Value <= 0)
                requestedEntityId = null;

            if (authEntityId.HasValue)
            {
                return authEntityId.Value;
            }

            if (requestedEntityId.HasValue)
                return requestedEntityId.Value;

            return 0; // fallback if necessary, or throw
        }
    }
}
