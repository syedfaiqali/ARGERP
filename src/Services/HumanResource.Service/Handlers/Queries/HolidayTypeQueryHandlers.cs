using MediatR;
using Microsoft.EntityFrameworkCore;
using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class HolidayTypeQueryHandlers :
        IRequestHandler<GetHolidayTypesV3Query, PaginationResponse<HolidayTypeV3Dto>>,
        IRequestHandler<GetHolidayTypeByIdV3Query, HolidayTypeV3Dto?>
    {
        private readonly HumanResourceDbContext _context;

        public HolidayTypeQueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<HolidayTypeV3Dto>> Handle(GetHolidayTypesV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.HolidayTypes
                .Where(x => x.ClientId == request.ClientId && x.EntityId != null && !(x.UtBlock ?? false))
                .AsNoTracking();

            if (request.EntityId.HasValue)
            {
                query = query.Where(x => x.EntityId == request.EntityId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(x => (x.Stxt != null && x.Stxt.Contains(request.SearchTerm)) || (x.Code != null && x.Code.Contains(request.SearchTerm)));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(x => x.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new HolidayTypeV3Dto(
                    x.Id,
                    x.Code ?? string.Empty,
                    x.Stxt ?? string.Empty,
                    x.Ltxt,
                    x.Colorcode,
                    !(x.UtBlock ?? false)
                ))
                .ToListAsync(cancellationToken);

            return new PaginationResponse<HolidayTypeV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<HolidayTypeV3Dto?> Handle(GetHolidayTypeByIdV3Query request, CancellationToken cancellationToken)
        {
            return await _context.HolidayTypes
                .Where(x => x.Id == request.Id && x.ClientId == request.ClientId && !(x.UtBlock ?? false) && (!request.EntityId.HasValue || x.EntityId == request.EntityId.Value))
                .Select(x => new HolidayTypeV3Dto(
                    x.Id,
                    x.Code ?? string.Empty,
                    x.Stxt ?? string.Empty,
                    x.Ltxt,
                    x.Colorcode,
                    !(x.UtBlock ?? false)
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
