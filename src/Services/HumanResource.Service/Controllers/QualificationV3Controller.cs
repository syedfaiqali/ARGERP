using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Queries;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Route("api/Qualification/v3")]
    [Route("api/HumanResource/Qualification/v3")]
    [Authorize]
    public sealed class QualificationV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;
        private const int HumanResourceLovTypeId = 368;

        public QualificationV3Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<QualificationV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? lovtypeId = HumanResourceLovTypeId)
        {
            var result = await _mediator.Send(new GetQualificationsV3Query(pageNumber, pageSize, GetClientId(), GetEntityId(), lovtypeId, searchTerm));
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<QualificationV3Dto>> GetById(int id)
        {
            var result = await _mediator.Send(new GetQualificationByIdV3Query(id, GetClientId(), GetEntityId()));
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<QualificationV3Dto>> Create([FromBody] CreateQualificationV3Request request)
        {
            try
            {
                var result = await _mediator.Send(new CreateQualificationV3Command(request, GetClientId(), GetEntityId()));
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int? id, [FromBody] UpdateQualificationV3Request request)
        {
            try
            {
                if (id.HasValue && id.Value != request.Id) request = request with { Id = id.Value };
                var updated = await _mediator.Send(new UpdateQualificationV3Command(request, GetClientId(), GetEntityId()));
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
            var deleted = await _mediator.Send(new DeleteQualificationV3Command(id, GetClientId(), GetEntityId()));
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


