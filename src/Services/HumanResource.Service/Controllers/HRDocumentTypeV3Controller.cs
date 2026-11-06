using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Route("api/HRDocumentType/v3")]
    [Route("api/HumanResource/HRDocumentType/v3")]
    [Authorize]
    public class HRDocumentTypeV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;

        public HRDocumentTypeV3Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        private int GetClientId()
        {
            if (int.TryParse(Request.Headers["X-Client-Id"].FirstOrDefault(), out var clientId))
                return clientId;

            return int.TryParse(User.FindFirst("client_id")?.Value, out var claimClientId) ? claimClientId : 0;
        }

        private int GetEntityId()
        {
            if (int.TryParse(Request.Headers["X-Entity-Id"].FirstOrDefault(), out var entityId))
                return entityId;

            return int.TryParse(User.FindFirst("entity_id")?.Value, out var claimEntityId) ? claimEntityId : 0;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, [FromQuery] string? search = null)
        {
            var query = new GetDocumentTypesV3Query(GetClientId(), GetEntityId(), pageNumber, pageSize, search);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var query = new GetDocumentTypeByIdV3Query(GetClientId(), GetEntityId(), id);
            var result = await _mediator.Send(query);
            
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDocumentTypeV3Request request)
        {
            var command = new CreateDocumentTypeV3Command(
                request.Code,
                request.DocumentType,
                request.Description,
                request.IsActive,
                GetClientId(),
                GetEntityId()
            );

            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDocumentTypeV3Request request)
        {
            if (id != request.Id)
            {
                return BadRequest("ID mismatch");
            }

            var command = new UpdateDocumentTypeV3Command(
                request.Id,
                request.Code,
                request.DocumentType,
                request.Description,
                request.IsActive,
                GetClientId(),
                GetEntityId()
            );

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteDocumentTypeV3Command(id, GetClientId(), GetEntityId());

            try
            {
                var result = await _mediator.Send(command);
                if (result)
                {
                    return NoContent();
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
