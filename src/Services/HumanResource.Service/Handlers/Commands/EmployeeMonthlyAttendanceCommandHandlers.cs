using HumanResource.Service.Commands;
using HumanResource.Service.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class EmployeeMonthlyAttendanceCommandHandlers :
        IRequestHandler<SaveEmployeeMonthlyAttendanceV3Command, bool>,
        IRequestHandler<RevertEmployeeMonthlyAttendanceV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;

        public EmployeeMonthlyAttendanceCommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(SaveEmployeeMonthlyAttendanceV3Command request, CancellationToken cancellationToken)
        {
            if (request.Items == null || request.Items.Count == 0)
            {
                return false;
            }

            var employeeIds = request.Items.Select(x => x.EmployeeId).Distinct().ToList();
            var employees = await _context.Employees.AsNoTracking()
                .Where(x => employeeIds.Contains(x.Id) && x.ClientId == request.ClientId)
                .ToListAsync(cancellationToken);

            var periodDetailIds = request.Items
                .Select(x => x.PayPeriodDetailId ?? 0)
                .Where(x => x > 0)
                .Distinct()
                .ToList();

            var periodDetails = await _context.PayrollPeriodDetails.AsNoTracking()
                .Include(x => x.Header)
                .Where(x => periodDetailIds.Contains(x.Id) && x.Header != null && x.Header.ClientId == request.ClientId)
                .ToListAsync(cancellationToken);

            var payPeriodIds = request.Items
                .Select(x => x.PayPeriodId ?? 0)
                .Where(x => x > 0)
                .Distinct()
                .ToList();

            var existingRows = await _context.MonthlyAttendances
                .Where(x => x.EmployeeId.HasValue
                            && employeeIds.Contains(x.EmployeeId.Value)
                            && (!request.EntityId.HasValue || x.EntityId == request.EntityId.Value)
                            && ((x.PayPeriodDetailId.HasValue && periodDetailIds.Contains(x.PayPeriodDetailId.Value))
                                || (x.PayPeriodId.HasValue && payPeriodIds.Contains(x.PayPeriodId.Value))))
                .ToListAsync(cancellationToken);

            var existingByKey = existingRows
                .GroupBy(x => (EmployeeId: x.EmployeeId ?? 0, x.PayPeriodDetailId, x.PayPeriodId))
                .ToDictionary(x => x.Key, x => x.First());

            var nextId = (await _context.MonthlyAttendances.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            foreach (var item in request.Items)
            {
                if (!employees.Any(x => x.Id == item.EmployeeId))
                {
                    throw new InvalidOperationException($"Employee {item.EmployeeId} is not valid for this client.");
                }

                if (request.EntityId.HasValue)
                {
                    var employee = employees.First(x => x.Id == item.EmployeeId);
                    if (employee.EntityId != request.EntityId.Value)
                    {
                        throw new InvalidOperationException($"Employee {item.EmployeeId} does not belong to the selected entity.");
                    }
                }

                if (item.PayPeriodDetailId.HasValue)
                {
                    var detail = periodDetails.FirstOrDefault(x => x.Id == item.PayPeriodDetailId.Value);
                    if (detail == null)
                    {
                        throw new InvalidOperationException("Payroll period detail not found for this client.");
                    }
                }

                var key = (EmployeeId: item.EmployeeId, item.PayPeriodDetailId, item.PayPeriodId);
                existingByKey.TryGetValue(key, out var existing);

                if (existing == null)
                {
                    existing = new Entities.HRMonthlyAttendanceLegacy
                    {
                        Id = nextId++,
                        UtBlock = false
                    };
                    _context.MonthlyAttendances.Add(existing);
                    existingByKey[key] = existing;
                }

                existing.EmployeeId = item.EmployeeId;
                existing.RegisterId = item.RegisterId;
                existing.PayPeriodId = item.PayPeriodId;
                existing.PayPeriodDetailId = item.PayPeriodDetailId;
                existing.StartDate = item.StartDate;
                existing.EndDate = item.EndDate;
                existing.DisbDate = item.DisbDate;
                existing.EntityId = item.EntityId ?? request.EntityId;
                existing.Days = item.Days;
                existing.PresentsCount = item.PresentsCount;
                existing.AbsentsCount = item.AbsentsCount;
                existing.HalfDaysCount = item.HalfDaysCount;
                existing.LateCount = item.LateCount;
                existing.EarlyCounts = item.EarlyCounts;
                existing.ActualDays = item.ActualDays;
                existing.ActualPresent = item.ActualPresent;
                existing.Holidays = item.Holidays;
                existing.Leaves = item.Leaves;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Handle(RevertEmployeeMonthlyAttendanceV3Command request, CancellationToken cancellationToken)
        {
            var employeeExists = await _context.Employees.AsNoTracking()
                .AnyAsync(x => x.Id == request.EmployeeId
                               && x.ClientId == request.ClientId
                               && (!request.EntityId.HasValue || x.EntityId == request.EntityId.Value),
                    cancellationToken);

            if (!employeeExists)
            {
                return false;
            }

            var record = await _context.MonthlyAttendances
                .FirstOrDefaultAsync(x => x.EmployeeId == request.EmployeeId
                                          && x.RegisterId == request.RegisterId
                                          && x.PayPeriodDetailId == request.PeriodDetailId
                                          && (!request.EntityId.HasValue || x.EntityId == request.EntityId.Value),
                    cancellationToken);

            if (record == null)
            {
                return false;
            }

            _context.MonthlyAttendances.Remove(record);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
