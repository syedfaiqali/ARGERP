using HumanResource.Service.PayrollAccounting;
using EDXS.Shared.Common.Payroll;
using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Entities;
using HumanResource.Service.Posting;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HumanResource.Service.Handlers.Commands
{
    public record SavePaySlipProcessCommand(PaySlipV3Dto Request, int ClientId, int EntityId) : IRequest<PaySlipV3Dto>;

    public class PaySlipV3CommandHandlers : IRequestHandler<SavePaySlipProcessCommand, PaySlipV3Dto>
    {
        private const int BaseSalaryLovTypeId = 1079;
        private const int AllowanceLovTypeId = 1080;
        private const int DeductionLovTypeId = 1081;
        private const int DraftStatusId = 397;
        private readonly HumanResourceDbContext _context;
        private readonly IPublisher? _publisher;

        public PaySlipV3CommandHandlers(HumanResourceDbContext context, IPublisher? publisher = null)
        {
            _context = context;
            _publisher = publisher;
        }

        // The payroll JV is posted inside the save transaction: if it fails, the pay slip is not saved.
        public Task<PaySlipV3Dto> Handle(SavePaySlipProcessCommand command, CancellationToken cancellationToken) =>
            FinanceTransaction.ExecuteAsync(_context, async ct =>
            {
                var saved = await SaveAsync(command, ct);
                if (_publisher != null)
                    await _publisher.Publish(new PayrollProcessedEvent(saved.Id, command.ClientId), ct);
                return saved;
            }, cancellationToken);

        private async Task<PaySlipV3Dto> SaveAsync(SavePaySlipProcessCommand command, CancellationToken cancellationToken)
        {
            var req = command.Request;
            var period = req.perioddtlid.HasValue
                ? await _context.PayrollPeriodDetails.AsNoTracking()
                    .Include(x => x.Header)
                    .FirstOrDefaultAsync(x =>
                        x.Id == req.perioddtlid.Value
                        && x.Header.Payrollregisterid == req.registerid
                        && x.Header.ClientId == command.ClientId
                        && (!x.Header.EntityId.HasValue || x.Header.EntityId == command.EntityId),
                        cancellationToken)
                : null;

            if (period == null)
            {
                throw new InvalidOperationException("Payroll period does not exist against selected register.");
            }

            var fromDate = period?.Startdate ?? DateTime.Today;
            var tillDate = period?.Enddate ?? DateTime.Today;
            var payPeriodId = req.periodid ?? period.Payperiodid;

            if (req.Id <= 0 && req.employeeid.HasValue)
            {
                var alreadyExists = await _context.PayrollProcesses.AsNoTracking()
                    .AnyAsync(x =>
                        x.clientid == command.ClientId
                        && x.entityid == command.EntityId
                        && x.employeeid == req.employeeid
                        && x.periodid == payPeriodId
                        && x.perioddtlid == req.perioddtlid
                        && x.deptid == req.deptId
                        && x.utblock == false,
                        cancellationToken);

                if (alreadyExists)
                {
                    throw new InvalidOperationException("Payroll Record already exists against employee.");
                }
            }

            var entity = await _context.PayrollProcesses
                .Include(x => x.HRpayrollprocessdtls)
                .FirstOrDefaultAsync(x => x.Id == req.Id, cancellationToken);

            if (entity == null)
            {
                int nextId = (await _context.PayrollProcesses.IgnoreQueryFilters().Select(x => (int?)x.Id).MaxAsync(cancellationToken) ?? 0) + 1;
                entity = new HRpayrollprocessLegacy
                {
                    Id = nextId,
                    transdate = DateTime.Now,
                    transno = "PS-" + nextId.ToString("D5"),
                    entityid = command.EntityId,
                    clientid = command.ClientId,
                    utsysgen = true
                };
                _context.PayrollProcesses.Add(entity);
            }

            // Map fields
            entity.employeeid = req.employeeid;
            entity.deptid = req.deptId;
            entity.desgid = req.designationid;
            entity.registerid = req.registerid;
            entity.periodid = payPeriodId;
            entity.perioddtlid = req.perioddtlid;
            entity.salarytypeid = req.salarytypeid;
            entity.processtypeid = req.processtypeid;
            entity.days = req.days;
            entity.present = req.present;
            entity.absent = req.absentscount;
            entity.leaves = req.leaves;
            entity.holidays = req.holidays;
            entity.salary = req.salary;
            entity.allowance = req.allowance;
            entity.deduction = req.deduction;
            entity.loanddeduction = req.loanddeduction;
            entity.tax = req.tax;
            entity.grosssalary = req.grosssalary;
            entity.remarks = req.remarks;
            entity.statusid = req.statusid ?? DraftStatusId;
            entity.arrears = req.arrears;
            entity.othded = (int?)req.othded;
            entity.fixedded = req.fixedded ?? entity.fixedded;
            entity.absded = req.absded ?? entity.absded;
            entity.overtime = req.overtime ?? entity.overtime;
            entity.feededuction = req.feededuction ?? entity.feededuction;
            entity.fromdate = fromDate;
            entity.tilldate = tillDate;
            entity.clientid = command.ClientId;
            entity.entityid = command.EntityId;

            if (entity.HRpayrollprocessdtls.Count > 0)
            {
                _context.PayrollProcessDetails.RemoveRange(entity.HRpayrollprocessdtls);
            }

            if (req.employeeid.HasValue)
            {
                var detailLines = await BuildPayrollProcessDetails(req.employeeid.Value, fromDate.Date, tillDate.Date, cancellationToken);
                var nextDetailId = (await _context.PayrollProcessDetails.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
                foreach (var line in detailLines)
                {
                    line.Id = nextDetailId++;
                    line.processid = entity.Id;
                    entity.HRpayrollprocessdtls.Add(line);
                }
            }

            if (req.employeeid.HasValue
                && (req.loanddeduction ?? 0) != 0
                && period.Monthid.HasValue)
            {
                var loanInstallment = await _context.LoanIssuanceDetails
                    .Include(x => x.LoanIssuance)
                    .Where(x =>
                        x.LoanIssuance != null
                        && x.LoanIssuance.employeeid == req.employeeid
                        && x.monthid == period.Monthid
                        && (!period.Year.HasValue || x.year == period.Year)
                        && x.notdeduct == false
                        && x.utblock == false)
                    .OrderBy(x => x.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (loanInstallment != null)
                {
                    loanInstallment.recovered = true;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            
            req.Id = entity.Id;
            return req;
        }

        private async Task<List<HRpayrollprocessdtlLegacy>> BuildPayrollProcessDetails(
            int employeeId,
            DateTime fromDate,
            DateTime tillDate,
            CancellationToken cancellationToken)
        {
            var result = new List<HRpayrollprocessdtlLegacy>();

            var baseSalaries = await _context.EmployeePayrollBaseSalaries.AsNoTracking()
                .Where(x => x.Payroll != null
                            && x.Payroll.EmployeeId == employeeId
                            && x.UtBlock == false
                            && x.StartDate.HasValue
                            && x.EndDate.HasValue
                            && ((fromDate <= x.StartDate.Value.Date && tillDate >= x.EndDate.Value.Date)
                                || (fromDate >= x.StartDate.Value.Date && tillDate <= x.EndDate.Value.Date)
                                || (x.StartDate.Value.Date <= tillDate && x.EndDate.Value.Date >= fromDate)))
                .Select(x => new HRpayrollprocessdtlLegacy
                {
                    lovtypeid = BaseSalaryLovTypeId,
                    typeid = x.EarningTypeId,
                    fromdate = x.StartDate,
                    tilldate = x.EndDate,
                    value = x.Value,
                    utsysgen = true
                })
                .ToListAsync(cancellationToken);

            var allowanceLines = await _context.EmployeePayrollAllowances.AsNoTracking()
                .Where(x => x.Payroll != null
                            && x.Payroll.EmployeeId == employeeId
                            && x.UtBlock == false
                            && x.StartDate.HasValue
                            && x.EndDate.HasValue
                            && ((fromDate <= x.StartDate.Value.Date && tillDate >= x.EndDate.Value.Date)
                                || (fromDate >= x.StartDate.Value.Date && tillDate <= x.EndDate.Value.Date)
                                || (x.StartDate.Value.Date <= tillDate && x.EndDate.Value.Date >= fromDate)))
                .Select(x => new { x.EarningTypeId, x.StartDate, x.EndDate, x.Value, x.IsPercentage })
                .ToListAsync(cancellationToken);
            var baseSalary = baseSalaries.Sum(x => x.value ?? 0d);
            var allowances = allowanceLines.Select(x => new HRpayrollprocessdtlLegacy
            {
                lovtypeid = AllowanceLovTypeId,
                typeid = x.EarningTypeId,
                fromdate = x.StartDate,
                tilldate = x.EndDate,
                value = AllowanceCalculation.Resolve(x.Value, x.IsPercentage, baseSalary),
                utsysgen = true
            });

            var deductions = await _context.EmployeePayrollDeductions.AsNoTracking()
                .Where(x => x.Payroll != null
                            && x.Payroll.EmployeeId == employeeId
                            && x.UtBlock == false
                            && x.StartDate.HasValue
                            && x.EndDate.HasValue
                            && ((fromDate <= x.StartDate.Value.Date && tillDate >= x.EndDate.Value.Date)
                                || (fromDate >= x.StartDate.Value.Date && tillDate <= x.EndDate.Value.Date)
                                || (x.StartDate.Value.Date <= tillDate && x.EndDate.Value.Date >= fromDate)))
                .Select(x => new HRpayrollprocessdtlLegacy
                {
                    lovtypeid = DeductionLovTypeId,
                    typeid = x.DeductionTypeId,
                    fromdate = x.StartDate,
                    tilldate = x.EndDate,
                    value = x.Value,
                    utsysgen = true
                })
                .ToListAsync(cancellationToken);

            result.AddRange(allowances);
            result.AddRange(deductions);
            result.AddRange(baseSalaries);
            return result;
        }
    }
}
