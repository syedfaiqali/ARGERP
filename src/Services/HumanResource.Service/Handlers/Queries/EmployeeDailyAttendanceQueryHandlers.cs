using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using HumanResource.Service.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class EmployeeDailyAttendanceQueryHandlers :
        IRequestHandler<GetEmployeeDailyAttendanceV3Query, IReadOnlyList<EmployeeDailyAttendanceV3Dto>>,
        IRequestHandler<GetEmployeeDailyAttendanceByIdV3Query, EmployeeDailyAttendanceV3Dto?>
    {
        private readonly HumanResourceDbContext _context;
        private const int CheckInActivityId = 1;
        private const int CheckOutActivityId = 2;
        public EmployeeDailyAttendanceQueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<EmployeeDailyAttendanceV3Dto>> Handle(GetEmployeeDailyAttendanceV3Query request, CancellationToken cancellationToken)
        {
            var query =
                from daily in _context.DailyAttendances.AsNoTracking()
                join emp in _context.Employees.AsNoTracking() on daily.EmployeeId equals emp.Id
                where daily.UtBlock == false && emp.ClientId == request.ClientId
                       && (request.EntityId == null || emp.EntityId == request.EntityId)
                       && (request.EntityId == null || daily.EntityId == request.EntityId || daily.EntityId == null)
                       && daily.AtnDate.HasValue
                       && _context.EmployeePositions.Any(position =>
                           position.EmployeeId == daily.EmployeeId &&
                           (position.UtBlock == null || position.UtBlock == false))
                select new { daily, emp };

            if (request.EmployeeId.HasValue && request.EmployeeId.Value > 0)
            {
                query = query.Where(x => x.daily.EmployeeId == request.EmployeeId.Value);
            }

            if (request.DepartmentId.HasValue && request.DepartmentId.Value > 0)
            {
                query = query.Where(x => _context.EmployeePositions.Any(position =>
                    position.EmployeeId == x.emp.Id &&
                    position.DepartmentId == request.DepartmentId.Value &&
                    (position.UtBlock == null || position.UtBlock == false)));
            }

            if (request.DesignationId.HasValue && request.DesignationId.Value > 0)
            {
                query = query.Where(x => _context.EmployeePositions.Any(position =>
                    position.EmployeeId == x.emp.Id &&
                    position.DesignationId == request.DesignationId.Value &&
                    (position.UtBlock == null || position.UtBlock == false)));
            }

            var effectiveFromDate = request.FromDate?.Date;
            var effectiveTillDate = request.TillDate?.Date;

            if (!effectiveFromDate.HasValue && !effectiveTillDate.HasValue)
            {
                var today = DateTime.Today;
                effectiveFromDate = new DateTime(today.Year, 1, 1);
                effectiveTillDate = new DateTime(today.Year, 12, 31);
            }
            else if (effectiveFromDate.HasValue && !effectiveTillDate.HasValue)
            {
                effectiveTillDate = effectiveFromDate.Value.AddYears(1).AddDays(-1);
            }
            else if (!effectiveFromDate.HasValue && effectiveTillDate.HasValue)
            {
                effectiveFromDate = new DateTime(effectiveTillDate.Value.Year, 1, 1);
            }

            query = query.Where(x =>
                x.daily.AtnDate.HasValue &&
                x.daily.AtnDate.Value >= effectiveFromDate!.Value &&
                x.daily.AtnDate.Value < effectiveTillDate!.Value.AddDays(1) &&
                _context.EmployeeRosters.Any(roster =>
                    roster.EmployeeId == x.daily.EmployeeId &&
                    (roster.UtBlock == null || roster.UtBlock == false) &&
                    roster.StartDate.HasValue &&
                    roster.StartDate.Value.Date <= x.daily.AtnDate.Value.Date &&
                    (!roster.EndDate.HasValue ||
                     roster.EndDate.Value.Date >= x.daily.AtnDate.Value.Date)));

            var items = await query
                .OrderByDescending(x => x.daily.AtnDate)
                .Select(x => new EmployeeDailyAttendanceV3Dto(
                    x.daily.Id,
                    x.daily.EmployeeId,
                    x.emp.EmployeeCode ?? string.Empty,
                    $"{x.emp.FirstName ?? string.Empty} {x.emp.LastName ?? string.Empty}".Trim().ToUpperInvariant(),
                    x.daily.AtnDate,
                    x.daily.AtnDate.HasValue ? x.daily.AtnDate.Value.ToString("dddd") : string.Empty,
                    x.daily.SchTimeIn,
                    x.daily.SchTimeOut,
                    x.daily.ActShiftIn,
                    x.daily.ActShitOut,
                    x.daily.LateMin,
                    x.daily.LateCount,
                    x.daily.LateHalfDays,
                    x.daily.LateAbsent,
                    x.daily.LateExcuse,
                    x.daily.EdMin,
                    x.daily.EdCount,
                    x.daily.EdHalfDays,
                    x.daily.EdAbsent,
                    x.daily.EdExcuse,
                    x.daily.OtMin,
                    x.daily.OtCount,
                    x.daily.OtHalfDays,
                    x.daily.Remarks ?? string.Empty,
                    x.daily.IsAbsent,
                    x.daily.IsHoliday,
                    x.daily.ShiftId,
                    x.daily.ShiftDtlId,
                    x.daily.DayId,
                    x.daily.DepId,
                    x.daily.DesignationId
                ))
                .ToListAsync(cancellationToken);

            var savedKeys = items
                .Where(x => x.EmployeeId.HasValue && x.AtnDate.HasValue)
                .Select(x => $"{x.EmployeeId!.Value}:{x.AtnDate!.Value.Date:yyyyMMdd}")
                .ToHashSet();

            var rawQuery =
                from atn in _context.EmployeeAttendances.AsNoTracking()
                join emp in _context.Employees.AsNoTracking() on atn.EmpId equals emp.Id
                where emp.ClientId == request.ClientId
                      && atn.UtBlock == false
                      && atn.Dttime.HasValue
                      && _context.EmployeePositions.Any(position =>
                          position.EmployeeId == atn.EmpId &&
                          (position.UtBlock == null || position.UtBlock == false))
                      && (atn.ClientId == request.ClientId || atn.ClientId == null)
                      && (!request.EntityId.HasValue || emp.EntityId == request.EntityId.Value)
                select new { atn, emp };

            if (request.EmployeeId.HasValue && request.EmployeeId.Value > 0)
            {
                rawQuery = rawQuery.Where(x => x.atn.EmpId == request.EmployeeId.Value);
            }

            if (request.DepartmentId.HasValue && request.DepartmentId.Value > 0)
            {
                rawQuery = rawQuery.Where(x => _context.EmployeePositions.Any(position =>
                    position.EmployeeId == x.emp.Id &&
                    position.DepartmentId == request.DepartmentId.Value &&
                    (position.UtBlock == null || position.UtBlock == false)));
            }

            if (request.DesignationId.HasValue && request.DesignationId.Value > 0)
            {
                rawQuery = rawQuery.Where(x => _context.EmployeePositions.Any(position =>
                    position.EmployeeId == x.emp.Id &&
                    position.DesignationId == request.DesignationId.Value &&
                    (position.UtBlock == null || position.UtBlock == false)));
            }

            rawQuery = rawQuery.Where(x =>
                x.atn.Dttime!.Value >= effectiveFromDate!.Value &&
                x.atn.Dttime!.Value < effectiveTillDate!.Value.AddDays(1) &&
                _context.EmployeeRosters.Any(roster =>
                    roster.EmployeeId == x.atn.EmpId &&
                    (roster.UtBlock == null || roster.UtBlock == false) &&
                    roster.StartDate.HasValue &&
                    roster.StartDate.Value.Date <= x.atn.Dttime.Value.Date &&
                    (!roster.EndDate.HasValue ||
                     roster.EndDate.Value.Date >= x.atn.Dttime.Value.Date)));

            var rawRows = await rawQuery
                .OrderBy(x => x.atn.Dttime)
                .Select(x => new
                {
                    x.atn.Id,
                    x.atn.EmpId,
                    x.emp.EmployeeCode,
                    x.emp.FirstName,
                    x.emp.LastName,
                    EmployeeShiftId = x.emp.ShiftId,
                    x.atn.Dttime,
                    x.atn.AtnActivityId,
                    x.atn.ShiftIn,
                    x.atn.ShiftOut,
                    x.atn.SchTimeIn,
                    x.atn.SchTimeOut,
                    x.atn.ShiftId,
                    x.atn.ShiftDtlId,
                    x.atn.DayId,
                    x.atn.LateMin,
                    x.atn.LateCount,
                    x.atn.EdMin,
                    x.atn.EdCount,
                    x.atn.OtMin,
                    x.atn.OtCount,
                    x.emp.DepartmentId,
                    x.emp.DesignationId,
                    x.atn.Remarks
                })
                .ToListAsync(cancellationToken);

            var derivedItems = rawRows
                .Where(x => x.EmpId.HasValue && x.Dttime.HasValue)
                .GroupBy(x => new { EmployeeId = x.EmpId!.Value, AtnDate = x.Dttime!.Value.Date })
                .Where(g => !savedKeys.Contains($"{g.Key.EmployeeId}:{g.Key.AtnDate:yyyyMMdd}"))
                .Select(g =>
                {
                    var ordered = g.OrderBy(x => x.Dttime).ToList();
                    var first = ordered.First();
                    var checkIn = ResolveCheckInPunch(ordered);
                    var checkOut = ResolveCheckOutPunch(ordered);
                    var schIn = ordered.FirstOrDefault(x => x.SchTimeIn.HasValue)?.SchTimeIn
                        ?? ordered.FirstOrDefault(x => x.ShiftIn.HasValue)?.ShiftIn;
                    var schOut = ordered.FirstOrDefault(x => x.SchTimeOut.HasValue)?.SchTimeOut
                        ?? ordered.FirstOrDefault(x => x.ShiftOut.HasValue)?.ShiftOut;

                    return new EmployeeDailyAttendanceV3Dto(
                        CreateSyntheticDailyAttendanceId(g.Key.EmployeeId, g.Key.AtnDate),
                        g.Key.EmployeeId,
                        first.EmployeeCode ?? string.Empty,
                        $"{first.FirstName ?? string.Empty} {first.LastName ?? string.Empty}".Trim().ToUpperInvariant(),
                        g.Key.AtnDate,
                        g.Key.AtnDate.ToString("dddd"),
                        schIn,
                        schOut,
                        checkIn,
                        checkOut,
                        ordered.FirstOrDefault(x => x.AtnActivityId == CheckInActivityId && x.LateMin.HasValue)?.LateMin ?? ordered.FirstOrDefault(x => x.LateMin.HasValue)?.LateMin,
                        ordered.FirstOrDefault(x => x.AtnActivityId == CheckInActivityId && x.LateCount.HasValue)?.LateCount ?? ordered.FirstOrDefault(x => x.LateCount.HasValue)?.LateCount,
                        null,
                        null,
                        null,
                        ordered.FirstOrDefault(x => x.AtnActivityId == CheckOutActivityId && x.EdMin.HasValue)?.EdMin ?? ordered.FirstOrDefault(x => x.EdMin.HasValue)?.EdMin,
                        ordered.FirstOrDefault(x => x.AtnActivityId == CheckOutActivityId && x.EdCount.HasValue)?.EdCount ?? ordered.FirstOrDefault(x => x.EdCount.HasValue)?.EdCount,
                        null,
                        null,
                        null,
                        ordered.FirstOrDefault(x => x.AtnActivityId == CheckOutActivityId && x.OtMin.HasValue)?.OtMin ?? ordered.FirstOrDefault(x => x.OtMin.HasValue)?.OtMin,
                        ordered.FirstOrDefault(x => x.AtnActivityId == CheckOutActivityId && x.OtCount.HasValue)?.OtCount ?? ordered.FirstOrDefault(x => x.OtCount.HasValue)?.OtCount,
                        null,
                        string.Join("; ", ordered.Select(x => x.Remarks).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct()),
                        checkIn == null && checkOut == null,
                        false,
                        ordered.FirstOrDefault(x => x.ShiftId.HasValue)?.ShiftId ?? first.EmployeeShiftId,
                        ordered.FirstOrDefault(x => x.ShiftDtlId.HasValue)?.ShiftDtlId,
                        ordered.FirstOrDefault(x => x.DayId.HasValue)?.DayId ?? ToLegacyDayId(g.Key.AtnDate.DayOfWeek),
                        first.DepartmentId,
                        first.DesignationId
                    );
                })
                .ToList();

            var existingKeys = savedKeys
                .Concat(derivedItems
                    .Where(x => x.EmployeeId.HasValue && x.AtnDate.HasValue)
                    .Select(x => $"{x.EmployeeId!.Value}:{x.AtnDate!.Value.Date:yyyyMMdd}"))
                .ToHashSet();

            var employeeQuery = _context.Employees.AsNoTracking()
                .Where(emp =>
                    emp.ClientId == request.ClientId &&
                    (!request.EntityId.HasValue || emp.EntityId == request.EntityId.Value) &&
                    (!request.EmployeeId.HasValue || request.EmployeeId.Value == 0 || emp.Id == request.EmployeeId.Value) &&
                    _context.EmployeeRosters.Any(roster =>
                        roster.EmployeeId == emp.Id &&
                        (roster.UtBlock == null || roster.UtBlock == false) &&
                        roster.StartDate.HasValue &&
                        roster.StartDate.Value.Date <= effectiveTillDate!.Value &&
                        (!roster.EndDate.HasValue ||
                         roster.EndDate.Value.Date >= effectiveFromDate!.Value)) &&
                    _context.EmployeePositions.Any(position =>
                        position.EmployeeId == emp.Id &&
                        (position.UtBlock == null || position.UtBlock == false) &&
                        (!request.DepartmentId.HasValue || request.DepartmentId.Value == 0 || position.DepartmentId == request.DepartmentId.Value) &&
                        (!request.DesignationId.HasValue || request.DesignationId.Value == 0 || position.DesignationId == request.DesignationId.Value)));

            var employees = await employeeQuery
                .Select(emp => new
                {
                    emp.Id,
                    emp.EmployeeCode,
                    emp.FirstName,
                    emp.LastName,
                    emp.JoiningDate,
                    emp.ShiftId,
                    emp.DepartmentId,
                    emp.DesignationId
                })
                .ToListAsync(cancellationToken);

            var employeeIds = employees.Select(x => x.Id)
                .Concat(rawRows.Where(x => x.EmpId.HasValue).Select(x => x.EmpId!.Value))
                .Concat(items.Where(x => x.EmployeeId.HasValue).Select(x => x.EmployeeId!.Value))
                .Distinct()
                .ToList();
            var rosters = await _context.EmployeeRosters.AsNoTracking()
                .Where(roster =>
                    roster.EmployeeId.HasValue &&
                    employeeIds.Contains(roster.EmployeeId.Value) &&
                    (roster.UtBlock == null || roster.UtBlock == false) &&
                    roster.StartDate.HasValue &&
                    roster.StartDate.Value.Date <= effectiveTillDate!.Value &&
                    (!roster.EndDate.HasValue || roster.EndDate.Value.Date >= effectiveFromDate!.Value))
                .Select(roster => new
                {
                    roster.EmployeeId,
                    roster.ShiftId,
                    roster.StartDate,
                    roster.EndDate
                })
                .ToListAsync(cancellationToken);

            var positions = await _context.EmployeePositions.AsNoTracking()
                .Where(position =>
                    position.EmployeeId.HasValue &&
                    employeeIds.Contains(position.EmployeeId.Value) &&
                    (position.UtBlock == null || position.UtBlock == false))
                .Select(position => new
                {
                    position.EmployeeId,
                    position.FromDate,
                    position.TillDate
                })
                .ToListAsync(cancellationToken);

            var shiftIds = rosters
                .Where(x => x.ShiftId.HasValue)
                .Select(x => x.ShiftId!.Value)
                .Concat(employees.Where(x => x.ShiftId.HasValue).Select(x => x.ShiftId!.Value))
                .Concat(rawRows.Where(x => x.ShiftId.HasValue).Select(x => x.ShiftId!.Value))
                .Concat(items.Where(x => x.ShiftId.HasValue).Select(x => x.ShiftId!.Value))
                .Distinct()
                .ToList();

            var shiftDetails = await (
                from detail in _context.ShiftDetails.AsNoTracking()
                join header in _context.Shifts.AsNoTracking() on detail.ShiftId equals header.Id
                where detail.ShiftId.HasValue &&
                      shiftIds.Contains(detail.ShiftId.Value) &&
                      (detail.UtBlock == null || detail.UtBlock == false) &&
                      (header.UtBlock == null || header.UtBlock == false) &&
                      header.ClientId == request.ClientId
                select new
                {
                    detail.Id,
                    detail.ShiftId,
                    detail.DayId,
                    detail.InTime,
                    detail.OutTime,
                    detail.StartDate,
                    detail.EndDate
                })
                .ToListAsync(cancellationToken);

            (int? ShiftDtlId, int? DayId, DateTime? InTime, DateTime? OutTime) ResolveScheduleTime(int? shiftId, int? dayId, DateTime attendanceDate)
            {
                if (!shiftId.HasValue)
                {
                    return (null, null, null, null);
                }

                var dayIds = GetLegacyDayIds(attendanceDate.DayOfWeek);
                if (dayId.HasValue && !dayIds.Contains(dayId.Value))
                {
                    dayIds.Insert(0, dayId.Value);
                }

                var detail = shiftDetails
                    .Where(x =>
                        x.ShiftId == shiftId.Value &&
                        x.DayId.HasValue &&
                        dayIds.Contains(x.DayId.Value) &&
                        (!x.StartDate.HasValue || x.StartDate.Value.Date <= attendanceDate) &&
                        (!x.EndDate.HasValue || x.EndDate.Value.Date >= attendanceDate) &&
                        (x.InTime.HasValue || x.OutTime.HasValue))
                    .OrderByDescending(x => x.StartDate)
                    .FirstOrDefault();

                if (detail != null)
                {
                    return (detail.Id, detail.DayId, detail.InTime, detail.OutTime);
                }

                return (null, null, null, null);
            }

            EmployeeDailyAttendanceV3Dto EnrichWithCurrentShift(EmployeeDailyAttendanceV3Dto item)
            {
                if (!item.EmployeeId.HasValue || !item.AtnDate.HasValue)
                {
                    return item;
                }

                var attendanceDate = item.AtnDate.Value.Date;
                var roster = rosters
                    .Where(x =>
                        x.EmployeeId == item.EmployeeId.Value &&
                                x.StartDate!.Value.Date <= attendanceDate &&
                                (!x.EndDate.HasValue || x.EndDate.Value.Date >= attendanceDate))
                    .OrderByDescending(x => x.StartDate)
                    .FirstOrDefault();

                var effectiveShiftId = roster?.ShiftId ?? item.ShiftId;
                var dayId = item.DayId ?? ToLegacyDayId(attendanceDate.DayOfWeek);
                var scheduleTime = ResolveScheduleTime(effectiveShiftId, dayId, attendanceDate);
                var schTimeIn = scheduleTime.InTime;
                var schTimeOut = scheduleTime.OutTime;
                var rawDayRows = rawRows
                    .Where(x => x.EmpId == item.EmployeeId.Value && x.Dttime!.Value.Date == attendanceDate)
                    .OrderBy(x => x.Dttime)
                    .ToList();
                var rawActShiftIn = ResolveCheckInPunch(rawDayRows);
                var rawActShiftOut = ResolveCheckOutPunch(rawDayRows);
                var hasExplicitRawActivity = rawActShiftIn.HasValue || rawActShiftOut.HasValue;
                var resolvedActShiftIn = hasExplicitRawActivity ? rawActShiftIn : item.ActShiftIn;
                var resolvedActShiftOut = hasExplicitRawActivity ? rawActShiftOut : item.ActShiftOut;
                var rawLateMin = rawDayRows.FirstOrDefault(x => x.AtnActivityId == CheckInActivityId && x.LateMin.HasValue)?.LateMin ?? rawDayRows.FirstOrDefault(x => x.LateMin.HasValue)?.LateMin;
                var rawLateCount = rawDayRows.FirstOrDefault(x => x.AtnActivityId == CheckInActivityId && x.LateCount.HasValue)?.LateCount ?? rawDayRows.FirstOrDefault(x => x.LateCount.HasValue)?.LateCount;
                var rawEdMin = rawDayRows.FirstOrDefault(x => x.AtnActivityId == CheckOutActivityId && x.EdMin.HasValue)?.EdMin ?? rawDayRows.FirstOrDefault(x => x.EdMin.HasValue)?.EdMin;
                var rawEdCount = rawDayRows.FirstOrDefault(x => x.AtnActivityId == CheckOutActivityId && x.EdCount.HasValue)?.EdCount ?? rawDayRows.FirstOrDefault(x => x.EdCount.HasValue)?.EdCount;
                var rawOtMin = rawDayRows.FirstOrDefault(x => x.AtnActivityId == CheckOutActivityId && x.OtMin.HasValue)?.OtMin ?? rawDayRows.FirstOrDefault(x => x.OtMin.HasValue)?.OtMin;
                var rawOtCount = rawDayRows.FirstOrDefault(x => x.AtnActivityId == CheckOutActivityId && x.OtCount.HasValue)?.OtCount ?? rawDayRows.FirstOrDefault(x => x.OtCount.HasValue)?.OtCount;

                return item with
                {
                    SchTimeIn = schTimeIn,
                    SchTimeOut = schTimeOut,
                    ActShiftIn = resolvedActShiftIn,
                    ActShiftOut = resolvedActShiftOut,
                    LateMin = rawLateMin ?? item.LateMin,
                    LateCount = rawLateCount ?? item.LateCount,
                    EdMin = rawEdMin ?? item.EdMin,
                    EdCount = rawEdCount ?? item.EdCount,
                    OtMin = rawOtMin ?? item.OtMin,
                    OtCount = rawOtCount ?? item.OtCount,
                    IsAbsent = !item.IsHoliday &&
                        resolvedActShiftIn == null && resolvedActShiftOut == null,
                    ShiftId = effectiveShiftId,
                    ShiftDtlId = item.ShiftDtlId ?? scheduleTime.ShiftDtlId,
                    DayId = scheduleTime.DayId ?? dayId
                };
            }

            items = items.Select(EnrichWithCurrentShift).ToList();

            derivedItems = derivedItems.Select(EnrichWithCurrentShift).ToList();

            var generatedItems = employees
                .SelectMany(employee => Enumerable.Range(0, (effectiveTillDate!.Value - effectiveFromDate!.Value).Days + 1)
                    .Select(offset => effectiveFromDate!.Value.AddDays(offset))
                    .Where(date => positions.Any(position =>
                        position.EmployeeId == employee.Id &&
                        (!position.FromDate.HasValue || position.FromDate.Value.Date <= date) &&
                        (!position.TillDate.HasValue || position.TillDate.Value.Date >= date)))
                    .Where(date => rosters.Any(roster =>
                        roster.EmployeeId == employee.Id &&
                        roster.StartDate!.Value.Date <= date &&
                        (!roster.EndDate.HasValue || roster.EndDate.Value.Date >= date)))
                    .Where(date => !existingKeys.Contains($"{employee.Id}:{date:yyyyMMdd}"))
                    .Select(date =>
                    {
                        var roster = rosters
                            .Where(x =>
                                x.EmployeeId == employee.Id &&
                                x.StartDate!.Value.Date <= date &&
                                (!x.EndDate.HasValue || x.EndDate.Value.Date >= date))
                            .OrderByDescending(x => x.StartDate)
                            .FirstOrDefault();

                        var dayId = ToLegacyDayId(date.DayOfWeek);
                        var effectiveShiftId = roster?.ShiftId;
                        var scheduleTime = ResolveScheduleTime(effectiveShiftId, dayId, date);

                        return new EmployeeDailyAttendanceV3Dto(
                            CreateSyntheticDailyAttendanceId(employee.Id, date),
                            employee.Id,
                            employee.EmployeeCode ?? string.Empty,
                            $"{employee.FirstName ?? string.Empty} {employee.LastName ?? string.Empty}".Trim().ToUpperInvariant(),
                            date,
                            date.ToString("dddd"),
                            scheduleTime.InTime,
                            scheduleTime.OutTime,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            null,
                            string.Empty,
                            true,
                            false,
                            effectiveShiftId,
                            scheduleTime.ShiftDtlId,
                            scheduleTime.DayId ?? dayId,
                            employee.DepartmentId,
                            employee.DesignationId
                        );
                    }))
                .ToList();

            bool IsWithinEmployeeRoster(EmployeeDailyAttendanceV3Dto item)
            {
                if (!item.EmployeeId.HasValue || !item.AtnDate.HasValue)
                {
                    return false;
                }

                var attendanceDate = item.AtnDate.Value.Date;
                return rosters.Any(roster =>
                    roster.EmployeeId == item.EmployeeId.Value &&
                    roster.StartDate!.Value.Date <= attendanceDate &&
                    (!roster.EndDate.HasValue ||
                     roster.EndDate.Value.Date >= attendanceDate));
            }

            var rules = await AttendanceShiftRules.LoadAsync(_context, request.ClientId, employeeIds,
                effectiveFromDate!.Value, effectiveTillDate!.Value, cancellationToken);
            return items.Concat(derivedItems).Concat(generatedItems)
                .Where(IsWithinEmployeeRoster)
                .Select(rules.Apply)
                .OrderBy(x => x.AtnDate)
                .ThenBy(x => x.EmployeeCode)
                .ToList();
        }

        public async Task<EmployeeDailyAttendanceV3Dto?> Handle(GetEmployeeDailyAttendanceByIdV3Query request, CancellationToken cancellationToken)
        {
            var query =
                from daily in _context.DailyAttendances.AsNoTracking()
                join emp in _context.Employees.AsNoTracking() on daily.EmployeeId equals emp.Id
                where daily.Id == request.Id && daily.UtBlock == false
                      && emp.ClientId == request.ClientId
                      && daily.AtnDate.HasValue
                      && _context.EmployeeRosters.Any(roster =>
                          roster.EmployeeId == daily.EmployeeId &&
                          (roster.UtBlock == null || roster.UtBlock == false) &&
                          roster.StartDate.HasValue &&
                          roster.StartDate.Value.Date <= daily.AtnDate.Value.Date &&
                          (!roster.EndDate.HasValue ||
                           roster.EndDate.Value.Date >= daily.AtnDate.Value.Date))
                      && _context.EmployeePositions.Any(position =>
                          position.EmployeeId == daily.EmployeeId &&
                          (position.UtBlock == null || position.UtBlock == false))
                select new EmployeeDailyAttendanceV3Dto(
                    daily.Id,
                    daily.EmployeeId,
                    emp.EmployeeCode ?? string.Empty,
                    $"{emp.FirstName ?? string.Empty} {emp.LastName ?? string.Empty}".Trim().ToUpperInvariant(),
                    daily.AtnDate,
                    daily.AtnDate.HasValue ? daily.AtnDate.Value.ToString("dddd") : string.Empty,
                    daily.SchTimeIn,
                    daily.SchTimeOut,
                    daily.ActShiftIn,
                    daily.ActShitOut,
                    daily.LateMin,
                    daily.LateCount,
                    daily.LateHalfDays,
                    daily.LateAbsent,
                    daily.LateExcuse,
                    daily.EdMin,
                    daily.EdCount,
                    daily.EdHalfDays,
                    daily.EdAbsent,
                    daily.EdExcuse,
                    daily.OtMin,
                    daily.OtCount,
                    daily.OtHalfDays,
                    daily.Remarks ?? string.Empty,
                    daily.IsAbsent,
                    daily.IsHoliday,
                    daily.ShiftId,
                    daily.ShiftDtlId,
                    daily.DayId,
                    daily.DepId,
                    daily.DesignationId
                );

            var item = await query.FirstOrDefaultAsync(cancellationToken);
            if (item?.EmployeeId == null || item.AtnDate == null) return item;
            var rules = await AttendanceShiftRules.LoadAsync(_context, request.ClientId,
                new[] { item.EmployeeId.Value }, item.AtnDate.Value, item.AtnDate.Value, cancellationToken);
            return rules.Apply(item);
        }

        private static long CreateSyntheticDailyAttendanceId(int employeeId, DateTime attendanceDate)
        {
            var datePart = long.Parse(attendanceDate.ToString("yyyyMMdd"));
            var employeePart = Math.Abs((long)employeeId % 100_000L);
            return 9_000_000_000_000L + (datePart * 100_000L) + employeePart;
        }

        private static int? CalculateLateMinutes(DateTime? scheduledIn, DateTime? actualIn)
        {
            if (!scheduledIn.HasValue || !actualIn.HasValue) return null;
            var minutes = (int)(actualIn.Value.TimeOfDay - scheduledIn.Value.TimeOfDay).TotalMinutes;
            return minutes > 0 ? minutes : 0;
        }

        private static int? CalculateEarlyDepartureMinutes(DateTime? scheduledOut, DateTime? actualOut)
        {
            if (!scheduledOut.HasValue || !actualOut.HasValue) return null;
            var minutes = (int)(scheduledOut.Value.TimeOfDay - actualOut.Value.TimeOfDay).TotalMinutes;
            return minutes > 0 ? minutes : 0;
        }

        private static int? CalculateOvertimeMinutes(DateTime? scheduledOut, DateTime? actualOut)
        {
            if (!scheduledOut.HasValue || !actualOut.HasValue) return null;
            var minutes = (int)(actualOut.Value.TimeOfDay - scheduledOut.Value.TimeOfDay).TotalMinutes;
            return minutes > 0 ? minutes : 0;
        }

        private static DateTime? ResolveCheckInPunch<T>(IReadOnlyList<T> orderedRows)
            where T : class
        {
            return orderedRows
                .Select(GetPunchInfo)
                .Where(x => x.ActivityId == CheckInActivityId)
                .Select(x => x.PunchTime)
                .FirstOrDefault()
                ?? orderedRows
                    .Select(GetPunchInfo)
                    .Where(x => x.PunchTime.HasValue && x.PunchTime.Value.TimeOfDay < new TimeSpan(12, 0, 0))
                    .Select(x => x.PunchTime)
                    .FirstOrDefault();
        }

        private static DateTime? ResolveCheckOutPunch<T>(IReadOnlyList<T> orderedRows)
            where T : class
        {
            return orderedRows
                .Select(GetPunchInfo)
                .Where(x => x.ActivityId == CheckOutActivityId)
                .Select(x => x.PunchTime)
                .LastOrDefault()
                ?? orderedRows
                    .Select(GetPunchInfo)
                    .Where(x => x.PunchTime.HasValue && x.PunchTime.Value.TimeOfDay >= new TimeSpan(12, 0, 0))
                    .Select(x => x.PunchTime)
                    .LastOrDefault();
        }

        private static (DateTime? PunchTime, int? ActivityId) GetPunchInfo<T>(T row)
            where T : class
        {
            var type = row.GetType();
            return (
                type.GetProperty("Dttime")?.GetValue(row) as DateTime?,
                type.GetProperty("AtnActivityId")?.GetValue(row) as int?
            );
        }
        private static int ToLegacyDayId(DayOfWeek dayOfWeek)
        {
            return dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;
        }

        private static List<int> GetLegacyDayIds(DayOfWeek dayOfWeek)
        {
            var standardDayId = ToLegacyDayId(dayOfWeek);
            var etDayId = dayOfWeek == DayOfWeek.Sunday ? 106 : (int)dayOfWeek + 99;
            return standardDayId == etDayId
                ? new List<int> { standardDayId }
                : new List<int> { standardDayId, etDayId };
        }
    }
}

