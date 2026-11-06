using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.DTOs;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Queries;

namespace Organization.Service.Handlers.Queries
{
    public sealed class CallTypeV3QueryHandlers :
        IRequestHandler<GetCallTypesV3Query, PaginationResponse<CallTypeV3Dto>>,
        IRequestHandler<GetCallTypeByIdV3Query, CallTypeV3Dto?>
    {
        private readonly OrganizationDbContext _context;

        public CallTypeV3QueryHandlers(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<CallTypeV3Dto>> Handle(GetCallTypesV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.Lovs
                .Where(x => x.vtype == "CALLT" && !x.utblock)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(x =>
                    (x.code != null && x.code.Contains(request.SearchTerm)) ||
                    (x.stxt != null && x.stxt.Contains(request.SearchTerm)));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(x => x.stxt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new CallTypeV3Dto
                {
                    Id = x.Id,
                    Code = x.code,
                    ShortName = x.stxt,
                    Description = x.ltxt
                })
                .ToListAsync(cancellationToken);

            return new PaginationResponse<CallTypeV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<CallTypeV3Dto?> Handle(GetCallTypeByIdV3Query request, CancellationToken cancellationToken)
        {
            return await _context.Lovs
                .Where(x => x.vtype == "CALLT" && !x.utblock && x.Id == request.Id)
                .Select(x => new CallTypeV3Dto
                {
                    Id = x.Id,
                    Code = x.code,
                    ShortName = x.stxt,
                    Description = x.ltxt
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
