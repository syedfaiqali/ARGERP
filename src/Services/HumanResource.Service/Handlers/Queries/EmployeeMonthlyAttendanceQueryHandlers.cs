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
    public sealed class EmployeeMonthlyAttendanceQueryHandlers :
        IRequestHandler<GetEmployeeMonthlyAttendanceProcessV3Query, IReadOnlyList<EmployeeMonthlyAttendanceV3Dto>>,
        IRequestHandler<GetPayrollPeriodDetailsByRegisterV3Query, IReadOnlyList<PayrollPeriodDetailLookupV3Dto>>
    {
        private const int LeaveTransactionApprovedStatus = 1;
        private const double CeoApprovalThresholdDays = 7;
        private readonly HumanResourceDbContext _context;

        public EmployeeMonthlyAttendanceQueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<EmployeeMonthlyAttendanceV3Dto>> Handle(GetEmployeeMonthlyAttendanceProcessV3Query request, CancellationToken cancellationToken)
        {
            if (request.PeriodDetailId <= 0)
            {
                return Array.Empty<EmployeeMonthlyAttendanceV3Dto>();
            }

            var periodDetail = await _context.PayrollPeriodDetails.AsNoTracking()
                .Include(x => x.Header)
                .FirstOrDefaultAsync(x =>
                        x.Id == request.PeriodDetailId
                        && x.Header != null
                        && x.Header.ClientId == request.ClientId
                        && !x.Header.Utblock
                        && !x.utblock
                        && (!request.EntityId.HasValue || x.Header.EntityId == request.EntityId.Value || x.Header.EntityId == null),
                    cancellationToken);

            if (periodDetail == null || periodDetail.Header == null)
            {
                return Array.Empty<EmployeeMonthlyAttendanceV3Dto>();
            }

            if (!periodDetail.Startdate.HasValue || !periodDetail.Enddate.HasValue)
            {
                return Array.Empty<EmployeeMonthlyAttendanceV3Dto>();
            }

            var startDate = periodDetail.Startdate.Value.Date;
            var endDate = periodDetail.Enddate.Value.Date;

            var leaveByEmployee = await (
                from le in _context.LeaveTransactions.AsNoTracking()
                join lr in _context.LeaveRequests.AsNoTracking() on le.RequestId equals lr.Id into requestGroup
                from lr in requestGroup.DefaultIfEmpty()
                let leaveDays = le.LeaveDays ?? lr.LeaveDays ?? 0
                let fromDate = le.FromDate ?? lr.FromDate
                let tillDate = le.TillDate ?? lr.TillDate
                where le.ClientId == request.ClientId
                      && (!request.EntityId.HasValue || le.EntityId == request.EntityId.Value)
                      && (request.EmployeeId == null || request.EmployeeId == 0 || le.EmployeeId == request.EmployeeId)
                      && fromDate.HasValue
                      && tillDate.HasValue
                      && fromDate.Value.Date >= startDate
                      && tillDate.Value.Date <= endDate
                      && le.StatusId == LeaveTransactionApprovedStatus
                      && (leaveDays <= CeoApprovalThresholdDays || (le.AdmApproval ?? 0) > 0)
                      && !le.UtBlock
                group leaveDays by le.EmployeeId ?? 0
                into employeeLeave
                select new
                {
                    EmployeeId = employeeLeave.Key,
                    LeaveDays = employeeLeave.Sum()
                })
                .ToListAsync(cancellationToken);
            var dailyRows = await new EmployeeDailyAttendanceQueryHandlers(_context).Handle(
                new GetEmployeeDailyAttendanceV3Query(request.ClientId, request.EntityId, request.EmployeeId,
                    startDate, endDate, request.DepartmentId, request.DesignationId), cancellationToken);
            var employeeIds = dailyRows.Where(x => x.EmployeeId.HasValue).Select(x => x.EmployeeId!.Value).Distinct().ToList();
            var employees = await _context.Employees.AsNoTracking()
                .Where(x => employeeIds.Contains(x.Id) && x.ClientId == request.ClientId).ToDictionaryAsync(x => x.Id, cancellationToken);
            var departmentIds = dailyRows.Where(x => x.DepartmentId.HasValue).Select(x => x.DepartmentId!.Value).Distinct().ToList();
            var designationIds = dailyRows.Where(x => x.DesignationId.HasValue).Select(x => x.DesignationId!.Value).Distinct().ToList();
            var departments = await _context.Departments.AsNoTracking().Where(x => departmentIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);
            var designations = await _context.Designations.AsNoTracking().Where(x => designationIds.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, cancellationToken);
            var rules = await AttendanceShiftRules.LoadAsync(_context, request.ClientId, employeeIds, startDate, endDate, cancellationToken);

            return dailyRows
                .Where(x => x.EmployeeId.HasValue && x.AtnDate.HasValue && employees.ContainsKey(x.EmployeeId.Value) &&
                    (!employees[x.EmployeeId.Value].JoiningDate.HasValue ||
                     employees[x.EmployeeId.Value].JoiningDate!.Value.Date <= x.AtnDate.Value.Date))
                .GroupBy(x => x.EmployeeId!.Value)
                .Select(grp =>
                {
                    var first = grp.First();
                    var leaveDays = leaveByEmployee.FirstOrDefault(x => x.EmployeeId == grp.Key)?.LeaveDays ?? 0;
                    var leaves = Convert.ToInt32(leaveDays);
                    var totalLate = grp.Sum(x => x.LateCount ?? 0);
                    // Each employee's date-effective shift determines their policy. Never
                    // combine another employee's thresholds, including after a roster change.
                    var lateAbsent = grp.GroupBy(x => rules.LatePolicyId(grp.Key, x.AtnDate!.Value, x.ShiftId))
                        .Sum(policyRows => rules.LateAbsences(policyRows.Key, policyRows.Sum(x => x.LateCount ?? 0)));
                    var absents = Math.Max(0, grp.Count(x => x.IsAbsent && !x.IsHoliday) - leaves)
                        + lateAbsent + grp.Sum(x => x.LateAbsent ?? 0);
                    var presents = grp.Count(x => !x.IsHoliday && (x.ActShiftIn != null || x.ActShiftOut != null));
                    var holidays = grp.Count(x => x.IsHoliday);
                    return new EmployeeMonthlyAttendanceV3Dto(
                        grp.Key, first.EmployeeCode, first.EmployeeName,
                        first.DepartmentId.HasValue && departments.TryGetValue(first.DepartmentId.Value, out var dep) ? dep.DepStxt ?? string.Empty : string.Empty,
                        first.DesignationId.HasValue && designations.TryGetValue(first.DesignationId.Value, out var desg) ? desg.Stxt ?? string.Empty : string.Empty,
                        periodDetail.Noofdays, presents, absents, holidays, presents, holidays, leaves,
                        totalLate, grp.Where(x => !x.IsHoliday).Sum(x => x.EdCount ?? 0), grp.Sum(x => x.LateHalfDays ?? 0),
                        periodDetail.Header.Payrollregisterid, periodDetail.Payperiodid, periodDetail.Id,
                        periodDetail.Startdate, periodDetail.Enddate, periodDetail.Disbdate, periodDetail.Header.EntityId);
                }).ToList();
        }
        public async Task<IReadOnlyList<PayrollPeriodDetailLookupV3Dto>> Handle(GetPayrollPeriodDetailsByRegisterV3Query request, CancellationToken cancellationToken)
        {
            if (request.RegisterId <= 0)
            {
                return Array.Empty<PayrollPeriodDetailLookupV3Dto>();
            }

            var headerIds = await _context.PayrollPeriods.AsNoTracking()
                .Where(x => x.Payrollregisterid == request.RegisterId
                            && x.ClientId == request.ClientId
                            && !x.Utblock
                            && (!request.EntityId.HasValue || x.EntityId == request.EntityId.Value || x.EntityId == null))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            if (headerIds.Count == 0)
            {
                return Array.Empty<PayrollPeriodDetailLookupV3Dto>();
            }

            var details = await _context.PayrollPeriodDetails.AsNoTracking()
                .Where(d => d.Payperiodid.HasValue && headerIds.Contains(d.Payperiodid.Value) && !d.utblock)
                .OrderByDescending(d => d.Startdate)
                .Select(d => new PayrollPeriodDetailLookupV3Dto(
                    d.Id,
                    d.Payperiodid,
                    d.Periodstxt ?? string.Empty,
                    d.Startdate,
                    d.Enddate,
                    d.Noofdays,
                    d.Disbdate
                ))
                .ToListAsync(cancellationToken);

            return details;
        }

    }
}
