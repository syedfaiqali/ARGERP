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
    [Route("api/HumanResource/EREmployment/v3")]
    [Route("api/EREmployment/v3")]
    public sealed class EREmploymentV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;

        public EREmploymentV3Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<EmploymentV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] int? employeeId = null
        )
        {
            var result = await _mediator.Send(
                new GetEmploymentsV3Query(pageNumber, pageSize, GetClientId(), GetEntityId(), employeeId)
            );
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmploymentV3Dto>> GetById(int id)
        {
            var result = await _mediator.Send(
                new GetEmploymentByIdV3Query(id, GetClientId(), GetEntityId())
            );
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<EmploymentV3Dto>> Create([FromBody] EmploymentUpsertV3Request request)
        {
            try
            {
                var result = await _mediator.Send(
                    new CreateEmploymentV3Command(request, GetClientId(), GetEntityId())
                );
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] EmploymentUpsertV3Request request)
        {
            try
            {
                var updated = await _mediator.Send(
                    new UpdateEmploymentV3Command(request, GetClientId(), GetEntityId())
                );
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
            var deleted = await _mediator.Send(
                new DeleteEmploymentV3Command(id, GetClientId(), GetEntityId())
            );
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
            var entityIdClaim = User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if (entityIdClaim != null && int.TryParse(entityIdClaim.Value, out var claimEntityId)) return claimEntityId;
            if (Request.Headers.TryGetValue("X-Entity-Id", out var headerEntityId) && int.TryParse(headerEntityId, out var parsedHeaderEntityId)) return parsedHeaderEntityId;
            return null;
        }
    }
}
