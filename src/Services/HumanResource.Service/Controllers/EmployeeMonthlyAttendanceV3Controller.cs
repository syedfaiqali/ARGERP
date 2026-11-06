using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Route("api/EmployeeMonthlyAttendance/v3")]
    [Authorize]
    public sealed class EmployeeMonthlyAttendanceV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeeMonthlyAttendanceV3Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("process")]
        public async Task<ActionResult<IReadOnlyList<EmployeeMonthlyAttendanceV3Dto>>> Process(
            [FromQuery] int periodDetailId,
            [FromQuery] int? siteId = null,
            [FromQuery] int? departmentId = null,
            [FromQuery] int? designationId = null,
            [FromQuery] int? employeeId = null)
        {
            var result = await _mediator.Send(new GetEmployeeMonthlyAttendanceProcessV3Query(
                GetClientId(),
                GetEntityId(),
                periodDetailId,
                siteId,
                departmentId,
                designationId,
                employeeId));

            return Ok(result);
        }

        public sealed record RecalculateRequest(int EmployeeId, DateTime From, DateTime To);

        [HttpPost("recalculate")]
        public async Task<ActionResult> Recalculate(RecalculateRequest request,
            [FromServices] HumanResource.Service.Infrastructure.Data.HumanResourceDbContext db,
            CancellationToken ct)
        {
            if (request.EmployeeId <= 0 || request.From == default || request.To < request.From)
                return BadRequest(new { error = "Employee and a valid date range are required." });
            var client = GetClientId(); var entity = GetEntityId();
            var periods = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.ToListAsync(
                db.PayrollPeriodDetails.Where(x => x.Header != null && x.Header.ClientId == client && !x.Header.Utblock && !x.utblock
                    && (!entity.HasValue || x.Header.EntityId == entity || x.Header.EntityId == null)
                    && x.Startdate <= request.To && x.Enddate >= request.From).Select(x => x.Id), ct);
            var updated = 0;
            foreach (var period in periods)
            {
                var rows = await _mediator.Send(new GetEmployeeMonthlyAttendanceProcessV3Query(client, entity, period, null, null, null, request.EmployeeId), ct);
                var items = rows.Select(x => new EmployeeMonthlyAttendanceUpsertV3Request(x.EmployeeId, x.RegisterId, x.PayPeriodId,
                    x.PayPeriodDetailId, x.StartDate, x.EndDate, x.DisbDate, x.EntityId, x.Days, x.PresentsCount, x.AbsentsCount,
                    x.HalfDaysCount, x.LateCount, x.EarlyCounts, x.ActualDays, x.ActualPresent, x.Holidays, x.Leaves)).ToList();
                if (items.Count > 0) { await _mediator.Send(new SaveEmployeeMonthlyAttendanceV3Command(items, client, entity), ct); updated += items.Count; }
            }
            return Ok(new { updated });
        }

        [HttpPost("bulk")]
        public async Task<ActionResult> Save([FromBody] SaveEmployeeMonthlyAttendanceV3Request request)
        {
            if (request?.Items == null || request.Items.Count == 0)
            {
                return BadRequest("Monthly attendance data is required.");
            }

            var success = await _mediator.Send(new SaveEmployeeMonthlyAttendanceV3Command(
                request.Items,
                GetClientId(),
                GetEntityId()));

            return Ok(success);
        }

        [HttpDelete("revert")]
        public async Task<ActionResult> Revert(
            [FromQuery] int employeeId,
            [FromQuery] int registerId,
            [FromQuery] int periodDetailId)
        {
            var success = await _mediator.Send(new RevertEmployeeMonthlyAttendanceV3Command(
                employeeId,
                registerId,
                periodDetailId,
                GetClientId(),
                GetEntityId()));

            if (!success) return NotFound();
            return Ok(success);
        }

        [HttpGet("pay-period-details")]
        public async Task<ActionResult<IReadOnlyList<PayrollPeriodDetailLookupV3Dto>>> GetPayPeriodDetails([FromQuery] int registerId)
        {
            var result = await _mediator.Send(new GetPayrollPeriodDetailsByRegisterV3Query(
                GetClientId(),
                GetEntityId(),
                registerId));

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
