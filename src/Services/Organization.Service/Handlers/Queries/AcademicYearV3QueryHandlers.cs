using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.DTOs;
using Organization.Service.Handlers.Commands;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Queries;
using System.Globalization;

namespace Organization.Service.Handlers.Queries
{
    public sealed class GetAcademicYearsV3QueryHandler : IRequestHandler<GetAcademicYearsV3Query, PaginationResponse<AcademicYearV3Dto>>
    {
        private readonly OrganizationDbContext _context;

        public GetAcademicYearsV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<AcademicYearV3Dto>> Handle(GetAcademicYearsV3Query request, CancellationToken cancellationToken)
        {
            var isMultiYearProgramme = await AcademicYearActiveRules.IsUniversityAsync(
                _context, request.ClientId, cancellationToken);
            var today = DateTime.Today;
            var query = _context.AcademicYearLegacy
                .Include(x => x.SMAcademicregister)
                .Include(x => x.SMAcademicYeardtls)
                .AsNoTracking()
                .Where(x => !x.utblock && x.clientid == request.ClientId)
                .AsQueryable();

            // Years have no entity column; the institution's selected register
            // supplies their scope. Apply it before searching and pagination.
            var registerId = await _context.OrgLegalentityLegacy.AsNoTracking()
                .Where(x => x.Id == request.EntityId && x.clientid == request.ClientId && !x.utblock)
                .Select(x => x.registerid).FirstOrDefaultAsync(cancellationToken);
            query = query.Where(x => registerId > 0 && x.registerid == registerId &&
                _context.AcademicRegisterLegacy.Any(register => register.Id == registerId &&
                    register.clientid == request.ClientId && !register.utblock));

            if (request.ActiveOnly)
                query = isMultiYearProgramme
                    ? query.Where(x => x.startdate != null && x.enddate != null && x.startdate <= today && x.enddate >= today)
                    : query.Where(x => x.active);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.Trim().ToLower();
                query = query.Where(x => 
                    (x.code != null && x.code.ToLower().Contains(search)) ||
                    (x.stxt != null && x.stxt.ToLower().Contains(search)) ||
                    (x.SMAcademicregister != null && x.SMAcademicregister.stxt != null && x.SMAcademicregister.stxt.ToLower().Contains(search))
                );
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(x => x.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new AcademicYearV3Dto
                {
                    Id = x.Id,
                    Code = x.code,
                    AcademicYear = x.stxt,
                    AcademicRegisterId = x.registerid,
                    AcademicRegisterName = x.SMAcademicregister != null ? x.SMAcademicregister.stxt : null,
                    SemLovId = x.semlovid,
                    CourseCatId = x.coursecatid,
                    SemLovName = _context.Lovs.Where(l => l.Id == x.semlovid).Select(l => l.stxt).FirstOrDefault(),
                    StartDate = x.startdate,
                    EndDate = x.enddate,
                    Description = x.ltxt,
                    SessionNotVisible = x.sessionnotvisiable,
                    CurrentYear = isMultiYearProgramme
                        ? x.startdate != null && x.enddate != null && x.startdate <= today && x.enddate >= today
                        : x.active,
                    Periods = x.SMAcademicYeardtls
                        .OrderBy(d => d.lineindex)
                        .Select(d => new AcademicYearPeriodV3Dto
                        {
                            Id = d.Id,
                            LineIndex = d.lineindex,
                            Year = d.year,
                            MonthId = d.monthid,
                            PeriodName = d.periodstxt,
                            StartDate = d.startdate,
                            EndDate = d.enddate
                        }).ToList()
                })
                .ToListAsync(cancellationToken);

            return new PaginationResponse<AcademicYearV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }

    public sealed class GetAcademicYearByIdV3QueryHandler : IRequestHandler<GetAcademicYearByIdV3Query, AcademicYearV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public GetAcademicYearByIdV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<AcademicYearV3Dto> Handle(GetAcademicYearByIdV3Query request, CancellationToken cancellationToken)
        {
            var isMultiYearProgramme = await AcademicYearActiveRules.IsUniversityAsync(
                _context, request.ClientId, cancellationToken);
            var today = DateTime.Today;
            var result = await _context.AcademicYearLegacy
                .Include(x => x.SMAcademicregister)
                .Include(x => x.SMAcademicYeardtls)
                .AsNoTracking()
                .Where(x => !x.utblock && x.Id == request.Id && x.clientid == request.ClientId)
                .Select(x => new AcademicYearV3Dto
                {
                    Id = x.Id,
                    Code = x.code,
                    AcademicYear = x.stxt,
                    AcademicRegisterId = x.registerid,
                    AcademicRegisterName = x.SMAcademicregister != null ? x.SMAcademicregister.stxt : null,
                    SemLovId = x.semlovid,
                    CourseCatId = x.coursecatid,
                    SemLovName = _context.Lovs.Where(l => l.Id == x.semlovid).Select(l => l.stxt).FirstOrDefault(),
                    StartDate = x.startdate,
                    EndDate = x.enddate,
                    Description = x.ltxt,
                    SessionNotVisible = x.sessionnotvisiable,
                    CurrentYear = isMultiYearProgramme
                        ? x.startdate != null && x.enddate != null && x.startdate <= today && x.enddate >= today
                        : x.active,
                    Periods = x.SMAcademicYeardtls.OrderBy(d => d.lineindex).Select(d => new AcademicYearPeriodV3Dto
                    {
                        Id = d.Id,
                        LineIndex = d.lineindex,
                        Year = d.year,
                        MonthId = d.monthid,
                        PeriodName = d.periodstxt,
                        StartDate = d.startdate,
                        EndDate = d.enddate
                    }).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (result == null)
                throw new KeyNotFoundException($"Academic Year with ID {request.Id} not found");

            return result;
        }
    }

