using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Route("api/DeductionGroup/v3")]
    [Authorize]
    public sealed class DeductionGroupV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;

        public DeductionGroupV3Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeductionGroupV3Dto>>> Get([FromQuery] string? searchTerm = null)
        {
            var result = await _mediator.Send(new GetDeductionGroupsV3Query(GetClientId(), GetEntityId(), searchTerm));
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<DeductionGroupHeaderV3Dto>> GetById(int id)
        {
            var result = await _mediator.Send(new GetDeductionGroupByIdV3Query(id, GetClientId(), GetEntityId()));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("lookups")]
        public async Task<ActionResult<DeductionGroupLookupsV3Dto>> GetLookups()
        {
            var result = await _mediator.Send(new GetDeductionGroupLookupsV3Query(GetClientId(), GetEntityId()));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<DeductionGroupHeaderV3Dto>> Create([FromBody] CreateDeductionGroupV3Request request)
        {
            try
            {
                var result = await _mediator.Send(new CreateDeductionGroupV3Command(request, GetClientId(), GetEntityId()));
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDeductionGroupV3Request request)
        {
            if (id != request.Id) return BadRequest("ID mismatch");

            try
            {
                var updated = await _mediator.Send(new UpdateDeductionGroupV3Command(request, GetClientId(), GetEntityId()));
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
            var deleted = await _mediator.Send(new DeleteDeductionGroupV3Command(id, GetClientId(), GetEntityId()));
            if (!deleted) return NotFound();
            return NoContent();
        }

        private int GetClientId()
        {
            var clientIdClaim = User.Claims.FirstOrDefault(c =>
                c.Type == "ClientId" || c.Type == "clientId" || c.Type == "client_id");
            if (clientIdClaim != null && int.TryParse(clientIdClaim.Value, out var claimClientId))
            {
                return claimClientId;
            }

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
            if (entityIdClaim != null && int.TryParse(entityIdClaim.Value, out var claimEntityId))
            {
                return claimEntityId;
            }

            if (Request.Headers.TryGetValue("X-Entity-Id", out var headerEntityId) &&
                int.TryParse(headerEntityId, out var parsedHeaderEntityId))
            {
                return parsedHeaderEntityId;
            }

            return null;
        }
    }
}
