using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Queries;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Route("api/PositionType/v3")]
    [Route("api/HumanResource/PositionType/v3")]
    [Authorize]
    public sealed class PositionTypeV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;

        public PositionTypeV3Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<PositionTypeV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] string? searchTerm = null)
        {
            var result = await _mediator.Send(new GetPositionTypesV3Query(pageNumber, pageSize, GetClientId(), GetEntityId(), searchTerm));
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PositionTypeV3Dto>> GetById(int id)
        {
            var result = await _mediator.Send(new GetPositionTypeByIdV3Query(id, GetClientId(), GetEntityId()));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<PositionTypeV3Dto>> Create([FromBody] CreatePositionTypeV3Request request)
        {
            try
            {
                var result = await _mediator.Send(new CreatePositionTypeV3Command(request, GetClientId(), GetEntityId()));
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int? id, [FromBody] UpdatePositionTypeV3Request request)
        {
            try
            {
                if (id.HasValue && id.Value != request.Id) request = request with { Id = id.Value };
                var updated = await _mediator.Send(new UpdatePositionTypeV3Command(request, GetClientId(), GetEntityId()));
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
            var deleted = await _mediator.Send(new DeletePositionTypeV3Command(id, GetClientId(), GetEntityId()));
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpGet("EmployeeTypeLookup")]
        public async Task<ActionResult<IEnumerable<EmploymentTypeLookupDto>>> GetEmployeeTypeLookup()
        {
            var result = await _mediator.Send(new GetEmploymentTypesV3LookupQuery());
            return Ok(result);
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
