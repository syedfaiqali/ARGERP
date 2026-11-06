using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using HumanResource.Service.Entities;

using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HumanResource.Service.Handlers.Queries
{
    public class CalendarHolidayQueryHandlers :
        IRequestHandler<GetCalendarHolidaysV3Query, PaginationResponse<CalendarHolidayV3Dto>>,
        IRequestHandler<GetCalendarHolidayByIdV3Query, CalendarHolidayV3Dto?>
    {
        private readonly HumanResourceDbContext _context;

        public CalendarHolidayQueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<CalendarHolidayV3Dto>> Handle(GetCalendarHolidaysV3Query request, CancellationToken cancellationToken)
        {
            IQueryable<ETCalholidayhdrLegacy> baseQuery = _context.CalendarHolidays
                .Where(x => x.ClientId == request.ClientId && !x.Utblock)
                .AsNoTracking();

            if (request.EntityId.HasValue)
            {
                baseQuery = baseQuery.Where(x => x.EntityId == request.EntityId.Value || x.EntityId == null);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.ToLower();
                baseQuery = baseQuery.Where(x => x.Stxt.ToLower().Contains(search) || (x.Code != null && x.Code.ToLower().Contains(search)));
            }

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            // 1. Fetch paginated basic header data (No Joins)
            var headerItems = await baseQuery
                .OrderByDescending(x => x.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(h => new
                {
                    h.Id,
                    h.Code,
                    h.Stxt,
                    h.Payperiodid,
                    h.Applyonid,
                    h.Lovsharedid,
                    h.Utblock
                })
                .ToListAsync(cancellationToken);

            var headerIds = headerItems.Select(x => x.Id).ToList();

            // 2. Fetch all related labels in bulk
            var ppIds = headerItems.Where(x => x.Payperiodid.HasValue).Select(x => x.Payperiodid!.Value).Distinct().ToList();
            var payperiods = await _context.PayrollPeriods.Where(x => ppIds.Contains(x.Id)).Select(p => new { p.Id, p.Ltxt }).ToListAsync(cancellationToken);

            var lovIds = headerItems.Where(x => x.Applyonid.HasValue).Select(x => x.Applyonid!.Value)
                .Concat(headerItems.Where(x => x.Lovsharedid.HasValue).Select(x => x.Lovsharedid!.Value))
                .Distinct().ToList();
            var lovs = await _context.Lovs.Where(x => lovIds.Contains(x.Id)).Select(l => new { l.Id, l.Stxt }).ToListAsync(cancellationToken);

            // 3. Fetch all details for these headers in bulk
            var detailsRaw = await _context.CalendarHolidayDetails
                .AsNoTracking()
                .Where(x => x.Calholidayid.HasValue && headerIds.Contains(x.Calholidayid.Value))
                .ToListAsync(cancellationToken);

            var detailPpIds = detailsRaw.Where(d => d.Payperioddtlid.HasValue).Select(d => d.Payperioddtlid!.Value).Distinct().ToList();
            var detailPpDtls = await _context.PayrollPeriodDetails
                .AsNoTracking()
                .Where(pd => detailPpIds.Contains(pd.Id))
                .Select(pd => new { pd.Id, pd.Year, pd.Startdate, pd.Periodstxt })
                .ToListAsync(cancellationToken);

            var applyOnDetails = await _context.CalendarHolidayApplyOnDetails
                .AsNoTracking()
                .Where(x => x.Calholidayid.HasValue && headerIds.Contains(x.Calholidayid.Value))
                .ToListAsync(cancellationToken);

            var sharedDetails = await _context.CalendarHolidaySharedDetails
                .AsNoTracking()
                .Where(x => x.Calholidayid.HasValue && headerIds.Contains(x.Calholidayid.Value))
                .ToListAsync(cancellationToken);

            // 4. Merge in memory
            var items = headerItems.Select(h => {
                var pName = payperiods.FirstOrDefault(p => p.Id == h.Payperiodid)?.Ltxt ?? string.Empty;
                return new CalendarHolidayV3Dto(
                    h.Id,
                    h.Code ?? string.Empty,
                    h.Stxt ?? string.Empty,
                    h.Payperiodid,
                    h.Applyonid,
                    h.Lovsharedid,
                    pName,
                    lovs.FirstOrDefault(l => l.Id == h.Applyonid)?.Stxt ?? string.Empty,
                    lovs.FirstOrDefault(l => l.Id == h.Lovsharedid)?.Stxt ?? string.Empty,
                    !h.Utblock,
                    detailsRaw.Where(d => d.Calholidayid == h.Id).Select(d => {
                        var pd = d.Payperioddtlid.HasValue ? detailPpDtls.FirstOrDefault(p => p.Id == d.Payperioddtlid.Value) : null;
                        return new CalendarHolidayDetailV3Dto(
                            d.Id,
                            d.Holidaytypeid,
                            d.Ltxt ?? string.Empty,
                            d.Startdate,
                            d.Enddate,
                            d.Days,
                            d.Payperioddtlid,
                            pd?.Year,
                            pd?.Startdate.HasValue == true ? pd.Startdate.Value.ToString("MMMM") : (pd?.Periodstxt ?? string.Empty)
                        );
                    }).ToList(),
                    applyOnDetails.Where(a => a.Calholidayid == h.Id).Select(a => new CalendarHolidayApplyOnDetailV3Dto(
                        a.Id,
                        a.Departid,
                        a.Emptypeid
                    )).ToList(),
                    sharedDetails.Where(s => s.Calholidayid == h.Id).Select(s => new CalendarHolidaySharedDetailV3Dto(
                        s.Id,
                        s.Lovsharedid,
                        s.Pclientid,
                        s.Pentityid,
                        s.Regionid
                    )).ToList()
                );
            }).ToList();

            return new PaginationResponse<CalendarHolidayV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<CalendarHolidayV3Dto?> Handle(GetCalendarHolidayByIdV3Query request, CancellationToken cancellationToken)
        {
            // 1. Fetch header (No Joins)
            var h = await _context.CalendarHolidays
                .AsNoTracking()
                .Where(x => x.Id == request.Id && !x.Utblock && x.ClientId == request.ClientId
                    && (!request.EntityId.HasValue || x.EntityId == request.EntityId.Value || x.EntityId == null))
                .FirstOrDefaultAsync(cancellationToken);

            if (h == null) return null;

            // 2. Fetch labels in bulk
            string payrollName = "";
            if (h.Payperiodid.HasValue)
                payrollName = await _context.PayrollPeriods.Where(p => p.Id == h.Payperiodid).Select(p => p.Ltxt).FirstOrDefaultAsync(cancellationToken) ?? "";

            string applyOnName = "";
            if (h.Applyonid.HasValue)
                applyOnName = await _context.Lovs.Where(l => l.Id == h.Applyonid).Select(l => l.Stxt).FirstOrDefaultAsync(cancellationToken) ?? "";

            string sharedWithName = "";
            if (h.Lovsharedid.HasValue)
                sharedWithName = await _context.Lovs.Where(l => l.Id == h.Lovsharedid).Select(l => l.Stxt).FirstOrDefaultAsync(cancellationToken) ?? "";

            // 3. Fetch details and related info separately
            var detailsRaw = await _context.CalendarHolidayDetails
                .AsNoTracking()
                .Where(x => x.Calholidayid == h.Id)
                .ToListAsync(cancellationToken);

            var detailPpIds = detailsRaw.Where(d => d.Payperioddtlid.HasValue).Select(d => d.Payperioddtlid!.Value).Distinct().ToList();
            var detailPpDtls = await _context.PayrollPeriodDetails
                .AsNoTracking()
                .Where(pd => detailPpIds.Contains(pd.Id))
                .Select(pd => new { pd.Id, pd.Year, pd.Startdate, pd.Periodstxt })
                .ToListAsync(cancellationToken);

            var applyOnDetails = await _context.CalendarHolidayApplyOnDetails
                .AsNoTracking()
                .Where(x => x.Calholidayid == h.Id)
                .ToListAsync(cancellationToken);

            var sharedDetails = await _context.CalendarHolidaySharedDetails
                .AsNoTracking()
                .Where(x => x.Calholidayid == h.Id)
                .ToListAsync(cancellationToken);

            // 4. Merge
            return new CalendarHolidayV3Dto(
                h.Id,
                h.Code ?? string.Empty,
                h.Stxt ?? string.Empty,
                h.Payperiodid,
                h.Applyonid,
                h.Lovsharedid,
                payrollName,
                applyOnName,
                sharedWithName,
                !h.Utblock,
                detailsRaw.Select(d => {
                    var pd = d.Payperioddtlid.HasValue ? detailPpDtls.FirstOrDefault(p => p.Id == d.Payperioddtlid.Value) : null;
                    return new CalendarHolidayDetailV3Dto(
                        d.Id,
                        d.Holidaytypeid,
                        d.Ltxt ?? string.Empty,
                        d.Startdate,
                        d.Enddate,
                        d.Days,
                        d.Payperioddtlid,
                        pd?.Year,
                        pd?.Startdate.HasValue == true ? pd.Startdate.Value.ToString("MMMM") : (pd?.Periodstxt ?? string.Empty)
                    );
                }).ToList(),
                applyOnDetails.Select(a => new CalendarHolidayApplyOnDetailV3Dto(
                    a.Id,
                    a.Departid,
                    a.Emptypeid
                )).ToList(),
                sharedDetails.Select(s => new CalendarHolidaySharedDetailV3Dto(
                    s.Id,
                    s.Lovsharedid,
                    s.Pclientid,
                    s.Pentityid,
                    s.Regionid
                )).ToList()
            );
        }
    }
}
