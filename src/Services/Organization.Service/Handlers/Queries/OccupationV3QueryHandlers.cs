using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.DTOs;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Queries;

namespace Organization.Service.Handlers.Queries
{
    public sealed class OccupationV3QueryHandlers :
        IRequestHandler<GetOccupationsV3Query, PaginationResponse<OccupationV3Dto>>,
        IRequestHandler<GetOccupationByIdV3Query, OccupationV3Dto?>
    {
        private readonly OrganizationDbContext _context;

        public OccupationV3QueryHandlers(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<OccupationV3Dto>> Handle(GetOccupationsV3Query request, CancellationToken cancellationToken)
        {
            // Occupation uses legacy columns: Id/code/stxt/ltxt.
            var query = _context.OccupationLegacy
                .Where(x => x.clientid == request.ClientId && !x.utblock)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(x =>
                    (x.code != null && x.code.Contains(request.SearchTerm)) ||
                    (x.stxt != null && x.stxt.Contains(request.SearchTerm)));
            }

            var totalItems = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(x => x.stxt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new OccupationV3Dto
                {
                    Id = x.Id,
                    Code = x.code,
                    OccupationName = x.stxt,
                    Description = x.ltxt
                })
                .ToListAsync(cancellationToken);

            return new PaginationResponse<OccupationV3Dto>
            {
                Items = items,
                TotalCount = totalItems,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<OccupationV3Dto?> Handle(GetOccupationByIdV3Query request, CancellationToken cancellationToken)
        {
            return await _context.OccupationLegacy
                .Where(x => x.clientid == request.ClientId && x.Id == request.Id && !x.utblock)
                .Select(x => new OccupationV3Dto
                {
                    Id = x.Id,
                    Code = x.code,
                    OccupationName = x.stxt,
                    Description = x.ltxt
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
