using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace HumanResource.Service.Handlers.Queries
{
    public class PayrollPeriodQueryHandlers :
        IRequestHandler<GetPayrollPeriodsV3Query, PaginationResponse<PayrollPeriodV3Dto>>,
        IRequestHandler<GetPayrollPeriodByIdV3Query, PayrollPeriodHeaderV3Dto?>,
        IRequestHandler<GetPayrollPeriodDetailsV3Query, List<PayrollPeriodDetailV3Dto>>,
        IRequestHandler<GeneratePayrollPeriodDetailsV3Query, List<PayrollPeriodDetailV3Dto>>,
        IRequestHandler<GetNextPayrollRegisterDateV3Query, NextPayrollRegisterDateV3Dto?>
    {
        private readonly HumanResourceDbContext _context;

        public PayrollPeriodQueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<PayrollPeriodV3Dto>> Handle(GetPayrollPeriodsV3Query request, CancellationToken cancellationToken)
        {
            // The list DTO projects header columns only, so the Details collection is
            // deliberately not Included here: loading it multiplied every header row by
            // its period-detail count for data the grid never shows.
            var query = _context.PayrollPeriods
                .Where(x => x.ClientId == request.ClientId && !x.Utblock);

            if (request.EntityId.HasValue)
            {
                query = query.Where(x => x.EntityId == request.EntityId.Value || x.EntityId == null);
            }

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(x => (x.Code != null && x.Code.Contains(request.SearchTerm)) || (x.Ltxt != null && x.Ltxt.Contains(request.SearchTerm)));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            // Manual pagination after sorting. The three display names used to be
            // correlated subqueries inside the projection, so SQL Server resolved them
            // once per returned row (three extra lookups per row). The page is read
            // first, then the names are resolved with two batched lookups.
            var headers = await query
                .OrderByDescending(x => x.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new
                {
                    x.Id,
                    x.Code,
                    x.Ltxt,
                    x.Payrollregisterid,
                    x.Atntypeid,
                    x.Startdate,
                    x.Noofperiod,
                    x.Statusid
                })
                .ToListAsync(cancellationToken);

            var registerIds = headers
                .Where(x => x.Payrollregisterid.HasValue)
                .Select(x => x.Payrollregisterid!.Value)
                .Distinct()
                .ToList();

            var registerNames = registerIds.Count == 0
                ? new Dictionary<int, string>()
                : await _context.PayrollRegisterLegacy
                    .Where(pr => registerIds.Contains(pr.Id))
                    .Select(pr => new { pr.Id, pr.Stxt })
                    .ToDictionaryAsync(pr => pr.Id, pr => pr.Stxt ?? "", cancellationToken);

            // Attendance type and status both come from the LOV table, so they are
            // fetched together rather than as two separate round trips.
            var lovIds = headers
                .SelectMany(x => new[] { x.Atntypeid, x.Statusid })
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();

            var lovNames = lovIds.Count == 0
                ? new Dictionary<int, string>()
                : await _context.Lovs
                    .Where(l => lovIds.Contains(l.Id))
                    .Select(l => new { l.Id, l.Stxt })
                    .ToDictionaryAsync(l => l.Id, l => l.Stxt ?? "", cancellationToken);

            string LookupRegister(int? id) =>
                id.HasValue && registerNames.TryGetValue(id.Value, out var name) ? name : "";

            string LookupLov(int? id) =>
                id.HasValue && lovNames.TryGetValue(id.Value, out var name) ? name : "";

            var items = headers
                .Select(x => new PayrollPeriodV3Dto(
                    x.Id,
                    x.Code ?? "",
                    x.Ltxt ?? "",
                    LookupRegister(x.Payrollregisterid),
                    LookupLov(x.Atntypeid),
                    x.Startdate,
                    x.Noofperiod,
                    LookupLov(x.Statusid)
                ))
                .ToList();

            return new PaginationResponse<PayrollPeriodV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<PayrollPeriodHeaderV3Dto?> Handle(GetPayrollPeriodByIdV3Query request, CancellationToken cancellationToken)
        {
            var header = await _context.PayrollPeriods
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (header == null) return null;

            return new PayrollPeriodHeaderV3Dto(
                header.Id,
                header.Code ?? "",
                header.Date,
                header.Atntypeid,
                header.Fromyear,
                header.Toyear,
                header.Ltxt ?? "",
                header.Startfrom,
                header.Statusid,
                header.Lovsharedid,
                header.Startdate,
                header.Payrollregisterid,
                header.Noofperiod,
                !header.Utblock,
                header.Details
                    .OrderBy(d => d.Lineindex ?? int.MaxValue)
                    .ThenBy(d => d.Startdate)
                    .ThenBy(d => d.Id)
                    .Select(d => new PayrollPeriodDetailV3Dto(
                    d.Id,
                    d.Payperiodid,
                    d.Periodstxt ?? "",
                    d.Year,
                    d.Startdate,
                    d.Enddate,
                    d.Noofdays,
                    d.Disbdate,
                    d.Statusid,
                    d.Lineindex
                )).ToList()
            );
        }

        public async Task<List<PayrollPeriodDetailV3Dto>> Handle(GetPayrollPeriodDetailsV3Query request, CancellationToken cancellationToken)
        {
            var details = await _context.PayrollPeriodDetails
                .Where(x => x.Payperiodid == request.PayPeriodId)
                .Select(x => new PayrollPeriodDetailV3Dto(
                    x.Id,
                    x.Payperiodid,
                    x.Periodstxt ?? (x.Startdate.HasValue ? x.Startdate.Value.ToString("MMMM") : ""),
                    x.Year,
                    x.Startdate,
                    x.Enddate,
                    x.Noofdays,
                    x.Disbdate,
                    x.Statusid,
                    x.Lineindex
                ))
                .ToListAsync(cancellationToken);

            return details
                .OrderBy(x => x.LineIndex ?? int.MaxValue)
                .ThenBy(x => x.StartDate)
                .ThenBy(x => x.Id)
                .ToList();
        }

        public async Task<List<PayrollPeriodDetailV3Dto>> Handle(GeneratePayrollPeriodDetailsV3Query request, CancellationToken cancellationToken)
        {
            const int daily = 259;
            const int weekly = 260;
            const int monthly = 261;
            const int yearly = 264;
            const int inactiveStatus = 406;

            List<PayrollPeriodDetailV3Dto> details = new();
            DateTime currentDate = request.FromDate;

            for (int i = 1; i <= request.NoOfPeriod; i++)
            {
                DateTime startDate = currentDate.Date;
                DateTime endDate;
                DateTime nextStartDate;

                switch (request.AtnTypeId)
                {
                    case daily:
                        endDate = startDate;
                        nextStartDate = startDate.AddDays(1);
                        break;
                    case weekly:
                        endDate = startDate.AddDays(6);
                        nextStartDate = startDate.AddDays(7);
                        break;
                    case monthly:
                        endDate = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month));
                        nextStartDate = endDate.AddDays(1);
                        break;
                    case yearly:
                        endDate = startDate.AddYears(1).AddDays(-1);
                        nextStartDate = endDate.AddDays(1);
                        break;
                    default:
                        endDate = new DateTime(startDate.Year, startDate.Month, DateTime.DaysInMonth(startDate.Year, startDate.Month));
                        nextStartDate = endDate.AddDays(1);
                        break;
                }

                details.Add(new PayrollPeriodDetailV3Dto(
                    0, // Temporary ID
                    null,
                    $"Period {i}",
                    startDate.Year,
                    startDate,
                    endDate,
                    (int)(endDate - startDate).TotalDays + 1,
                    endDate.AddDays(1),
                    inactiveStatus,
                    i
                ));

                currentDate = nextStartDate;
            }

            return await Task.FromResult(details);
        }

        public async Task<NextPayrollRegisterDateV3Dto?> Handle(GetNextPayrollRegisterDateV3Query request, CancellationToken cancellationToken)
        {
            var lastEndDate = await (
                from detail in _context.PayrollPeriodDetails
                join header in _context.PayrollPeriods on detail.Payperiodid equals header.Id
                where header.Payrollregisterid == request.PayrollRegisterId
                    && !detail.utblock
                    && !header.Utblock
                select detail.Enddate
            ).MaxAsync(cancellationToken);

            return lastEndDate.HasValue
                ? new NextPayrollRegisterDateV3Dto(lastEndDate.Value.AddDays(1))
                : null;
        }
    }
}
