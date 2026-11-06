using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Queries;
using System.Linq;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Route("api/DeviceType/v3")]
    [Authorize]
    [EDXS.Shared.Workflow.WorkflowAction("DeviceType")]
    public sealed class DeviceTypeV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;

        public DeviceTypeV3Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<DeviceTypeV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] string? searchTerm = null)
        {
            try
            {
                var result = await _mediator.Send(new GetDeviceTypesV3Query(pageNumber, pageSize, GetClientId(), searchTerm));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DeviceTypeV3Dto>> GetById(int id)
        {
            try
            {
                var result = await _mediator.Send(new GetDeviceTypeByIdV3Query(id, GetClientId()));
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<DeviceTypeV3Dto>> Create([FromBody] CreateDeviceTypeV3Request request)
        {
            try
            {
                var result = await _mediator.Send(new CreateDeviceTypeV3Command(request, GetClientId()));
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // The id also travels in the route so an approval workflow can identify the record.
        [HttpPut]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int? id, [FromBody] UpdateDeviceTypeV3Request request)
        {
            try
            {
                if (id.HasValue && id.Value != request.Id) request = request with { Id = id.Value };
                var updated = await _mediator.Send(new UpdateDeviceTypeV3Command(request, GetClientId()));
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
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _mediator.Send(new DeleteDeviceTypeV3Command(id, GetClientId()));
                if (!deleted)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private int GetClientId()
        {
            try
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
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to get ClientId: {ex.Message}");
            }
        }
    }
}
