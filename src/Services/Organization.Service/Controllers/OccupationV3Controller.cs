using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Queries;

namespace Organization.Service.Controllers
{
    [ApiController]
    [Route("api/Occupation/v3")]
    [Authorize]
    public sealed class OccupationV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;

        public OccupationV3Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<OccupationV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null)
        {
            var clientId = GetClientId();
            var query = new GetOccupationsV3Query(pageNumber, pageSize, searchTerm, clientId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OccupationV3Dto>> GetById(int id)
        {
            var clientId = GetClientId();
            var result = await _mediator.Send(new GetOccupationByIdV3Query(id, clientId));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<OccupationV3Dto>> Create([FromBody] CreateOccupationV3Request request)
        {
            var clientId = GetClientId();
            var result = await _mediator.Send(new CreateOccupationV3Command(request, clientId));
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateOccupationV3Request request)
        {
            if (id != request.Id) return BadRequest("ID mismatch");
            var clientId = GetClientId();
            var result = await _mediator.Send(new UpdateOccupationV3Command(request, clientId));
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var clientId = GetClientId();
            var result = await _mediator.Send(new DeleteOccupationV3Command(id, clientId));
            if (!result) return NotFound();
            return NoContent();
        }

        private int GetClientId()
        {
            return RequestScopeHelper.GetClientId(this);
        }
    }
}
