using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Route("api/AttendancePolicy/v3")]
    [Authorize]
    public sealed class AttendancePolicyV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;

        public AttendancePolicyV3Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttendancePolicyV3Dto>>> Get([FromQuery] string? searchTerm = null)
        {
            var result = await _mediator.Send(new GetAttendancePoliciesV3Query(GetClientId(), GetEntityId(), searchTerm));
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AttendancePolicyHeaderV3Dto>> GetById(int id)
        {
            var result = await _mediator.Send(new GetAttendancePolicyByIdV3Query(id, GetClientId(), GetEntityId()));
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<AttendancePolicyV3Dto>> Create([FromBody] CreateAttendancePolicyV3Request request)
        {
            try
            {
                var result = await _mediator.Send(new CreateAttendancePolicyV3Command(request, GetClientId(), GetEntityId()));
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAttendancePolicyV3Request request)
        {
            if (id != request.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var updated = await _mediator.Send(new UpdateAttendancePolicyV3Command(request, GetClientId(), GetEntityId()));
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
            var deleted = await _mediator.Send(new DeleteAttendancePolicyV3Command(id, GetClientId(), GetEntityId()));
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
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
    }
}
