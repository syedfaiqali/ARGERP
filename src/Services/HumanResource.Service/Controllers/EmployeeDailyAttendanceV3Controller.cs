using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Route("api/HumanResource/EmployeeDailyAttendance/v3")]
    [Route("api/EmployeeDailyAttendance/v3")]
    [Authorize]
    public sealed class EmployeeDailyAttendanceV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly HumanResourceDbContext _context;

        public EmployeeDailyAttendanceV3Controller(IMediator mediator, HumanResourceDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<IReadOnlyList<EmployeeAttendanceDashboardMarkDto>>> GetDashboardMarks(
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime tillDate)
        {
            var startDate = fromDate.Date;
            var endDate = tillDate.Date;
            if (endDate < startDate) return BadRequest("Till date must be on or after from date.");
            if ((endDate - startDate).TotalDays > 62) return BadRequest("Dashboard attendance is limited to 63 days.");

            var endExclusive = endDate.AddDays(1);
            var clientId = GetClientId();
            var entityId = GetEntityId();

            // Resolve the small employee scope once. Both attendance reads can then
            // filter directly by employee/date instead of joining attendance history.
            var employeeIds = await _context.Employees.AsNoTracking()
                .Where(employee => employee.ClientId == clientId
                    && (!entityId.HasValue || employee.EntityId == entityId.Value))
                .Select(employee => employee.Id)
                .ToArrayAsync(HttpContext.RequestAborted);
            if (employeeIds.Length == 0)
                return Ok(Array.Empty<EmployeeAttendanceDashboardMarkDto>());

            // The dashboard needs only compact daily marks. The full attendance endpoint
            // also builds rosters, schedules and missing rows for every employee/date,
            // which is appropriate for attendance maintenance but too expensive here.
            var savedMarks = await (
                from daily in _context.DailyAttendances.AsNoTracking()
                where daily.EmployeeId.HasValue
                      && employeeIds.Contains(daily.EmployeeId.Value)
                      && (!entityId.HasValue || daily.EntityId == entityId.Value || daily.EntityId == null)
                      && daily.AtnDate.HasValue
                      && daily.AtnDate.Value >= startDate
                      && daily.AtnDate.Value < endExclusive
                select new EmployeeAttendanceDashboardMarkDto(
                    daily.Id,
                    daily.EmployeeId!.Value,
                    daily.AtnDate!.Value,
                    daily.IsAbsent,
                    daily.IsHoliday,
                    daily.LateMin ?? 0,
                    daily.ActShiftIn,
                    daily.ActShitOut))
                .ToListAsync(HttpContext.RequestAborted);

            var savedKeys = savedMarks
                .Select(mark => $"{mark.EmployeeId}:{mark.AtnDate:yyyyMMdd}")
                .ToHashSet();

            var rawRows = await (
                from attendance in _context.EmployeeAttendances.AsNoTracking()
                where attendance.EmpId.HasValue
                      && employeeIds.Contains(attendance.EmpId.Value)
                      && (attendance.ClientId == clientId || attendance.ClientId == null)
                      && attendance.UtBlock == false
                      && attendance.Dttime.HasValue
                      && attendance.Dttime.Value >= startDate
                      && attendance.Dttime.Value < endExclusive
                select new
                {
                    attendance.Id,
                    EmployeeId = attendance.EmpId!.Value,
                    PunchTime = attendance.Dttime!.Value,
                    attendance.AtnActivityId,
                    LateMin = attendance.LateMin ?? 0
                })
                .ToListAsync(HttpContext.RequestAborted);

            var rawMarks = rawRows
                .GroupBy(row => new { row.EmployeeId, Date = row.PunchTime.Date })
                .Where(group => !savedKeys.Contains($"{group.Key.EmployeeId}:{group.Key.Date:yyyyMMdd}"))
                .Select(group =>
                {
                    var ordered = group.OrderBy(row => row.PunchTime).ToList();
                    var checkIn = ordered.FirstOrDefault(row => row.AtnActivityId == 1)?.PunchTime
                        ?? ordered.FirstOrDefault(row => row.PunchTime.TimeOfDay < TimeSpan.FromHours(12))?.PunchTime;
                    var checkOut = ordered.LastOrDefault(row => row.AtnActivityId == 2)?.PunchTime
                        ?? ordered.LastOrDefault(row => row.PunchTime.TimeOfDay >= TimeSpan.FromHours(12))?.PunchTime;
                    var lateMinutes = ordered.FirstOrDefault(row => row.AtnActivityId == 1 && row.LateMin > 0)?.LateMin
                        ?? ordered.FirstOrDefault(row => row.LateMin > 0)?.LateMin
                        ?? 0;

                    return new EmployeeAttendanceDashboardMarkDto(
                        ordered.Max(row => (long)row.Id),
                        group.Key.EmployeeId,
                        group.Key.Date,
                        checkIn == null && checkOut == null,
                        false,
                        lateMinutes,
                        checkIn,
                        checkOut);
                });

            return Ok(savedMarks.Concat(rawMarks)
                .OrderBy(mark => mark.AtnDate)
                .ThenBy(mark => mark.EmployeeId)
                .ToList());
        }

        [HttpGet("dashboard/leaves")]
        public async Task<ActionResult<IReadOnlyList<int>>> GetDashboardLeaveEmployeeIds([FromQuery] DateTime date)
        {
            var selectedDate = date.Date;
            var clientId = GetClientId();
            var entityId = GetEntityId();
            var employeeIds = await (
                from leave in _context.LeaveTransactions.AsNoTracking()
                join employee in _context.Employees.AsNoTracking() on leave.EmployeeId equals employee.Id
                where employee.ClientId == clientId
                      && (!entityId.HasValue || employee.EntityId == entityId.Value)
                      && (leave.ClientId == clientId || leave.ClientId == null)
                      && (!entityId.HasValue || leave.EntityId == entityId.Value || leave.EntityId == null)
                      && !leave.UtBlock
                      && leave.StatusId == 582
                      && ((leave.LeaveDays ?? 0) < 3 || (leave.AdmApproval.HasValue && leave.AdmApproval.Value != 0))
                      && leave.FromDate.HasValue && leave.FromDate.Value < selectedDate.AddDays(1)
                      && leave.TillDate.HasValue && leave.TillDate.Value >= selectedDate
                select employee.Id)
                .Distinct()
                .ToListAsync(HttpContext.RequestAborted);

            return Ok(employeeIds);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<EmployeeDailyAttendanceV3Dto>>> GetAll(
            [FromQuery] int? employeeId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? tillDate = null,
            [FromQuery] int? departmentId = null,
            [FromQuery] int? designationId = null)
        {
            var entityId = GetEntityId();
            var result = await _mediator.Send(new GetEmployeeDailyAttendanceV3Query(GetClientId(), entityId, employeeId, fromDate, tillDate, departmentId, designationId));
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeDailyAttendanceV3Dto>> GetById(int id)
        {
            var entityId = GetEntityId();
            var result = await _mediator.Send(new GetEmployeeDailyAttendanceByIdV3Query(id, GetClientId(), entityId));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeDailyAttendanceV3Dto>> Create([FromBody] CreateEmployeeDailyAttendanceV3Request request)
        {
            var entityId = GetEntityId();
            try
            {
                var result = await _mediator.Send(new CreateEmployeeDailyAttendanceV3Command(request, GetClientId(), entityId));
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("bulk")]
        public async Task<ActionResult<IReadOnlyList<EmployeeDailyAttendanceV3Dto>>> BulkSave([FromBody] BulkSaveEmployeeDailyAttendanceV3Request request)
        {
            var entityId = GetEntityId();
            try
            {
                var result = await _mediator.Send(new BulkSaveEmployeeDailyAttendanceV3Command(request, GetClientId(), entityId));
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeDailyAttendanceV3Request request)
        {
            if (id != request.Id) return BadRequest("Invalid attendance id.");
            var entityId = GetEntityId();
            try
            {
                var result = await _mediator.Send(new UpdateEmployeeDailyAttendanceV3Command(request, GetClientId(), entityId));
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entityId = GetEntityId();
            var deleted = await _mediator.Send(new DeleteEmployeeDailyAttendanceV3Command(id, GetClientId(), entityId));
            if (!deleted) return NotFound();
            return NoContent();
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
            if (Request.Headers.TryGetValue("X-Campus-Id", out var campusId)
                && int.TryParse(campusId, out var parsedCampusId)) return parsedCampusId;
            if (Request.Headers.TryGetValue("Campus-Id", out var campusIdAlias)
                && int.TryParse(campusIdAlias, out var parsedCampusIdAlias)) return parsedCampusIdAlias;
            var entityIdClaim = User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if (entityIdClaim != null && int.TryParse(entityIdClaim.Value, out var claimEntityId)) return claimEntityId;
            if (Request.Headers.TryGetValue("X-Entity-Id", out var headerEntityId) && int.TryParse(headerEntityId, out var parsedHeaderEntityId)) return parsedHeaderEntityId;
            return null;
        }
    }
}
