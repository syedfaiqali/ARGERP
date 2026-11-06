using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.DTOs;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Queries;

namespace Organization.Service.Handlers.Queries
{
    public sealed class GetRegionsV3QueryHandler : IRequestHandler<GetRegionsV3Query, PaginationResponse<RegionV3Dto>>
    {
        private readonly OrganizationDbContext _context;

        public GetRegionsV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<RegionV3Dto>> Handle(GetRegionsV3Query request, CancellationToken cancellationToken)
        {
            var clientId = EnsureClientId(request.ClientId);

            var query = _context.OrgRegion
                .AsNoTracking()
                .Include(r => r.Country)
                .Where(r => r.ClientId == clientId && !r.UtBlock)
                .AsQueryable();

            //if (!request.IncludeInactive)
            //    query = query.Where(r => r.IsActive);

            if (request.CountryId.HasValue)
                query = query.Where(r => r.CountryId == request.CountryId.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.Trim().ToLower();
                query = query.Where(r =>
                    r.stxt.ToLower().Contains(search)
                    || r.Code.ToLower().Contains(search)
                    || (r.Country != null && r.Country.CountryName.ToLower().Contains(search)));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(r => r.stxt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(r => new RegionV3Dto
                {
                    RegionId = r.Id,
                    RegionCode = r.Code,
                    RegionName = r.stxt,
                    CountryId = r.CountryId ?? 0,
                    CountryName = r.Country != null ? r.Country.CountryName : string.Empty,
                    //IsActive = r.IsActive,
                })
                .ToListAsync(cancellationToken);

            return new PaginationResponse<RegionV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            };
        }

        private static int EnsureClientId(int? clientId)
        {
            if (!clientId.HasValue || clientId.Value <= 0)
                throw new InvalidOperationException("X-Client-Id header is required");

            return clientId.Value;
        }
    }

    public sealed class GetRegionByIdV3QueryHandler : IRequestHandler<GetRegionByIdV3Query, RegionV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public GetRegionByIdV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<RegionV3Dto> Handle(GetRegionByIdV3Query request, CancellationToken cancellationToken)
        {
            var clientId = EnsureClientId(request.ClientId);

            var region = await _context.OrgRegion
                .AsNoTracking()
                .Include(r => r.Country)
                .FirstOrDefaultAsync(r =>
                    r.Id == request.RegionId
                    && r.ClientId == clientId
                    && !r.UtBlock
                    , cancellationToken);

            if (region == null)
                throw new KeyNotFoundException($"Region with ID {request.RegionId} not found");

            return new RegionV3Dto
            {
                RegionId = region.Id,
                RegionCode = region.Code,
                RegionName = region.stxt,
                CountryId = region.CountryId ?? 0,
                CountryName = region.Country != null ? region.Country.CountryName : string.Empty,
                //IsActive = region.IsActive,
            };
        }

        private static int EnsureClientId(int? clientId)
        {
            if (!clientId.HasValue || clientId.Value <= 0)
                throw new InvalidOperationException("X-Client-Id header is required");

            return clientId.Value;
        }
    }
}
