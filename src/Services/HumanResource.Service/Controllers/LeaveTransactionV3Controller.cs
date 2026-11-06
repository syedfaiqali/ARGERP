using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HumanResource.Service.Controllers
{
    [Route("api/[controller]/v3")]
    [ApiController]
    [Authorize]
    public class LeaveTransactionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LeaveTransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private int? GetClientId()
        {
            if (Request.Headers.TryGetValue("X-Client-Id", out var clientIdValues))
            {
                if (int.TryParse(clientIdValues.FirstOrDefault(), out int clientId))
                    return clientId;
            }
            return null; // fallback or error handling depending on your global policy
        }

        private int? GetEntityId()
        {
            if (Request.Headers.TryGetValue("X-Entity-Id", out var entityIdValues))
            {
                if (int.TryParse(entityIdValues.FirstOrDefault(), out int entityId))
                    return entityId;
            }
            return null;
        }

        private int? GetUserId()
        {
            var claimValue =
                User.FindFirst("userId")?.Value ??
                User.FindFirst("userid")?.Value ??
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                User.FindFirst("sub")?.Value;

            return int.TryParse(claimValue, out var userId) && userId > 0
                ? userId
                : null;
        }

        [HttpGet]
        public async Task<IActionResult> GetList()
        {
            var result = await _mediator.Send(new GetLeaveTransactionListV3Query(GetEntityId(), GetClientId()));
            return Ok(new { success = true, data = result });
        }

        [HttpGet("scoped")]
        public async Task<IActionResult> GetScopedList(
            [FromQuery] int? referenceEmployeeId = null,
            [FromQuery] bool includeAttachments = true)
        {
            var result = await _mediator.Send(new GetScopedLeaveTransactionListV3Query(
                GetEntityId(),
                GetClientId(),
                GetUserId(),
                referenceEmployeeId,
                includeAttachments));

            return Ok(new { success = true, data = result });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetLeaveTransactionByIdV3Query(id));
            if (result == null) return NotFound(new { success = false, message = "Record not found" });
            return Ok(new { success = true, data = result });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLeaveTransactionV3Dto dto)
        {
            try
            {
                var result = await _mediator.Send(new CreateLeaveTransactionV3Command(dto, GetEntityId(), GetClientId()));
                return Ok(new { success = true, message = "Leave Transaction created successfully", data = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateLeaveTransactionV3Dto dto)
        {
            try
            {
                var result = await _mediator.Send(new UpdateLeaveTransactionV3Command(id, dto));
                if (!result) return NotFound(new { success = false, message = "Record not found" });
                return Ok(new { success = true, message = "Leave Transaction updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteLeaveTransactionV3Command(id));
            if (!result) return NotFound(new { success = false, message = "Record not found" });
            return Ok(new { success = true, message = "Leave Transaction deleted successfully" });
        }
    }
}
