using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Route("api/CustomShift/v3")]
    [Authorize]
    public sealed class CustomShiftV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomShiftV3Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<CustomShiftV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] string? searchTerm = null)
        {
            var result = await _mediator.Send(new GetCustomShiftsV3Query(pageNumber, pageSize, GetClientId(), GetEntityId(), searchTerm));
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CustomShiftHeaderV3Dto>> GetById(int id)
        {
            var result = await _mediator.Send(new GetCustomShiftByIdV3Query(id, GetClientId(), GetEntityId()));
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("entity/{entityId:int}")]
        public async Task<ActionResult<List<CustomShiftV3Dto>>> GetByEntity(int entityId)
        {
            var result = await _mediator.Send(new GetCustomShiftsByEntityV3Query(entityId, GetClientId()));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<CustomShiftHeaderV3Dto>> Create([FromBody] CreateCustomShiftV3Request request)
        {
            if (request == null)
            {
                return BadRequest("Request cannot be null");
            }

            if (_mediator == null)
            {
                throw new InvalidOperationException("Mediator not available");
            }

            try
            {
                var result = await _mediator.Send(new CreateCustomShiftV3Command(request, GetClientId(), GetEntityId()));
                if (result == null)
                {
                    return BadRequest("Unable to create Custom Shift.");
                }

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateCustomShiftV3Request request)
        {
            try
            {
                var updated = await _mediator.Send(new UpdateCustomShiftV3Command(request, GetClientId(), GetEntityId()));
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
            var deleted = await _mediator.Send(new DeleteCustomShiftV3Command(id, GetClientId(), GetEntityId()));
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        private int GetClientId()
        {
            if (User == null)
            {
                throw new InvalidOperationException("User not authenticated");
            }

            try
            {
                var clientIdClaim = User.Claims.FirstOrDefault(c => c.Type == "ClientId");
                if (clientIdClaim != null && int.TryParse(clientIdClaim.Value, out var claimClientId))
                {
                    return claimClientId;
                }
            }
            catch (NullReferenceException)
            {
                throw new InvalidOperationException("User claims not found");
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
            if (User == null)
            {
                throw new InvalidOperationException("User not authenticated");
            }

            try
            {
                var entityIdClaim = User.Claims.FirstOrDefault(c => c.Type == "EntityId");
                if (entityIdClaim != null && int.TryParse(entityIdClaim.Value, out var claimEntityId))
                {
                    return claimEntityId;
                }
            }
            catch (NullReferenceException)
            {
                throw new InvalidOperationException("User claims not found");
            }

            if (Request.Headers.TryGetValue("X-Campus-Id", out var headerEntityId)
                && int.TryParse(headerEntityId, out var parsedHeaderEntityId))
            {
                return parsedHeaderEntityId;
            }

            if (Request.Headers.TryGetValue("X-Campus-Id", out var headerCampusId)
                && int.TryParse(headerCampusId, out var parsedCampusId))
            {
                return parsedCampusId;
            }

            if (Request.Headers.TryGetValue("Campus-Id", out var headerCampusId2)
                && int.TryParse(headerCampusId2, out var parsedCampusId2))
            {
                return parsedCampusId2;
            }

            return null;
        }
    }
}
