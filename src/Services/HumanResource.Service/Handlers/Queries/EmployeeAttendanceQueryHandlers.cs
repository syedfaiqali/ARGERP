using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class EmployeeAttendanceQueryHandlers :
        IRequestHandler<GetEmployeeAttendanceV3Query, IReadOnlyList<EmployeeAttendanceV3Dto>>,
        IRequestHandler<GetEmployeeAttendanceByIdV3Query, EmployeeAttendanceV3Dto?>,
        IRequestHandler<GetEmployeeAttendanceEmployeesV3Query, IReadOnlyList<EmployeeAttendanceEmployeeDto>>,
        IRequestHandler<GetEmployeeAttendanceActivitiesV3Query, IReadOnlyList<EmployeeAttendanceActivityDto>>
    {
        private static readonly int[] InactiveEmployeeStatusIds = { 126, 127 };
        private readonly HumanResourceDbContext _context;

        public EmployeeAttendanceQueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<EmployeeAttendanceV3Dto>> Handle(GetEmployeeAttendanceV3Query request, CancellationToken cancellationToken)
        {
            var isEmployeeSpecific = request.EmployeeId.HasValue && request.EmployeeId.Value > 0;

            var attendanceQuery =
                from atn in _context.EmployeeAttendances.AsNoTracking()
                join emp in _context.Employees.AsNoTracking() on atn.EmpId equals emp.Id
                join act in _context.AttendanceActivities.AsNoTracking() on atn.AtnActivityId equals act.Id into actGroup
                from act in actGroup.DefaultIfEmpty()
                join dep in _context.Departments.AsNoTracking() on emp.DepartmentId equals dep.Id into depGroup
                from dep in depGroup.DefaultIfEmpty()
                join des in _context.Designations.AsNoTracking() on emp.DesignationId equals des.Id into desGroup
                from des in desGroup.DefaultIfEmpty()
                join shift in _context.Shifts.AsNoTracking() on emp.ShiftId equals shift.Id into shiftGroup
                from shift in shiftGroup.DefaultIfEmpty()
                where emp.ClientId == request.ClientId
                      && (isEmployeeSpecific || !request.EntityId.HasValue || emp.EntityId == request.EntityId.Value)
                      && (atn.UtBlock == false)
                select new { atn, emp, act, dep, des, shift };

            if (request.EmployeeId.HasValue && request.EmployeeId.Value > 0)
            {
                attendanceQuery = attendanceQuery.Where(x => x.atn.EmpId == request.EmployeeId.Value);
            }

            if (request.FromDate.HasValue && request.TillDate.HasValue)
            {
                var fromDate = request.FromDate.Value.Date;
                var tillDate = request.TillDate.Value.Date;
                attendanceQuery = attendanceQuery.Where(x =>
                    x.atn.Dttime.HasValue &&
                    x.atn.Dttime.Value.Date >= fromDate &&
                    x.atn.Dttime.Value.Date <= tillDate);
            }

            var items = await attendanceQuery
                .OrderByDescending(x => x.atn.Dttime)
                .Select(x => new EmployeeAttendanceV3Dto(
                    x.atn.Id,
                    x.atn.EmpId,
                    x.emp.EmployeeCode ?? string.Empty,
                    $"{x.emp.FirstName ?? string.Empty} {x.emp.LastName ?? string.Empty}".Trim().ToUpperInvariant(),
                    x.dep != null ? x.dep.DepStxt ?? string.Empty : string.Empty,
                    x.des != null ? x.des.Stxt ?? string.Empty : string.Empty,
                    x.act != null ? x.act.Id : null,
                    x.act != null ? x.act.Stxt ?? string.Empty : string.Empty,
                    x.atn.Dttime,
                    x.atn.OrgDttime,
                    x.atn.Remarks ?? string.Empty,
                    x.emp.MachineCode ?? string.Empty,
                    x.emp.ShiftId,
                    x.shift != null ? x.shift.Stxt ?? string.Empty : string.Empty
                ))
                .ToListAsync(cancellationToken);

            return items;
        }

        public async Task<EmployeeAttendanceV3Dto?> Handle(GetEmployeeAttendanceByIdV3Query request, CancellationToken cancellationToken)
        {
            var query =
                from atn in _context.EmployeeAttendances.AsNoTracking()
                join emp in _context.Employees.AsNoTracking() on atn.EmpId equals emp.Id
                join act in _context.AttendanceActivities.AsNoTracking() on atn.AtnActivityId equals act.Id into actGroup
                from act in actGroup.DefaultIfEmpty()
                join dep in _context.Departments.AsNoTracking() on emp.DepartmentId equals dep.Id into depGroup
                from dep in depGroup.DefaultIfEmpty()
                join des in _context.Designations.AsNoTracking() on emp.DesignationId equals des.Id into desGroup
                from des in desGroup.DefaultIfEmpty()
                join shift in _context.Shifts.AsNoTracking() on emp.ShiftId equals shift.Id into shiftGroup
                from shift in shiftGroup.DefaultIfEmpty()
                where atn.Id == request.Id
                      && emp.ClientId == request.ClientId
                      && (!request.EntityId.HasValue || emp.EntityId == request.EntityId.Value)
                      && (atn.UtBlock == false)
                select new EmployeeAttendanceV3Dto(
                    atn.Id,
                    atn.EmpId,
                    emp.EmployeeCode ?? string.Empty,
                    $"{emp.FirstName ?? string.Empty} {emp.LastName ?? string.Empty}".Trim().ToUpperInvariant(),
                    dep != null ? dep.DepStxt ?? string.Empty : string.Empty,
                    des != null ? des.Stxt ?? string.Empty : string.Empty,
                    act != null ? act.Id : null,
                    act != null ? act.Stxt ?? string.Empty : string.Empty,
                    atn.Dttime,
                    atn.OrgDttime,
                    atn.Remarks ?? string.Empty,
                    emp.MachineCode ?? string.Empty,
                    emp.ShiftId,
                    shift != null ? shift.Stxt ?? string.Empty : string.Empty
                );

            return await query.FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<EmployeeAttendanceEmployeeDto>> Handle(GetEmployeeAttendanceEmployeesV3Query request, CancellationToken cancellationToken)
        {
            var query =
                from emp in _context.Employees.AsNoTracking()
                join dep in _context.Departments.AsNoTracking() on emp.DepartmentId equals dep.Id into depGroup
                from dep in depGroup.DefaultIfEmpty()
                join des in _context.Designations.AsNoTracking() on emp.DesignationId equals des.Id into desGroup
                from des in desGroup.DefaultIfEmpty()
                join shift in _context.Shifts.AsNoTracking() on emp.ShiftId equals shift.Id into shiftGroup
                from shift in shiftGroup.DefaultIfEmpty()
                join employeeType in _context.Lovs.AsNoTracking().Where(x => x.Vtype == "EMPTP" && !x.UtBlock)
                    on emp.EmployeeTypeId equals employeeType.Id into employeeTypeGroup
                from employeeType in employeeTypeGroup.DefaultIfEmpty()
                where emp.ClientId == request.ClientId
                      && (!request.EntityId.HasValue || emp.EntityId == request.EntityId.Value)
                      && (emp.UtBlock == null || emp.UtBlock == false)
                      && (!emp.StatusId.HasValue || !InactiveEmployeeStatusIds.Contains(emp.StatusId.Value))
                select new
                {
                    emp.Id,
                    emp.EmployeeCode,
                    emp.FirstName,
                    emp.LastName,
                    Department = dep != null ? dep.DepStxt : null,
                    Designation = des != null ? des.Stxt : null,
                    emp.MachineCode,
                    emp.ShiftId,
                    Shift = shift != null ? shift.Stxt : null,
                    emp.EmployeeTypeId,
                    EmployeeType = employeeType != null ? employeeType.Stxt : null
                };

            var rows = await query.ToListAsync(cancellationToken);
            return rows
                .OrderBy(x => x.EmployeeCode)
                .Select(x => new EmployeeAttendanceEmployeeDto(
                    x.Id,
                    x.EmployeeCode ?? string.Empty,
                    $"{x.FirstName ?? string.Empty} {x.LastName ?? string.Empty}".Trim().ToUpperInvariant(),
                    x.Department ?? string.Empty,
                    x.Designation ?? string.Empty,
                    x.MachineCode ?? string.Empty,
                    x.ShiftId,
                    x.Shift ?? string.Empty,
                    x.EmployeeTypeId,
                    x.EmployeeType ?? string.Empty
                ))
                .ToList();
        }

        public async Task<IReadOnlyList<EmployeeAttendanceActivityDto>> Handle(GetEmployeeAttendanceActivitiesV3Query request, CancellationToken cancellationToken)
        {
            return await _context.AttendanceActivities
                .AsNoTracking()
                .Where(x => (x.ClientId == null || x.ClientId == request.ClientId) && x.UtBlock == false)
                .OrderBy(x => x.OrderId ?? int.MaxValue)
                .ThenBy(x => x.Stxt)
                .Select(x => new EmployeeAttendanceActivityDto(x.Id, x.Stxt ?? string.Empty))
                .ToListAsync(cancellationToken);
        }
    }
}
