using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Queries;

namespace Organization.Service.Controllers
{
    [ApiController]
    [Route("api/Designation/v3")]
    [Authorize]
    public sealed class DesignationV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;

        public DesignationV3Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<DesignationV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null)
        {
            var clientId = GetClientId();
            var query = new GetDesignationsV3Query(pageNumber, pageSize, searchTerm, clientId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DesignationV3Dto>> GetById(int id)
        {
            var clientId = GetClientId();
            var result = await _mediator.Send(new GetDesignationByIdV3Query(id, clientId));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<DesignationV3Dto>> Create([FromBody] CreateDesignationV3Request request)
        {
            try
            {
                var clientId = GetClientId();
                var command = new CreateDesignationV3Command(request, clientId);
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateDesignationV3Request request)
        {
            try
            {
                if (id != request.Id) return BadRequest("ID mismatch");
                var clientId = GetClientId();
                var result = await _mediator.Send(new UpdateDesignationV3Command(request, clientId));
                if (!result) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var clientId = GetClientId();
                var result = await _mediator.Send(new DeleteDesignationV3Command(id, clientId), HttpContext.RequestAborted);
                if (!result.Found) return NotFound();
                if (!result.Deleted) return BadRequest(new { error = result.Message("Designation"), usedIn = result.UsedIn });
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        private int GetClientId()
        {
            return RequestScopeHelper.GetClientId(this);
        }
    }
}
