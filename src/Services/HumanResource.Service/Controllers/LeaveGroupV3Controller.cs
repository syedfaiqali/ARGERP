using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Route("api/LeaveGroup/v3")]
    public class LeaveGroupV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;

        public LeaveGroupV3Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? searchTerm, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var clientId = int.Parse(Request.Headers["X-Client-Id"].FirstOrDefault() ?? "1");
            var entityId = int.TryParse(Request.Headers["X-Entity-Id"].FirstOrDefault(), out var eId) ? eId : (int?)null;
            
            var result = await _mediator.Send(new GetLeaveGroupsV3Query(clientId, entityId, searchTerm, pageNumber, pageSize));
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var clientId = int.Parse(Request.Headers["X-Client-Id"].FirstOrDefault() ?? "1");
            var entityId = int.TryParse(Request.Headers["X-Entity-Id"].FirstOrDefault(), out var eId) ? eId : (int?)null;
            
            var result = await _mediator.Send(new GetLeaveGroupByIdV3Query(id, clientId, entityId));
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] LeaveGroupHeaderV3Dto dto)
        {
            var clientId = int.Parse(Request.Headers["X-Client-Id"].FirstOrDefault() ?? "1");
            var entityId = int.TryParse(Request.Headers["X-Entity-Id"].FirstOrDefault(), out var eId) ? eId : (int?)null;
            
            var id = await _mediator.Send(new CreateLeaveGroupV3Command(dto, clientId, entityId));
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] LeaveGroupHeaderV3Dto dto)
        {
            var clientId = int.Parse(Request.Headers["X-Client-Id"].FirstOrDefault() ?? "1");
            var entityId = int.TryParse(Request.Headers["X-Entity-Id"].FirstOrDefault(), out var eId) ? eId : (int?)null;
            
            var result = await _mediator.Send(new UpdateLeaveGroupV3Command(dto, clientId, entityId));
            return result ? Ok() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var clientId = int.Parse(Request.Headers["X-Client-Id"].FirstOrDefault() ?? "1");
            var entityId = int.TryParse(Request.Headers["X-Entity-Id"].FirstOrDefault(), out var eId) ? eId : (int?)null;
            
            var result = await _mediator.Send(new DeleteLeaveGroupV3Command(id, clientId, entityId));
            return result ? Ok() : NotFound();
        }
    }
}
