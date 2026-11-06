using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class ScheduleQueryHandlers :
        IRequestHandler<GetSchedulesV3Query, PaginationResponse<ScheduleV3Dto>>,
        IRequestHandler<GetScheduleByIdV3Query, ScheduleHeaderV3Dto?>,
        IRequestHandler<GetScheduleDetailsV3Query, IReadOnlyList<ScheduleDetailV3Dto>>
    {
        private readonly HumanResourceDbContext _context;

        public ScheduleQueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<ScheduleV3Dto>> Handle(GetSchedulesV3Query request, CancellationToken cancellationToken)
        {
            var effectiveEntityId = await ResolveEntityIdAsync(request.EntityId, cancellationToken);

            var baseQuery = _context.Schedules.AsNoTracking()
                .Where(s => s.ClientId == request.ClientId && !(s.UtBlock ?? false));

            if (effectiveEntityId.HasValue)
            {
                baseQuery = baseQuery.Where(s => s.EntityId == effectiveEntityId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                baseQuery = baseQuery.Where(s => (s.Stxt ?? "").Contains(term) || (s.Code ?? "").Contains(term));
            }

            var total = await baseQuery.CountAsync(cancellationToken);

            var items = await baseQuery
                .OrderByDescending(s => s.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new
                {
                    s,
                    ScheduleForName = _context.Lovs.Where(l => l.Id == s.ScheduleFor).Select(l => l.Stxt).FirstOrDefault(),
                    ShiftName = _context.Shifts.Where(x => x.Id == s.ShiftId).Select(x => x.Stxt).FirstOrDefault()
                })
                .ToListAsync(cancellationToken);

            var data = items.Select(x => new ScheduleV3Dto(
                x.s.Id,
                x.s.Code ?? string.Empty,
                x.s.Stxt ?? string.Empty,
                x.s.FromDate,
                x.s.TillDate,
                x.s.ScheduleFor,
                x.ScheduleForName,
                x.s.ShiftId,
                x.ShiftName,
                x.s.AssignTo
            )).ToList();

            return new PaginationResponse<ScheduleV3Dto>
            {
                Items = data,
                TotalCount = total,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<ScheduleHeaderV3Dto?> Handle(GetScheduleByIdV3Query request, CancellationToken cancellationToken)
        {
            var effectiveEntityId = await ResolveEntityIdAsync(request.EntityId, cancellationToken);

            var header = await _context.Schedules.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == request.Id
                    && s.ClientId == request.ClientId
                    && !(s.UtBlock ?? false)
                    && (!effectiveEntityId.HasValue || s.EntityId == effectiveEntityId.Value),
                    cancellationToken);

            if (header == null) return null;

            var scheduleForName = await _context.Lovs.AsNoTracking()
                .Where(l => l.Id == header.ScheduleFor)
                .Select(l => l.Stxt)
                .FirstOrDefaultAsync(cancellationToken);

            var shiftName = await _context.Shifts.AsNoTracking()
                .Where(s => s.Id == header.ShiftId)
                .Select(s => s.Stxt)
                .FirstOrDefaultAsync(cancellationToken);

            var details = await _context.ScheduleDetails.AsNoTracking()
                .Where(d => d.ScheduleId == header.Id && !(d.UtBlock ?? false))
                .OrderBy(d => d.LineIndex)
                .ToListAsync(cancellationToken);

            var detailDtos = details.Select((d, index) => new ScheduleDetailV3Dto(
                d.Id,
                d.LineIndex ?? index + 1,
                d.SchDate,
                d.Holiday ?? false,
                d.ShiftId,
                d.TimeIn,
                d.TimeOut,
                d.Duration,
                d.SchDate.HasValue ? d.SchDate.Value.ToString("dddd") : null
            )).ToList();

            return new ScheduleHeaderV3Dto(
                header.Id,
                header.Code ?? string.Empty,
                header.Stxt ?? string.Empty,
                header.FromDate,
                header.TillDate,
                header.ScheduleFor,
                scheduleForName,
                header.ShiftId,
                shiftName,
                header.AssignTo,
                detailDtos
            );
        }

        public Task<IReadOnlyList<ScheduleDetailV3Dto>> Handle(GetScheduleDetailsV3Query request, CancellationToken cancellationToken)
        {
            if (request.TillDate.Date < request.FromDate.Date)
            {
                return Task.FromResult<IReadOnlyList<ScheduleDetailV3Dto>>(new List<ScheduleDetailV3Dto>());
            }

            var days = (request.TillDate.Date - request.FromDate.Date).Days + 1;
            var list = new List<ScheduleDetailV3Dto>();
            var current = request.FromDate.Date;
            for (var i = 0; i < days; i++)
            {
                var day = current.AddDays(i);
                list.Add(new ScheduleDetailV3Dto(
                    null,
                    i + 1,
                    day,
                    day.DayOfWeek == DayOfWeek.Sunday,
                    null,
                    null,
                    null,
                    null,
                    day.ToString("dddd")
                ));
            }

            return Task.FromResult<IReadOnlyList<ScheduleDetailV3Dto>>(list);
        }

        private async Task<int?> ResolveEntityIdAsync(int? entityId, CancellationToken cancellationToken)
        {
            if (!entityId.HasValue) return null;
            var exists = await _context.OrgLegalentities
                .AsNoTracking()
                .AnyAsync(e => e.Id == entityId.Value && !e.UtBlock, cancellationToken);
            return exists ? entityId : null;
        }
    }
}
