using HumanResource.Service.Commands;
using HumanResource.Service.Queries;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class EmployeeDailyAttendanceCommandHandlers :
        IRequestHandler<BulkSaveEmployeeDailyAttendanceV3Command, IReadOnlyList<EmployeeDailyAttendanceV3Dto>>,
        IRequestHandler<CreateEmployeeDailyAttendanceV3Command, EmployeeDailyAttendanceV3Dto>,
        IRequestHandler<UpdateEmployeeDailyAttendanceV3Command, EmployeeDailyAttendanceV3Dto>,
        IRequestHandler<DeleteEmployeeDailyAttendanceV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;
        private readonly IMediator _mediator;

        public EmployeeDailyAttendanceCommandHandlers(HumanResourceDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task<EmployeeDailyAttendanceV3Dto> Handle(CreateEmployeeDailyAttendanceV3Command command, CancellationToken cancellationToken)
        {
            var entity = await UpsertDailyAttendance(command.Request, command.ClientId, command.EntityId, null, null, cancellationToken);
            await ApplyShiftRules(new[] { entity }, command.ClientId, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await LinkRawAttendance(entity, command.ClientId, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return await GetSavedDto(entity.Id, command.ClientId, command.EntityId, cancellationToken);
        }

        public async Task<EmployeeDailyAttendanceV3Dto> Handle(UpdateEmployeeDailyAttendanceV3Command command, CancellationToken cancellationToken)
        {
            var request = new CreateEmployeeDailyAttendanceV3Request(
                command.Request.EmployeeId,
                command.Request.AtnDate,
                command.Request.SchTimeIn,
                command.Request.SchTimeOut,
                command.Request.ActShiftIn,
                command.Request.ActShiftOut,
                command.Request.LateMin,
                command.Request.LateCount,
                command.Request.LateHalfDays,
                command.Request.LateAbsent,
                command.Request.LateExcuse,
                command.Request.EdMin,
                command.Request.EdCount,
                command.Request.EdHalfDays,
                command.Request.EdAbsent,
                command.Request.EdExcuse,
                command.Request.OtMin,
                command.Request.OtCount,
                command.Request.OtHalfDays,
                command.Request.Remarks,
                command.Request.IsAbsent,
                command.Request.IsHoliday,
                command.Request.ShiftId,
                command.Request.ShiftDtlId,
                command.Request.DayId,
                command.Request.DepartmentId,
                command.Request.DesignationId);
            var entity = await UpsertDailyAttendance(request, command.ClientId, command.EntityId, command.Request.Id, null, cancellationToken);
            await ApplyShiftRules(new[] { entity }, command.ClientId, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await LinkRawAttendance(entity, command.ClientId, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return await GetSavedDto(entity.Id, command.ClientId, command.EntityId, cancellationToken);
        }

        public async Task<IReadOnlyList<EmployeeDailyAttendanceV3Dto>> Handle(BulkSaveEmployeeDailyAttendanceV3Command command, CancellationToken cancellationToken)
        {
            var items = command.Request.Items.Where(x => x.EmployeeId > 0).ToList();
            if (items.Count == 0)
            {
                return Array.Empty<EmployeeDailyAttendanceV3Dto>();
            }

            var savedEntities = await BulkUpsertDailyAttendances(
                items,
                command.ClientId,
                command.EntityId,
                cancellationToken);

            await ApplyShiftRules(savedEntities, command.ClientId, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            // Link all unposted device punches with one query instead of
            // loading the attendance entity and raw rows once per saved row.
            var employeeIds = savedEntities
                .Where(x => x.EmployeeId.HasValue)
                .Select(x => x.EmployeeId!.Value)
                .Distinct()
                .ToList();
            var fromDate = savedEntities.Min(x => x.AtnDate!.Value.Date);
            var tillDate = savedEntities.Max(x => x.AtnDate!.Value.Date);
            var entityByKey = savedEntities
                .Where(x => x.EmployeeId.HasValue && x.AtnDate.HasValue)
                .GroupBy(x => $"{x.EmployeeId!.Value}:{x.AtnDate!.Value.Date:yyyyMMdd}")
                .ToDictionary(x => x.Key, x => x.First());
            var savedAttendanceIds = savedEntities.Select(x => x.Id).Distinct().ToList();

            var rawRows = await _context.EmployeeAttendances
                .Where(x =>
                    x.EmpId.HasValue &&
                    employeeIds.Contains(x.EmpId.Value) &&
                    x.Dttime.HasValue &&
                    x.Dttime.Value.Date >= fromDate &&
                    x.Dttime.Value.Date <= tillDate &&
                    (x.PostTransId == null ||
                     (x.PostTransId.HasValue && savedAttendanceIds.Contains(x.PostTransId.Value))))
                .ToListAsync(cancellationToken);

            foreach (var raw in rawRows)
            {
                var key = $"{raw.EmpId!.Value}:{raw.Dttime!.Value.Date:yyyyMMdd}";
                if (entityByKey.TryGetValue(key, out var attendance) &&
                    (raw.PostTransId == null || raw.PostTransId == attendance.Id))
                {
                    raw.PostTransId = attendance.Id;
                    raw.ClientId = command.ClientId;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            var savedIds = savedEntities.Select(x => x.Id).Distinct().ToList();
            return await GetSavedDtos(savedIds, command.ClientId, cancellationToken);
        }

        public async Task<bool> Handle(DeleteEmployeeDailyAttendanceV3Command command, CancellationToken cancellationToken)
        {
            var entity = await (
                from daily in _context.DailyAttendances
                join employee in _context.Employees.AsNoTracking() on daily.EmployeeId equals employee.Id
                where daily.Id == command.Id && employee.ClientId == command.ClientId
                select daily)
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                return false;
            }

            entity.UtBlock = true;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private async Task<List<HRDailyAttendanceLegacy>> BulkUpsertDailyAttendances(
            IReadOnlyList<CreateEmployeeDailyAttendanceV3Request> requests,
            int clientId,
            int? entityId,
            CancellationToken cancellationToken)
        {
            var employeeIds = requests.Select(x => x.EmployeeId).Distinct().ToList();
            var fromDate = requests.Min(x => x.AtnDate.Date);
            var tillDate = requests.Max(x => x.AtnDate.Date);

            var employees = await _context.Employees.AsNoTracking()
                .Where(x =>
                    employeeIds.Contains(x.Id) &&
                    x.ClientId == clientId)
                .Select(x => new
                {
                    x.Id,
                    x.EmployeeCode,
                    x.FirstName,
                    x.LastName,
                    x.EntityId,
                    x.ShiftId,
                    x.DepartmentId,
                    x.DesignationId
                })
                .ToListAsync(cancellationToken);
            var employeeById = employees.ToDictionary(x => x.Id);

            var positions = await _context.EmployeePositions.AsNoTracking()
                .Where(x =>
                    x.EmployeeId.HasValue &&
                    employeeIds.Contains(x.EmployeeId.Value) &&
                    (x.UtBlock == null || x.UtBlock == false) &&
                    (!x.FromDate.HasValue || x.FromDate.Value.Date <= tillDate) &&
                    (!x.TillDate.HasValue || x.TillDate.Value.Date >= fromDate))
                .Select(x => new { x.EmployeeId, x.FromDate, x.TillDate })
                .ToListAsync(cancellationToken);

            var rosters = await _context.EmployeeRosters.AsNoTracking()
                .Where(x =>
                    x.EmployeeId.HasValue &&
                    employeeIds.Contains(x.EmployeeId.Value) &&
                    (x.UtBlock == null || x.UtBlock == false) &&
                    x.StartDate.HasValue &&
                    x.StartDate.Value.Date <= tillDate &&
                    (!x.EndDate.HasValue || x.EndDate.Value.Date >= fromDate))
                .Select(x => new
                {
                    x.EmployeeId,
                    x.ShiftId,
                    x.StartDate,
                    x.EndDate
                })
                .ToListAsync(cancellationToken);

            var shiftIds = rosters.Where(x => x.ShiftId.HasValue).Select(x => x.ShiftId!.Value)
                .Concat(employees.Where(x => x.ShiftId.HasValue).Select(x => x.ShiftId!.Value))
                .Concat(requests.Where(x => x.ShiftId.HasValue).Select(x => x.ShiftId!.Value))
                .Distinct()
                .ToList();

            var shiftHeaders = await _context.Shifts.AsNoTracking()
                .Where(x =>
                    shiftIds.Contains(x.Id) &&
                    (x.UtBlock == null || x.UtBlock == false) &&
                    (!entityId.HasValue || x.EntityId == entityId.Value || x.EntityId == null))
                .Select(x => new { x.Id, x.Code, x.Stxt, x.EntityId })
                .ToListAsync(cancellationToken);

            var shiftDetails = await (
                from detail in _context.ShiftDetails.AsNoTracking()
                join header in _context.Shifts.AsNoTracking() on detail.ShiftId equals header.Id
                where detail.ShiftId.HasValue &&
                      shiftIds.Contains(detail.ShiftId.Value) &&
                      (detail.UtBlock == null || detail.UtBlock == false) &&
                      (header.UtBlock == null || header.UtBlock == false) &&
                      (!entityId.HasValue || header.EntityId == entityId.Value || header.EntityId == null)
                select new
                {
                    detail.Id,
                    detail.ShiftId,
                    detail.DayId,
                    detail.InTime,
                    detail.OutTime,
                    detail.StartDate,
                    HeaderEntityId = header.EntityId
                })
                .ToListAsync(cancellationToken);

            var smShifts = await _context.SmShifts.AsNoTracking()
                .Where(x =>
                    !x.UtBlock &&
                    (!entityId.HasValue || x.EntityId == entityId.Value || x.EntityId == null))
                .Select(x => new
                {
                    x.Id,
                    x.Code,
                    x.Stxt,
                    x.StartTime,
                    x.EndTime,
                    x.EntityId
                })
                .ToListAsync(cancellationToken);

            var existingRows = await _context.DailyAttendances
                .Where(x =>
                    x.EmployeeId.HasValue &&
                    employeeIds.Contains(x.EmployeeId.Value) &&
                    x.AtnDate.HasValue &&
                    x.AtnDate.Value.Date >= fromDate &&
                    x.AtnDate.Value.Date <= tillDate)
                .ToListAsync(cancellationToken);
            var existingByKey = existingRows
                .GroupBy(x => $"{x.EmployeeId!.Value}:{x.AtnDate!.Value.Date:yyyyMMdd}")
                .ToDictionary(x => x.Key, x => x.First());

            var nextId = (await _context.DailyAttendances.IgnoreQueryFilters()
                .MaxAsync(x => (long?)x.Id, cancellationToken) ?? 0L) + 1L;
            var savedEntities = new List<HRDailyAttendanceLegacy>(requests.Count);

            foreach (var request in requests)
            {
                if (!employeeById.TryGetValue(request.EmployeeId, out var employee))
                {
                    throw new InvalidOperationException("Employee not found.");
                }

                var attendanceDate = request.AtnDate.Date;
                if (!positions.Any(x =>
                        x.EmployeeId == request.EmployeeId &&
                        (!x.FromDate.HasValue || x.FromDate.Value.Date <= attendanceDate) &&
                        (!x.TillDate.HasValue || x.TillDate.Value.Date >= attendanceDate)))
                {
                    throw new InvalidOperationException("Employee position assignment is not defined for the selected attendance date.");
                }

                var roster = rosters
                    .Where(x =>
                        x.EmployeeId == request.EmployeeId &&
                        x.StartDate!.Value.Date <= attendanceDate &&
                        (!x.EndDate.HasValue || x.EndDate.Value.Date >= attendanceDate))
                    .OrderByDescending(x => x.StartDate)
                    .FirstOrDefault();
                var shiftId = roster?.ShiftId ?? request.ShiftId ?? employee.ShiftId;

                var dayId = request.DayId ?? ToLegacyDayId(attendanceDate.DayOfWeek);
                var dayIds = GetLegacyDayIds(attendanceDate.DayOfWeek);
                var detail = shiftId.HasValue
                    ? shiftDetails
                        .Where(x =>
                            x.ShiftId == shiftId.Value &&
                            x.DayId.HasValue &&
                            dayIds.Contains(x.DayId.Value))
                        .OrderByDescending(x => entityId.HasValue && x.HeaderEntityId == entityId.Value)
                        .ThenByDescending(x => x.StartDate)
                        .FirstOrDefault()
                    : null;

                DateTime? shiftIn = detail?.InTime;
                DateTime? shiftOut = detail?.OutTime;
                int? shiftDetailId = detail?.Id;
                if (detail == null && shiftId.HasValue)
                {
                    var header = shiftHeaders.FirstOrDefault(x => x.Id == shiftId.Value);
                    var legacyShift = smShifts
                        .Where(x =>
                            x.Id == shiftId.Value ||
                            (!string.IsNullOrWhiteSpace(header?.Code) &&
                             string.Equals(x.Code, header.Code, StringComparison.OrdinalIgnoreCase)) ||
                            (!string.IsNullOrWhiteSpace(header?.Stxt) &&
                             string.Equals(x.Stxt, header.Stxt, StringComparison.OrdinalIgnoreCase)))
                        .OrderByDescending(x => entityId.HasValue && x.EntityId == entityId.Value)
                        .FirstOrDefault();
                    if (legacyShift != null)
                    {
                        shiftIn = legacyShift.StartTime;
                        shiftOut = legacyShift.EndTime;
                    }
                }

                var key = $"{request.EmployeeId}:{attendanceDate:yyyyMMdd}";
                if (!existingByKey.TryGetValue(key, out var entity))
                {
                    entity = new HRDailyAttendanceLegacy
                    {
                        Id = nextId++,
                        UtBlock = false
                    };
                    _context.DailyAttendances.Add(entity);
                    existingByKey[key] = entity;
                }

                entity.EmployeeId = request.EmployeeId;
                entity.AtnDate = attendanceDate;
                entity.SchTimeIn = request.SchTimeIn ?? shiftIn;
                entity.SchTimeOut = request.SchTimeOut ?? shiftOut;
                entity.ActShiftIn = NormalizeToAttendanceDate(attendanceDate, request.ActShiftIn);
                entity.ActShitOut = NormalizeToAttendanceDate(attendanceDate, request.ActShiftOut);
                entity.ShiftId = shiftId;
                entity.ShiftDtlId = NormalizePositiveId(request.ShiftDtlId) ?? shiftDetailId;
                entity.DayId = dayId;
                entity.Remarks = request.Remarks?.Trim();
                entity.EntityId = employee.EntityId ?? entityId;
                entity.LateMin = request.LateMin;
                entity.LateCount = request.LateCount;
                entity.LateHalfDays = request.LateHalfDays;
                entity.IsHoliday = request.IsHoliday;
                entity.LateAbsent = request.LateAbsent;
                entity.LateExcuse = request.LateExcuse;
                entity.EdMin = request.EdMin;
                entity.EdCount = request.EdCount;
                entity.EdHalfDays = request.EdHalfDays;
                entity.EdAbsent = request.EdAbsent;
                entity.EdExcuse = request.EdExcuse;
                entity.OtMin = request.OtMin;
                entity.OtCount = request.OtCount;
                entity.OtHalfDays = request.OtHalfDays;
                entity.DepId = request.DepartmentId ?? employee.DepartmentId;
                entity.DesignationId = request.DesignationId ?? employee.DesignationId;
                entity.IsAbsent = !entity.IsHoliday && entity.ActShiftIn == null && entity.ActShitOut == null;
                entity.UtSysGen = false;
                entity.UtIsPartial = false;
                entity.UtRecordVersion = entity.UtRecordVersion <= 0 ? 1 : entity.UtRecordVersion;
                entity.UtBlock = false;
                savedEntities.Add(entity);
            }

            return savedEntities;
        }

        private async Task<HRDailyAttendanceLegacy> UpsertDailyAttendance(
            CreateEmployeeDailyAttendanceV3Request request,
            int clientId,
            int? entityId,
            long? requestedId,
            long? generatedId,
            CancellationToken cancellationToken)
        {
            var employee = await _context.Employees.AsNoTracking()
                .FirstOrDefaultAsync(e =>
                    e.Id == request.EmployeeId &&
                    e.ClientId == clientId &&
                    (e.UtBlock == null || e.UtBlock == false),
                    cancellationToken);

            if (employee == null)
            {
                throw new InvalidOperationException("Employee not found.");
            }

            var attendanceDate = request.AtnDate.Date;
            var hasPositionAssignment = await _context.EmployeePositions.AsNoTracking()
                .AnyAsync(position =>
                    position.EmployeeId == request.EmployeeId &&
                    (position.UtBlock == null || position.UtBlock == false) &&
                    (!position.FromDate.HasValue || position.FromDate.Value.Date <= attendanceDate) &&
                    (!position.TillDate.HasValue || position.TillDate.Value.Date >= attendanceDate),
                    cancellationToken);
            if (!hasPositionAssignment)
            {
                throw new InvalidOperationException("Employee position assignment is not defined for the selected attendance date.");
            }

            var currentShift = await GetCurrentShiftDetail(
                request.EmployeeId,
                employee.EntityId ?? entityId,
                attendanceDate,
                request.ShiftId ?? employee.ShiftId,
                request.DayId,
                cancellationToken);
            var entity = requestedId.HasValue && requestedId.Value > 0
                ? await _context.DailyAttendances.FirstOrDefaultAsync(x =>
                    x.Id == requestedId.Value &&
                    x.EmployeeId == request.EmployeeId &&
                    (x.EntityId == employee.EntityId || !employee.EntityId.HasValue),
                    cancellationToken)
                : null;

            entity ??= await _context.DailyAttendances.FirstOrDefaultAsync(x =>
                x.EmployeeId == request.EmployeeId &&
                x.AtnDate.HasValue &&
                x.AtnDate.Value.Date == attendanceDate &&
                (x.EntityId == employee.EntityId || !employee.EntityId.HasValue),
                cancellationToken);

            if (entity == null)
            {
                entity = new HRDailyAttendanceLegacy
                {
                    Id = generatedId ?? await GetNextDailyAttendanceId(cancellationToken),
                    UtBlock = false
                };
                _context.DailyAttendances.Add(entity);
            }

            entity.EmployeeId = request.EmployeeId;
            entity.AtnDate = attendanceDate;
            entity.SchTimeIn = request.SchTimeIn ?? currentShift?.InTime;
            entity.SchTimeOut = request.SchTimeOut ?? currentShift?.OutTime;
            entity.ActShiftIn = NormalizeToAttendanceDate(attendanceDate, request.ActShiftIn);
            entity.ActShitOut = NormalizeToAttendanceDate(attendanceDate, request.ActShiftOut);
            entity.ShiftId = currentShift?.ShiftId ?? request.ShiftId ?? employee.ShiftId;
            entity.ShiftDtlId = NormalizePositiveId(request.ShiftDtlId) ?? currentShift?.ShiftDtlId;
            entity.DayId = request.DayId ?? currentShift?.DayId ?? ToLegacyDayId(attendanceDate.DayOfWeek);
            entity.Remarks = request.Remarks?.Trim();
            entity.EntityId = employee.EntityId ?? entityId;
            entity.LateMin = request.LateMin;
            entity.LateCount = request.LateCount;
            entity.LateHalfDays = request.LateHalfDays;
            entity.IsHoliday = request.IsHoliday;
            entity.LateAbsent = request.LateAbsent;
            entity.LateExcuse = request.LateExcuse;
            entity.EdMin = request.EdMin;
            entity.EdCount = request.EdCount;
            entity.EdHalfDays = request.EdHalfDays;
            entity.EdAbsent = request.EdAbsent;
            entity.EdExcuse = request.EdExcuse;
            entity.OtMin = request.OtMin;
            entity.OtCount = request.OtCount;
            entity.OtHalfDays = request.OtHalfDays;
            entity.DepId = request.DepartmentId ?? employee.DepartmentId;
            entity.DesignationId = request.DesignationId ?? employee.DesignationId;
            entity.IsAbsent = !entity.IsHoliday && entity.ActShiftIn == null && entity.ActShitOut == null;
            entity.UtSysGen = false;
            entity.UtIsPartial = false;
            entity.UtRecordVersion = entity.UtRecordVersion <= 0 ? 1 : entity.UtRecordVersion;
            entity.UtBlock = false;

            return entity;
        }

        private async Task ApplyShiftRules(IEnumerable<HRDailyAttendanceLegacy> entities, int clientId, CancellationToken ct)
        {
            var rows = entities.ToList();
            var rules = await AttendanceShiftRules.LoadAsync(_context, clientId,
                rows.Select(x => x.EmployeeId!.Value), rows.Min(x => x.AtnDate!.Value), rows.Max(x => x.AtnDate!.Value), ct);
            foreach (var row in rows) rules.Apply(row);
        }

        private async Task LinkRawAttendance(HRDailyAttendanceLegacy entity, int clientId, CancellationToken cancellationToken)
        {
            if (!entity.EmployeeId.HasValue || !entity.AtnDate.HasValue)
            {
                return;
            }

            var attendanceDate = entity.AtnDate.Value.Date;
            var rawRows = await _context.EmployeeAttendances
                .Where(x =>
                    x.EmpId == entity.EmployeeId.Value &&
                    x.Dttime.HasValue &&
                    x.Dttime.Value.Date == attendanceDate &&
                    (x.PostTransId == null || x.PostTransId == entity.Id))
                .ToListAsync(cancellationToken);

            foreach (var raw in rawRows)
            {
                raw.PostTransId = entity.Id;
                raw.ClientId = clientId;
            }
        }

        private async Task<CurrentShiftDetail?> GetCurrentShiftDetail(
            int employeeId,
            int? entityId,
            DateTime attendanceDate,
            int? requestShiftId,
            int? requestDayId,
            CancellationToken cancellationToken)
        {
            var roster = await _context.EmployeeRosters.AsNoTracking()
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    (x.UtBlock == null || x.UtBlock == false) &&
                    x.StartDate.HasValue &&
                    x.StartDate.Value.Date <= attendanceDate &&
                    (!x.EndDate.HasValue || x.EndDate.Value.Date >= attendanceDate))
                .OrderByDescending(x => x.StartDate)
                .Select(x => new { x.ShiftId })
                .FirstOrDefaultAsync(cancellationToken);

            if (roster == null)
            {
                return null;
            }

            var shiftId = roster.ShiftId ?? requestShiftId;
            if (!shiftId.HasValue)
            {
                return null;
            }

            var dayId = requestDayId ?? ToLegacyDayId(attendanceDate.DayOfWeek);
            var dayIds = GetLegacyDayIds(attendanceDate.DayOfWeek);
            var query =
                from detail in _context.ShiftDetails.AsNoTracking()
                join header in _context.Shifts.AsNoTracking() on detail.ShiftId equals header.Id
                where detail.Id > 0
                      && detail.ShiftId == shiftId.Value
                      && detail.DayId.HasValue
                      && dayIds.Contains(detail.DayId.Value)
                      && (detail.UtBlock == null || detail.UtBlock == false)
                      && (header.UtBlock == null || header.UtBlock == false)
                      && (!entityId.HasValue || header.EntityId == entityId.Value || header.EntityId == null)
                orderby header.EntityId == entityId descending, detail.StartDate descending
                select new CurrentShiftDetail
                {
                    ShiftId = shiftId.Value,
                    ShiftDtlId = detail.Id,
                    DayId = dayId,
                    InTime = detail.InTime,
                    OutTime = detail.OutTime
                };

            var detailResult = await query.FirstOrDefaultAsync(cancellationToken);
            if (detailResult != null)
            {
                return detailResult;
            }

            // Some legacy schedules define the working hours on SMShift rather
            // than creating an ETShiftDetail row for every day. The attendance
            // query already uses this fallback, so bulk-save must resolve the
            // same fetched rows in the same way.
            var shiftHeader = await _context.Shifts.AsNoTracking()
                .Where(x =>
                    x.Id == shiftId.Value &&
                    (x.UtBlock == null || x.UtBlock == false))
                .Select(x => new { x.Code, x.Stxt })
                .FirstOrDefaultAsync(cancellationToken);

            var shiftCode = shiftHeader != null ? shiftHeader.Code : null;
            var shiftName = shiftHeader != null ? shiftHeader.Stxt : null;
            var legacyShift = await _context.SmShifts.AsNoTracking()
                .Where(x =>
                    !x.UtBlock &&
                    (!entityId.HasValue || x.EntityId == entityId.Value || x.EntityId == null) &&
                    (x.Id == shiftId.Value ||
                     (shiftCode != null && x.Code == shiftCode) ||
                     (shiftName != null && x.Stxt == shiftName)))
                .OrderByDescending(x => entityId.HasValue && x.EntityId == entityId.Value)
                .Select(x => new { x.StartTime, x.EndTime })
                .FirstOrDefaultAsync(cancellationToken);

            if (legacyShift == null || (!legacyShift.StartTime.HasValue && !legacyShift.EndTime.HasValue))
            {
                return null;
            }

            return new CurrentShiftDetail
            {
                ShiftId = shiftId.Value,
                ShiftDtlId = null,
                DayId = dayId,
                InTime = legacyShift.StartTime,
                OutTime = legacyShift.EndTime
            };
        }

        private async Task EnsureAttendanceDateIsOnOrAfterRosterStart(
            int employeeId,
            DateTime attendanceDate,
            CancellationToken cancellationToken)
        {
            var rosterStartDate = await _context.EmployeeRosters.AsNoTracking()
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    (x.UtBlock == null || x.UtBlock == false) &&
                    x.StartDate.HasValue)
                .MinAsync(x => (DateTime?)x.StartDate, cancellationToken);

            if (!rosterStartDate.HasValue)
            {
                throw new InvalidOperationException("Employee roster is not defined.");
            }

            if (attendanceDate.Date < rosterStartDate.Value.Date)
            {
                throw new InvalidOperationException("Attendance cannot be marked before the employee's roster start date.");
            }
        }

        private async Task<long> GetNextDailyAttendanceId(CancellationToken cancellationToken)
        {
            var localMaxId = _context.DailyAttendances.Local
                .Select(x => (long?)x.Id)
                .Max() ?? 0L;
            var dbMaxId = await _context.DailyAttendances.IgnoreQueryFilters()
                .MaxAsync(x => (long?)x.Id, cancellationToken) ?? 0L;
            return Math.Max(localMaxId, dbMaxId) + 1L;
        }

        private async Task<IReadOnlyList<EmployeeDailyAttendanceV3Dto>> GetSavedDtos(
            IReadOnlyCollection<long> ids,
            int clientId,
            CancellationToken cancellationToken)
        {
            return await (
                from daily in _context.DailyAttendances.AsNoTracking()
                join emp in _context.Employees.AsNoTracking() on daily.EmployeeId equals emp.Id
                where ids.Contains(daily.Id)
                      && emp.ClientId == clientId
                      && daily.AtnDate.HasValue
                orderby daily.AtnDate descending, emp.EmployeeCode
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
                    daily.DesignationId))
                .ToListAsync(cancellationToken);
        }

        private async Task<EmployeeDailyAttendanceV3Dto> GetSavedDto(long id, int clientId, int? entityId, CancellationToken cancellationToken)
        {
            var dto = await _mediator.Send(new GetEmployeeDailyAttendanceByIdV3Query(id, clientId, entityId), cancellationToken);
            return dto ?? throw new InvalidOperationException("Failed to retrieve saved record.");
        }

        private static DateTime? NormalizeToAttendanceDate(DateTime attendanceDate, DateTime? value)
        {
            if (!value.HasValue) return null;
            return new DateTime(attendanceDate.Year, attendanceDate.Month, attendanceDate.Day, value.Value.Hour, value.Value.Minute, value.Value.Second);
        }

        private static int? NormalizePositiveId(int? value)
        {
            return value.HasValue && value.Value > 0 ? value : null;
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

        private sealed class CurrentShiftDetail
        {
            public int ShiftId { get; set; }
            public int? ShiftDtlId { get; set; }
            public int DayId { get; set; }
            public DateTime? InTime { get; set; }
            public DateTime? OutTime { get; set; }
        }
    }
}
