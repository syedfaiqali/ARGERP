using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Route("api/Schedule/v3")]
    public sealed class ScheduleV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;

        public ScheduleV3Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        private int GetClientId()
        {
            if (Request.Headers.TryGetValue("X-Client-Id", out var values)
                && int.TryParse(values.FirstOrDefault(), out var id))
            {
                return id;
            }
            return 0;
        }

        private int? GetEntityId()
        {
            if (Request.Headers.TryGetValue("X-Campus-Id", out var values)
                && int.TryParse(values.FirstOrDefault(), out var id))
            {
                return id;
            }
            if (Request.Headers.TryGetValue("Campus-Id", out var values2)
                && int.TryParse(values2.FirstOrDefault(), out var id2))
            {
                return id2;
            }
            return null;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<ScheduleV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 1000,
            [FromQuery] string? searchTerm = null)
        {
            var result = await _mediator.Send(new GetSchedulesV3Query(pageNumber, pageSize, GetClientId(), GetEntityId(), searchTerm));
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ScheduleHeaderV3Dto>> GetById(int id)
        {
            var result = await _mediator.Send(new GetScheduleByIdV3Query(id, GetClientId(), GetEntityId()));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ScheduleHeaderV3Dto>> Create([FromBody] CreateScheduleV3Request request)
        {
            try
            {
                var created = await _mediator.Send(new CreateScheduleV3Command(request, GetClientId(), GetEntityId()));
                return Ok(created);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
        }

        [HttpPut]
        public async Task<ActionResult> Update([FromBody] UpdateScheduleV3Request request)
        {
            try
            {
                var updated = await _mediator.Send(new UpdateScheduleV3Command(request, GetClientId(), GetEntityId()));
                if (!updated) return NotFound();
                return Ok(updated);
            }
            catch (InvalidOperationException exception)
            {
                return BadRequest(new { message = exception.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _mediator.Send(new DeleteScheduleV3Command(id, GetClientId(), GetEntityId()));
            if (!deleted) return NotFound();
            return Ok(deleted);
        }

        [HttpGet("details")]
        public async Task<ActionResult<IReadOnlyList<ScheduleDetailV3Dto>>> GetDetails([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            var result = await _mediator.Send(new GetScheduleDetailsV3Query(fromDate, toDate));
            return Ok(result);
        }
    }
}
