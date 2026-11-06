using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Queries;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Route("api/QualificationType/v3")]
    [Route("api/HumanResource/QualificationType/v3")]
    [Authorize]
    public sealed class QualificationTypeV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;

        public QualificationTypeV3Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<QualificationTypeV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] string? searchTerm = null)
        {
            var result = await _mediator.Send(new GetQualificationTypesV3Query(pageNumber, pageSize, GetClientId(), GetEntityId(), searchTerm));
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<QualificationTypeV3Dto>> GetById(int id)
        {
            var result = await _mediator.Send(new GetQualificationTypeByIdV3Query(id, GetClientId(), GetEntityId()));
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<QualificationTypeV3Dto>> Create([FromBody] CreateQualificationTypeV3Request request)
        {
            try
            {
                var result = await _mediator.Send(new CreateQualificationTypeV3Command(request, GetClientId(), GetEntityId()));
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int? id, [FromBody] UpdateQualificationTypeV3Request request)
        {
            try
            {
                if (id.HasValue && id.Value != request.Id) request = request with { Id = id.Value };
                var updated = await _mediator.Send(new UpdateQualificationTypeV3Command(request, GetClientId(), GetEntityId()));
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
                var deleted = await _mediator.Send(new DeleteQualificationTypeV3Command(id, GetClientId(), GetEntityId()));
                if (!deleted)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
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
    }
}
