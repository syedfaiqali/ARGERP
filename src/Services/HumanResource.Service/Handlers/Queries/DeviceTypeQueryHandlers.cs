using MediatR;
using Microsoft.EntityFrameworkCore;
using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class DeviceTypeQueryHandlers :
        IRequestHandler<GetDeviceTypesV3Query, PaginationResponse<DeviceTypeV3Dto>>,
        IRequestHandler<GetDeviceTypeByIdV3Query, DeviceTypeV3Dto?>
    {
        private readonly HumanResourceDbContext _context;

        public DeviceTypeQueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<DeviceTypeV3Dto>> Handle(GetDeviceTypesV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.DeviceTypes
                .Where(x => x.ClientId == request.ClientId && !x.UtBlock)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(x =>
                    (x.Stxt != null && x.Stxt.Contains(request.SearchTerm)) ||
                    (x.Code != null && x.Code.Contains(request.SearchTerm)));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(x => x.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new DeviceTypeV3Dto(
                    x.Id,
                    x.Code ?? string.Empty,
                    x.Stxt ?? string.Empty,
                    x.Ltxt,
                    !x.UtBlock
                ))
                .ToListAsync(cancellationToken);

            return new PaginationResponse<DeviceTypeV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<DeviceTypeV3Dto?> Handle(GetDeviceTypeByIdV3Query request, CancellationToken cancellationToken)
        {
            return await _context.DeviceTypes
                .Where(x => x.Id == request.Id && x.ClientId == request.ClientId && !x.UtBlock)
                .Select(x => new DeviceTypeV3Dto(
                    x.Id,
                    x.Code ?? string.Empty,
                    x.Stxt ?? string.Empty,
                    x.Ltxt,
                    !x.UtBlock
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
