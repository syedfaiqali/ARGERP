using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Queries;
using HumanResource.Service.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Route("api/CalendarHoliday/v3")]
    [Route("api/HumanResource/CalendarHoliday/v3")]
    [Authorize]
    public class CalendarHolidayV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly HumanResourceDbContext _context;

        public CalendarHolidayV3Controller(IMediator mediator, HumanResourceDbContext context)
        {
            _mediator = mediator;
            _context = context;
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

            return 0;
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

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<CalendarHolidayV3Dto>>> GetCalendarHolidays([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100, [FromQuery] string? searchTerm = null)
        {
            var query = new GetCalendarHolidaysV3Query(pageNumber, pageSize, searchTerm, GetClientId(), GetEntityId());
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CalendarHolidayV3Dto>> GetCalendarHolidayById(int id)
        {
            var query = new GetCalendarHolidayByIdV3Query(id, GetClientId(), GetEntityId());
            var result = await _mediator.Send(query);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("copy-from/{sourceCalendarId:int}/payroll-period/{targetPayrollPeriodId:int}")]
        public async Task<IActionResult> CopyFrom(int sourceCalendarId, int targetPayrollPeriodId)
        {
            var clientId = GetClientId();
            var entityId = GetEntityId();
            var source = await _context.CalendarHolidays.AsNoTracking().Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.Id == sourceCalendarId && x.ClientId == clientId && !x.Utblock
                    && (!entityId.HasValue || x.EntityId == entityId.Value || x.EntityId == null));
            var target = await _context.PayrollPeriods.AsNoTracking().Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.Id == targetPayrollPeriodId && x.ClientId == clientId && !x.Utblock
                    && (!entityId.HasValue || x.EntityId == entityId.Value || x.EntityId == null));
            if (source == null || !source.Payperiodid.HasValue || target == null) return BadRequest(new { Message = "Copy From Detail not found." });

            var sourcePeriods = await _context.PayrollPeriodDetails.AsNoTracking().Where(x => x.Payperiodid == source.Payperiodid && !x.utblock).ToListAsync();
            var targetPeriods = target.Details.Where(x => !x.utblock).ToList();
            if (sourcePeriods.Count != targetPeriods.Count) return BadRequest(new { Message = "Payroll Period detail count must be equal to Copy From Period Detail count." });

            var copied = new List<object>();
            foreach (var detail in source.Details.Where(x => !x.Utblock && x.Payperioddtlid.HasValue))
            {
                var sourcePeriod = sourcePeriods.FirstOrDefault(x => x.Id == detail.Payperioddtlid);
                var targetPeriod = sourcePeriod == null ? null : targetPeriods.FirstOrDefault(x => x.Monthid == sourcePeriod.Monthid);
                if (sourcePeriod?.Startdate == null || sourcePeriod.Enddate == null || targetPeriod?.Startdate == null || targetPeriod.Enddate == null
                    || sourcePeriod.Startdate.Value.Month != targetPeriod.Startdate.Value.Month || sourcePeriod.Startdate.Value.Day != targetPeriod.Startdate.Value.Day
                    || sourcePeriod.Enddate.Value.Month != targetPeriod.Enddate.Value.Month || sourcePeriod.Enddate.Value.Day != targetPeriod.Enddate.Value.Day)
                    return BadRequest(new { Message = "Date not match." });

                copied.Add(new { holidayTypeId = detail.Holidaytypeid, description = detail.Ltxt ?? "", startDate = new DateTime(targetPeriod.Startdate.Value.Year, detail.Startdate!.Value.Month, detail.Startdate.Value.Day), endDate = new DateTime(targetPeriod.Enddate.Value.Year, detail.Enddate!.Value.Month, detail.Enddate.Value.Day), days = detail.Days, payPeriodDetailId = targetPeriod.Id, year = targetPeriod.Year, period = targetPeriod.Periodstxt ?? "" });
            }
            return Ok(copied);
        }

        [HttpPost]
        public async Task<ActionResult<CalendarHolidayV3Dto>> CreateCalendarHoliday(CreateCalendarHolidayV3Request request)
        {
            try
            {
                var command = new CreateCalendarHolidayV3Command(request, GetClientId(), GetEntityId());
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetCalendarHolidayById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<ActionResult<CalendarHolidayV3Dto>> UpdateCalendarHoliday(UpdateCalendarHolidayV3Request request)
        {
            try
            {
                var command = new UpdateCalendarHolidayV3Command(request, GetClientId(), GetEntityId());
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCalendarHoliday(int id)
        {
            try
            {
                var command = new DeleteCalendarHolidayV3Command(id, GetClientId(), GetEntityId());
                var success = await _mediator.Send(command);
                if (!success) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
