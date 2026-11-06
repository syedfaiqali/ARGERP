using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class EmployeeAttendanceCommandHandlers :
        IRequestHandler<CreateEmployeeAttendanceV3Command, EmployeeAttendanceV3Dto>,
        IRequestHandler<UpdateEmployeeAttendanceV3Command, bool>,
        IRequestHandler<DeleteEmployeeAttendanceV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;

        public EmployeeAttendanceCommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<EmployeeAttendanceV3Dto> Handle(CreateEmployeeAttendanceV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == request.EmpId
                         && x.ClientId == command.ClientId
                         && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value),
                    cancellationToken);

            if (employee == null)
            {
                throw new InvalidOperationException("Employee not found.");
            }

            HRAtnactivityLegacy? activity = null;
            if (request.AtnActivityId != 0)
            {
                activity = await _context.AttendanceActivities
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.Id == request.AtnActivityId
                             && (x.ClientId == null || x.ClientId == command.ClientId)
                             && x.UtBlock == false,
                        cancellationToken);

                if (activity == null && request.AtnActivityId > 0)
                {
                    throw new InvalidOperationException("Activity not found.");
                }
            }

            var attendanceDate = request.AttendanceDate.Date;
            var attendanceTime = request.AttendanceTime;
            var combinedDateTime = attendanceDate.Add(attendanceTime.TimeOfDay);

            await EnsureAttendanceDateIsOnOrAfterRosterStart(request.EmpId, attendanceDate, cancellationToken);

            var dayId = (int)combinedDateTime.DayOfWeek == 0 ? 7 : (int)combinedDateTime.DayOfWeek;
            var currentShift = await GetCurrentShiftDetail(request.EmpId, command.EntityId ?? employee.EntityId, combinedDateTime, employee.ShiftId, dayId, cancellationToken);
            if (currentShift == null)
            {
                throw new InvalidOperationException("Employee shift is not defined for the selected attendance date.");
            }

            var nextId = (await _context.EmployeeAttendances.IgnoreQueryFilters()
                .MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            var entity = new HRAtnintoutLegacy
            {
                Id = nextId,
                EmpId = request.EmpId,
                AtnActivityId = request.AtnActivityId != 0 ? request.AtnActivityId : null,
                AtnTypeId = activity?.Value,
                Dttime = combinedDateTime,
                OrgDttime = combinedDateTime,
                DayId = dayId,
                Remarks = request.Remarks?.Trim() ?? string.Empty,
                ShiftId = currentShift.ShiftId,
                ShiftDtlId = currentShift.ShiftDtlId,
                ShiftIn = currentShift.InTime,
                ShiftOut = currentShift.OutTime,
                SchTimeIn = currentShift.InTime,
                SchTimeOut = currentShift.OutTime,
                ClientId = command.ClientId,
                EntityId = command.EntityId,
                UtSysGen = false,
                UtIsPartial = false,
                UtRecordVersion = 1,
                UtBlock = false
            };

            _context.EmployeeAttendances.Add(entity);
            _context.Entry(entity).Property(e => e.Id).IsTemporary = false;
            await _context.SaveChangesAsync(cancellationToken);

            var department = await _context.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == employee.DepartmentId, cancellationToken);

            var designation = await _context.Designations
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == employee.DesignationId, cancellationToken);

            var shift = await _context.Shifts
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == employee.ShiftId, cancellationToken);

            return new EmployeeAttendanceV3Dto(
                entity.Id,
                entity.EmpId,
                employee.EmployeeCode ?? string.Empty,
                $"{employee.FirstName ?? string.Empty} {employee.LastName ?? string.Empty}".Trim().ToUpperInvariant(),
                department?.DepStxt ?? string.Empty,
                designation?.Stxt ?? string.Empty,
                activity?.Id,
                activity?.Stxt ?? string.Empty,
                entity.Dttime,
                entity.OrgDttime,
                entity.Remarks ?? string.Empty,
                employee.MachineCode ?? string.Empty,
                employee.ShiftId,
                shift?.Stxt ?? string.Empty
            );
        }

        public async Task<bool> Handle(UpdateEmployeeAttendanceV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var entity = await _context.EmployeeAttendances
                .FirstOrDefaultAsync(
                    x => x.Id == request.Id
                         && x.ClientId == command.ClientId
                         && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value)
                         && x.UtBlock == false,
                    cancellationToken);

            if (entity == null) return false;

            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.Id == request.EmpId
                         && x.ClientId == command.ClientId
                         && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value),
                    cancellationToken);

            if (employee == null)
            {
                throw new InvalidOperationException("Employee not found.");
            }

            HRAtnactivityLegacy? activity = null;
            if (request.AtnActivityId != 0)
            {
                activity = await _context.AttendanceActivities
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.Id == request.AtnActivityId
                             && (x.ClientId == null || x.ClientId == command.ClientId)
                             && x.UtBlock == false,
                        cancellationToken);

                if (activity == null && request.AtnActivityId > 0)
                {
                    throw new InvalidOperationException("Activity not found.");
                }
            }

            var attendanceDate = request.AttendanceDate.Date;
            var attendanceTime = request.AttendanceTime;
            var combinedDateTime = attendanceDate.Add(attendanceTime.TimeOfDay);

            await EnsureAttendanceDateIsOnOrAfterRosterStart(request.EmpId, attendanceDate, cancellationToken);

            var dayId = (int)combinedDateTime.DayOfWeek == 0 ? 7 : (int)combinedDateTime.DayOfWeek;
            var currentShift = await GetCurrentShiftDetail(request.EmpId, command.EntityId ?? employee.EntityId, combinedDateTime, employee.ShiftId, dayId, cancellationToken);
            if (currentShift == null)
            {
                throw new InvalidOperationException("Employee shift is not defined for the selected attendance date.");
            }

            entity.EmpId = request.EmpId;
            entity.AtnActivityId = request.AtnActivityId != 0 ? request.AtnActivityId : null;
            entity.AtnTypeId = activity?.Value;
            entity.Dttime = combinedDateTime;
            entity.OrgDttime = combinedDateTime;
            entity.DayId = dayId;
            entity.Remarks = request.Remarks?.Trim() ?? string.Empty;
            entity.ShiftId = currentShift.ShiftId;
            entity.ShiftDtlId = currentShift.ShiftDtlId;
            entity.ShiftIn = currentShift.InTime;
            entity.ShiftOut = currentShift.OutTime;
            entity.SchTimeIn = currentShift.InTime;
            entity.SchTimeOut = currentShift.OutTime;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Handle(DeleteEmployeeAttendanceV3Command command, CancellationToken cancellationToken)
        {
            var entity = await _context.EmployeeAttendances
                .FirstOrDefaultAsync(
                    x => x.Id == command.Id
                         && x.ClientId == command.ClientId
                         && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value)
                         && x.UtBlock == false,
                    cancellationToken);

            if (entity == null) return false;

            entity.UtBlock = true;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private async Task<CurrentShiftDetail?> GetCurrentShiftDetail(
            int employeeId,
            int? entityId,
            DateTime attendanceDate,
            int? requestShiftId,
            int dayId,
            CancellationToken cancellationToken)
        {
            var roster = await _context.EmployeeRosters.AsNoTracking()
                .Where(x =>
                    x.EmployeeId == employeeId &&
                    (x.UtBlock == null || x.UtBlock == false) &&
                    x.ShiftId.HasValue &&
                    x.StartDate.HasValue &&
                    x.StartDate.Value.Date <= attendanceDate.Date)
                .OrderByDescending(x => x.StartDate)
                .Select(x => new { x.ShiftId })
                .FirstOrDefaultAsync(cancellationToken);

            var shiftId = roster?.ShiftId ?? requestShiftId;
            if (!shiftId.HasValue)
            {
                return null;
            }

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
                    InTime = detail.InTime,
                    OutTime = detail.OutTime
                };

            return await query.FirstOrDefaultAsync(cancellationToken);
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

        private sealed class CurrentShiftDetail
        {
            public int ShiftId { get; set; }
            public int ShiftDtlId { get; set; }
            public DateTime? InTime { get; set; }
            public DateTime? OutTime { get; set; }
        }

        private static List<int> GetLegacyDayIds(DayOfWeek dayOfWeek)
        {
            var standardDayId = dayOfWeek == DayOfWeek.Sunday ? 7 : (int)dayOfWeek;
            var etDayId = dayOfWeek == DayOfWeek.Sunday ? 106 : (int)dayOfWeek + 99;
            return standardDayId == etDayId
                ? new List<int> { standardDayId }
                : new List<int> { standardDayId, etDayId };
        }
    }
}
