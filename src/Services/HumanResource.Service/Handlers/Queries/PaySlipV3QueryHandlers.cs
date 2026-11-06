using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Entities;
using MediatR;
using EDXS.Shared.Common.Payroll;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HumanResource.Service.Handlers.Queries
{
    public record GetPaySlipProcessQuery(PaySlipFetchRequestV3 Request, int ClientId, int EntityId) : IRequest<List<PaySlipV3Dto>>;

    public class PaySlipV3QueryHandlers : IRequestHandler<GetPaySlipProcessQuery, List<PaySlipV3Dto>>
    {
        private const int DraftStatusId = 397;
        // Older issued loans retain the legacy Approved status. New workflow loans
        // move to Loan Disbursed / Completed after Finance creates the issuance.
        private const int LegacyLoanStatusApproved = 582;
        private readonly HumanResourceDbContext _context;

        public PaySlipV3QueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<List<PaySlipV3Dto>> Handle(GetPaySlipProcessQuery query, CancellationToken cancellationToken)
        {
            var req = query.Request;

            var period = await _context.PayrollPeriodDetails.AsNoTracking()
                .Include(x => x.Header)
                .FirstOrDefaultAsync(x =>
                    x.Id == req.periodid
                    && x.Header.Payrollregisterid == req.registerid
                    && x.Header.ClientId == query.ClientId
                    && (!x.Header.EntityId.HasValue || x.Header.EntityId == query.EntityId),
                    cancellationToken);

            if (period == null)
            {
                return new List<PaySlipV3Dto>();
            }

            var fromDate = (req.fromdate ?? period.Startdate)?.Date;
            var tillDate = (req.tilldate ?? period.Enddate)?.Date;
            if (!fromDate.HasValue || !tillDate.HasValue)
            {
                return new List<PaySlipV3Dto>();
            }

            var periodDays = period.Noofdays ?? (tillDate.Value - fromDate.Value).Days + 1;

            var attendanceQuery = _context.MonthlyAttendances
                .AsNoTracking()
                .Where(x =>
                    x.EntityId == query.EntityId
                    && x.RegisterId == req.registerid
                    && x.PayPeriodDetailId == req.periodid
                    && !x.UtBlock)
                .AsQueryable();

            if (req.employeeid.HasValue && req.employeeid > 0)
                attendanceQuery = attendanceQuery.Where(x => x.EmployeeId == req.employeeid);

            var monthlyAttendance = await attendanceQuery.ToListAsync(cancellationToken);
            var attendanceByEmployee = monthlyAttendance
                .Where(x => x.EmployeeId.HasValue)
                .GroupBy(x => x.EmployeeId!.Value)
                .ToDictionary(x => x.Key, x => x.First());

            var employeeQuery =
                from emp in _context.Employees.AsNoTracking()
                join payroll in _context.EmployeePayrolls.AsNoTracking() on emp.Id equals payroll.EmployeeId
                join dep in _context.Departments.AsNoTracking() on emp.DepartmentId equals dep.Id into depGroup
                from dep in depGroup.DefaultIfEmpty()
                join desg in _context.Designations.AsNoTracking() on emp.DesignationId equals desg.Id into desgGroup
                from desg in desgGroup.DefaultIfEmpty()
                where emp.ClientId == query.ClientId
                      && emp.EntityId == query.EntityId
                      && payroll.UtBlock == false
                      && emp.UtBlock != true
                      && (!req.employeeid.HasValue || req.employeeid == 0 || emp.Id == req.employeeid)
                      && (!req.deptid.HasValue || req.deptid == 0 || emp.DepartmentId == req.deptid)
                      && (!req.desgid.HasValue || req.desgid == 0 || emp.DesignationId == req.desgid)
                select new
                {
                    emp.Id,
                    emp.EmployeeCode,
                    emp.FirstName,
                    emp.LastName,
                    emp.DepartmentId,
                    Department = dep != null ? dep.DepStxt : string.Empty,
                    emp.DesignationId,
                    Designation = desg != null ? desg.Stxt : string.Empty,
                    emp.JoiningDate,
                    payroll.PayFullSalary
                };

            var employees = await employeeQuery
                .OrderBy(x => x.FirstName)
                .ToListAsync(cancellationToken);

            var attendanceEmployeeIds = attendanceByEmployee.Keys.ToHashSet();
            if (attendanceEmployeeIds.Count == 0)
            {
                return new List<PaySlipV3Dto>();
            }

            employees = employees
                .Where(x => attendanceEmployeeIds.Contains(x.Id))
                .ToList();

            var employeeIds = employees.Select(x => x.Id).ToList();
            if (employeeIds.Count == 0)
            {
                return new List<PaySlipV3Dto>();
            }

            var baseSalaries = await _context.EmployeePayrollBaseSalaries.AsNoTracking()
                .Where(x => x.Payroll != null
                            && x.Payroll.EmployeeId.HasValue
                            && employeeIds.Contains(x.Payroll.EmployeeId.Value)
                            && x.UtBlock == false
                            && x.StartDate.HasValue
                            && x.EndDate.HasValue
                            && ((fromDate.Value <= x.StartDate.Value.Date && tillDate.Value >= x.EndDate.Value.Date)
                                || (fromDate.Value >= x.StartDate.Value.Date && tillDate.Value <= x.EndDate.Value.Date)
                                || (x.StartDate.Value.Date <= tillDate.Value && x.EndDate.Value.Date >= fromDate.Value)))
                .Select(x => new PayrollAmountLine(x.Payroll!.EmployeeId!.Value, x.EarningTypeId, 1079, x.StartDate, x.EndDate, x.Value ?? 0, false))
                .ToListAsync(cancellationToken);

            var allowances = await _context.EmployeePayrollAllowances.AsNoTracking()
                .Where(x => x.Payroll != null
                            && x.Payroll.EmployeeId.HasValue
                            && employeeIds.Contains(x.Payroll.EmployeeId.Value)
                            && x.UtBlock == false
                            && x.StartDate.HasValue
                            && x.EndDate.HasValue
                            && ((fromDate.Value <= x.StartDate.Value.Date && tillDate.Value >= x.EndDate.Value.Date)
                                || (fromDate.Value >= x.StartDate.Value.Date && tillDate.Value <= x.EndDate.Value.Date)
                                || (x.StartDate.Value.Date <= tillDate.Value && x.EndDate.Value.Date >= fromDate.Value)))
                .Select(x => new PayrollAmountLine(x.Payroll!.EmployeeId!.Value, x.EarningTypeId, 1080, x.StartDate, x.EndDate, x.Value ?? 0, x.IsPercentage))
                .ToListAsync(cancellationToken);

            var deductions = await _context.EmployeePayrollDeductions.AsNoTracking()
                .Where(x => x.Payroll != null
                            && x.Payroll.EmployeeId.HasValue
                            && employeeIds.Contains(x.Payroll.EmployeeId.Value)
                            && x.UtBlock == false
                            && x.StartDate.HasValue
                            && x.EndDate.HasValue
                            && ((fromDate.Value <= x.StartDate.Value.Date && tillDate.Value >= x.EndDate.Value.Date)
                                || (fromDate.Value >= x.StartDate.Value.Date && tillDate.Value <= x.EndDate.Value.Date)
                                || (x.StartDate.Value.Date <= tillDate.Value && x.EndDate.Value.Date >= fromDate.Value)))
                .Select(x => new PayrollAmountLine(x.Payroll!.EmployeeId!.Value, x.DeductionTypeId, 1081, x.StartDate, x.EndDate, x.Value ?? 0, false))
                .ToListAsync(cancellationToken);

            var eligibleLoanRequestStatusIds = await _context.Lovs.AsNoTracking()
                .Where(x => x.Vtype == "TRTP"
                            && !x.UtBlock
                            && (x.Stxt == "Approved" || x.Stxt == "Loan Disbursed / Completed"))
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            // Retain historical issuances created before the workflow status LOVs
            // were introduced, where Approved was stored as 582.
            if (!eligibleLoanRequestStatusIds.Contains(LegacyLoanStatusApproved))
            {
                eligibleLoanRequestStatusIds.Add(LegacyLoanStatusApproved);
            }

            var loanDeductions = await (from request in _context.LoanRequests.AsNoTracking()
                                        join header in _context.LoanIssuances.AsNoTracking() on request.Id equals header.requestid
                                        join detail in _context.LoanIssuanceDetails.AsNoTracking() on header.Id equals detail.loanid
                                        where request.entityid == query.EntityId
                                              && request.clientid == query.ClientId
                                              && request.statusid.HasValue
                                              && eligibleLoanRequestStatusIds.Contains(request.statusid.Value)
                                              && request.employeeid.HasValue
                                              && employeeIds.Contains(request.employeeid.Value)
                                              && detail.monthid == period.Monthid
                                              && detail.year == period.Year
                                              && detail.notdeduct == false
                                        group detail by request.employeeid into grp
                                        select new
                                        {
                                            EmployeeId = grp.Key!.Value,
                                            Amount = grp.Sum(x => x.installment ?? 0)
                                        })
                .ToDictionaryAsync(x => x.EmployeeId, x => x.Amount, cancellationToken);

            var processed = await _context.PayrollProcesses
                .AsNoTracking()
                .Where(x =>
                    x.clientid == query.ClientId
                    && x.entityid == query.EntityId
                    && x.perioddtlid == req.periodid
                    && x.registerid == req.registerid
                    && x.utblock == false)
                .ToListAsync(cancellationToken);

            var result = new List<PaySlipV3Dto>();
            foreach (var emp in employees)
            {
                attendanceByEmployee.TryGetValue(emp.Id, out var atn);

                var salary = baseSalaries.Where(x => x.EmployeeId == emp.Id).Sum(x => x.Value);
                var allowance = allowances.Where(x => x.EmployeeId == emp.Id)
                    .Sum(x => AllowanceCalculation.Resolve(x.Value, x.IsPercentage, salary));
                var employeeDeduction = deductions.Where(x => x.EmployeeId == emp.Id).Sum(x => x.Value);
                var loanDeduction = loanDeductions.TryGetValue(emp.Id, out var loanAmount) ? loanAmount : 0;
                var absents = atn?.AbsentsCount ?? 0;
                var absentDeduction = emp.PayFullSalary ? 0 : (salary + allowance) * 12 / 365 * absents;
                var deduction = emp.PayFullSalary ? employeeDeduction : employeeDeduction + absentDeduction;
                var grossSalary = salary + allowance - deduction - loanDeduction;

                var item = new PaySlipV3Dto
                {
                    employeeid = emp.Id,
                    Name = $"{emp.FirstName ?? string.Empty} {emp.LastName ?? string.Empty}".Trim().ToUpperInvariant(),
                    deptId = emp.DepartmentId,
                    departmentstxt = emp.Department ?? string.Empty,
                    designationid = emp.DesignationId,
                    designationstxt = emp.Designation ?? string.Empty,
                    days = Convert.ToInt32(atn?.Days ?? periodDays),
                    present = Convert.ToInt32(atn?.PresentsCount ?? 0),
                    absentscount = Convert.ToInt32(absents),
                    holidays = Convert.ToInt32(atn?.Holidays ?? 0),
                    leaves = atn?.Leaves ?? 0,
                    salary = salary,
                    allowance = allowance,
                    deduction = deduction,
                    loanddeduction = loanDeduction,
                    grosssalary = grossSalary,
                    tax = 0,
                    arrears = 0,
                    othded = 0,
                    statusid = DraftStatusId,
                    entityid = query.EntityId,
                    registerid = req.registerid,
                    periodid = period.Payperiodid,
                    perioddtlid = req.periodid,
                    salarytypeid = req.salarytypeid,
                    processtypeid = req.processtypeid
                };

                var p = processed.FirstOrDefault(x => x.employeeid == emp.Id);
                if (p != null)
                {
                    item.Id = p.Id;
                    item.salary = p.salary;
                    item.allowance = p.allowance;
                    item.deduction = p.deduction;
                    item.tax = p.tax;
                    // Recalculate loan recovery from the current installment schedule.
                    // This lets a payroll row saved before a workflow-status change be
                    // corrected by Fetch and then persisted again through Process.
                    item.grosssalary = (item.salary ?? 0) + (item.allowance ?? 0)
                        - (item.deduction ?? 0) - (item.loanddeduction ?? 0);
                    item.remarks = p.remarks;
                    item.statusid = p.statusid;
                    item.arrears = p.arrears;
                    item.othded = p.othded;
                    item.fixedded = p.fixedded;
                    item.absded = p.absded;
                    item.overtime = p.overtime;
                    item.feededuction = p.feededuction;
                    item.registerid = p.registerid;
                    item.periodid = p.periodid;
                    item.perioddtlid = p.perioddtlid;
                    item.salarytypeid = p.salarytypeid;
                    item.processtypeid = p.processtypeid;
                }

                result.Add(item);
            }

            return result;
        }

        private sealed record PayrollAmountLine(
            int EmployeeId,
            int? TypeId,
            int LovTypeId,
            DateTime? StartDate,
            DateTime? EndDate,
            double Value,
            bool IsPercentage);
    }
}
