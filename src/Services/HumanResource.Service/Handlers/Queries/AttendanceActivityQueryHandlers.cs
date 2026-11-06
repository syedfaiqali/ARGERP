using MediatR;
using Microsoft.EntityFrameworkCore;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class AttendanceActivityQueryHandlers :
        IRequestHandler<GetAttendanceActivitiesV3Query, PaginationResponse<AttendanceActivityV3Dto>>,
        IRequestHandler<GetAttendanceActivityByIdV3Query, AttendanceActivityV3Dto?>
    {
        private readonly HumanResourceDbContext _context;

        public AttendanceActivityQueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<AttendanceActivityV3Dto>> Handle(GetAttendanceActivitiesV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.AttendanceActivities
                .Where(x => x.ClientId == request.ClientId && !x.UtBlock)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(x =>
                    (x.Stxt != null && x.Stxt.Contains(request.SearchTerm)) ||
                    (x.OrderId.ToString().Contains(request.SearchTerm)));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(x => x.OrderId)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new AttendanceActivityV3Dto(
                    x.Id,
                    x.OrderId,
                    x.Stxt,
                    x.Value
                ))
                .ToListAsync(cancellationToken);

            return new PaginationResponse<AttendanceActivityV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<AttendanceActivityV3Dto?> Handle(GetAttendanceActivityByIdV3Query request, CancellationToken cancellationToken)
        {
            return await _context.AttendanceActivities
                .Where(x => x.Id == request.Id && x.ClientId == request.ClientId && !x.UtBlock)
                .Select(x => new AttendanceActivityV3Dto(
                    x.Id,
                    x.OrderId,
                    x.Stxt,
                    x.Value
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}