    public sealed class GetAcademicYearSemesterLovsV3QueryHandler : IRequestHandler<GetAcademicYearSemesterLovsV3Query, List<LookupItemDto>>
    {
        private readonly OrganizationDbContext _context;

        public GetAcademicYearSemesterLovsV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public Task<List<LookupItemDto>> Handle(GetAcademicYearSemesterLovsV3Query request, CancellationToken cancellationToken)
        {
            return _context.Lovs
                .AsNoTracking()
                .Where(x => !x.utblock && x.vtype == "SEM")
                .OrderBy(x => x.indx ?? int.MaxValue)
                .ThenBy(x => x.stxt)
                .Select(x => new LookupItemDto
                {
                    Id = x.Id,
                    Code = x.code,
                    Name = x.stxt,
                    Description = x.ltxt
                })
                .ToListAsync(cancellationToken);
        }
    }

    public sealed class GetAcademicRegisterStartDateV3QueryHandler(OrganizationDbContext context)
        : IRequestHandler<GetAcademicRegisterStartDateV3Query, DateTime?>
    {
        public async Task<DateTime?> Handle(GetAcademicRegisterStartDateV3Query request, CancellationToken ct)
        {
            if (!await context.AcademicRegisterLegacy.AnyAsync(x => x.Id == request.AcademicRegisterId && x.clientid == request.ClientId && !x.utblock, ct))
                throw new InvalidOperationException("Academic Register is invalid");
            var years = context.AcademicYearLegacy.AsNoTracking().Where(x =>
                x.clientid == request.ClientId && x.registerid == request.AcademicRegisterId && !x.utblock && x.enddate != null);
            if (request.AcademicYearId is > 0)
            {
                var current = await context.AcademicYearLegacy.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.AcademicYearId && x.clientid == request.ClientId && !x.utblock, ct);
                if (current == null) throw new InvalidOperationException("Academic Year is invalid");
                years = years.Where(x => x.Id != request.AcademicYearId);
                // Editing an older year must not advance its start past later years.
                if (current.registerid == request.AcademicRegisterId && current.startdate.HasValue)
                    years = years.Where(x => x.enddate < current.startdate);
            }
            var latest = await years.MaxAsync(x => x.enddate, ct);
            return latest?.Date.AddDays(1);
        }
    }

    public sealed class GetGeneratedPeriodsV3QueryHandler : IRequestHandler<GetGeneratedPeriodsV3Query, List<AcademicYearPeriodV3Dto>>
    {
        private readonly OrganizationDbContext _context;
        public GetGeneratedPeriodsV3QueryHandler(OrganizationDbContext context) => _context = context;

        public async Task<List<AcademicYearPeriodV3Dto>> Handle(GetGeneratedPeriodsV3Query request, CancellationToken cancellationToken)
        {
            if (request.AcademicRegisterId is not > 0)
                throw new InvalidOperationException("Academic Register is required to generate periods");
            if (!await _context.AcademicRegisterLegacy.AnyAsync(x => x.Id == request.AcademicRegisterId && x.clientid == request.ClientId && !x.utblock, cancellationToken))
                throw new InvalidOperationException("Academic Register is invalid");
            var startDate = request.StartDate.Date;
            var endDate = request.EndDate.Date;

            if (startDate > endDate)
                throw new InvalidOperationException("Start Date cannot be greater than End Date");

            var periods = new List<AcademicYearPeriodV3Dto>();
            var current = startDate;
            int lineIndex = 1;

            while (current <= endDate)
            {
                // LegacyPak uses months anchored to the selected start day.
                // Clip the final (including same-day) interval to the supplied end.
                var periodEnd = current.AddMonths(1).AddDays(-1);
                if (periodEnd > endDate)
                    periodEnd = endDate;

                periods.Add(new AcademicYearPeriodV3Dto
                {
                    LineIndex = lineIndex++,
                    StartDate = current,
                    EndDate = periodEnd,
                    MonthId = current.Month,
                    Year = current.Year,
                    PeriodName = current.ToString("MMMM", CultureInfo.InvariantCulture)
                });

                current = periodEnd.AddDays(1);
            }

            return periods;
        }
    }
}
