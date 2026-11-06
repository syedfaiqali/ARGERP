using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class UploadAttendanceCommandHandlers :
        IRequestHandler<ProcessUploadAttendanceV3Command, UploadAttendanceProcessResultV3Dto>
    {
        private readonly HumanResourceDbContext _context;

        public UploadAttendanceCommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<UploadAttendanceProcessResultV3Dto> Handle(ProcessUploadAttendanceV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var rows = request.Items ?? Array.Empty<UploadAttendanceProcessRowV3Request>();

            if (!request.FromDate.HasValue || !request.TillDate.HasValue)
            {
                throw new InvalidOperationException("From Date and Till Date are required.");
            }

            var fromDate = request.FromDate.Value.Date;
            var tillDate = request.TillDate.Value.Date;
            if (fromDate > tillDate)
            {
                throw new InvalidOperationException("From Date cannot be greater than Till Date.");
            }

            var employees = await _context.Employees
                .AsNoTracking()
                .Where(x => x.ClientId == command.ClientId
                            && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value)
                            && (x.UtBlock == null || x.UtBlock == false))
                .Select(x => new
                {
                    x.Id,
                    Code = x.EmployeeCode ?? string.Empty,
                    x.ShiftId
                })
                .ToListAsync(cancellationToken);

            var employeeByCode = employees
                .Where(x => !string.IsNullOrWhiteSpace(x.Code))
                .GroupBy(x => x.Code.Trim().ToUpperInvariant())
                .ToDictionary(g => g.Key, g => g.First());

            var nextId = (await _context.EmployeeAttendances.IgnoreQueryFilters()
                .MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            var entitiesToInsert = new List<HRAtnintoutLegacy>();
            var results = new List<UploadAttendanceProcessRowResultV3Dto>();
            var rowNo = 0;

            foreach (var row in rows)
            {
                rowNo++;
                var code = (row.EmployeeCode ?? string.Empty).Trim().ToUpperInvariant();
                if (string.IsNullOrWhiteSpace(code))
                {
                    results.Add(new UploadAttendanceProcessRowResultV3Dto(rowNo, string.Empty, false, "Employee Code is required.", null));
                    continue;
                }

                if (!employeeByCode.TryGetValue(code, out var employee))
                {
                    results.Add(new UploadAttendanceProcessRowResultV3Dto(rowNo, code, false, "Employee does not exist in employee profile.", null));
                    continue;
                }

                var attendanceDate = row.AttendanceDate.Date;
                if (attendanceDate < fromDate || attendanceDate > tillDate)
                {
                    results.Add(new UploadAttendanceProcessRowResultV3Dto(rowNo, code, false, "Attendance date must be between selected dates.", null));
                    continue;
                }

                var checkInTime = ParseTime(row.CheckIn);
                var checkOutTime = ParseTime(row.CheckOut);
                var eventDateTime = attendanceDate;
                if (checkInTime.HasValue) eventDateTime = attendanceDate.Add(checkInTime.Value);
                else if (checkOutTime.HasValue) eventDateTime = attendanceDate.Add(checkOutTime.Value);

                var dayId = (int)eventDateTime.DayOfWeek == 0 ? 7 : (int)eventDateTime.DayOfWeek;
                var currentId = nextId++;

                entitiesToInsert.Add(new HRAtnintoutLegacy
                {
                    Id = currentId,
                    EmpId = employee.Id,
                    Dttime = eventDateTime,
                    OrgDttime = eventDateTime,
                    DayId = dayId,
                    Remarks = row.Remarks?.Trim() ?? string.Empty,
                    ShiftId = employee.ShiftId,
                    ShiftIn = checkInTime.HasValue ? attendanceDate.Add(checkInTime.Value) : null,
                    ShiftOut = checkOutTime.HasValue ? attendanceDate.Add(checkOutTime.Value) : null,
                    ClientId = command.ClientId,
                    EntityId = command.EntityId,
                    UtSysGen = false,
                    UtIsPartial = false,
                    UtRecordVersion = 1,
                    UtBlock = false
                });

                results.Add(new UploadAttendanceProcessRowResultV3Dto(rowNo, code, true, "Processed", currentId));
            }

            if (entitiesToInsert.Count > 0)
            {
                _context.EmployeeAttendances.AddRange(entitiesToInsert);
                await _context.SaveChangesAsync(cancellationToken);
            }

            var successCount = results.Count(x => x.Success);
            var failedCount = results.Count - successCount;
            return new UploadAttendanceProcessResultV3Dto(results.Count, successCount, failedCount, results);
        }

        private static TimeSpan? ParseTime(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            if (TimeSpan.TryParse(value, out var span)) return span;
            if (DateTime.TryParse(value, out var dateTime)) return dateTime.TimeOfDay;
            return null;
        }
    }
}
