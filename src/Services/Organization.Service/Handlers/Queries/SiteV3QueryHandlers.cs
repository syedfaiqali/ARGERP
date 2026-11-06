using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.DTOs;
using Organization.Service.Handlers;
using Organization.Service.Handlers.Commands;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Queries;

namespace Organization.Service.Handlers.Queries
{
    public sealed class GetSitesV3QueryHandler : IRequestHandler<GetSitesV3Query, PaginationResponse<SiteV3Dto>>
    {
        private readonly OrganizationDbContext _context;

        public GetSitesV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<SiteV3Dto>> Handle(GetSitesV3Query request, CancellationToken cancellationToken)
        {
            var clientId = SiteV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = SiteV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");

            var query = _context.OrgSites
                .AsNoTracking()
                .Where(s => s.ClientId == clientId && s.EntityId == entityId)
                .AsQueryable();

            if (request.RegionId.HasValue && request.RegionId.Value > 0)
                query = query.Where(s => s.RegionId == request.RegionId.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.Trim().ToLower();
                query = query.Where(s =>
                    (s.Code != null && s.Code.ToLower().Contains(search))
                    || (s.SiteName != null && s.SiteName.ToLower().Contains(search)));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var ids = await query
                .OrderBy(s => s.SiteName)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => s.Id)
                .ToListAsync(cancellationToken);

            var items = new List<SiteV3Dto>(ids.Count);
            foreach (var siteId in ids)
            {
                items.Add(await SiteV3Mapper.MapByIdAsync(_context, siteId, clientId, entityId, cancellationToken));
            }

            return new PaginationResponse<SiteV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            };
        }
    }

    public sealed class GetSiteByIdV3QueryHandler : IRequestHandler<GetSiteByIdV3Query, SiteV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public GetSiteByIdV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public Task<SiteV3Dto> Handle(GetSiteByIdV3Query request, CancellationToken cancellationToken)
        {
            var clientId = SiteV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = SiteV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");
            var siteId = SiteV3AccessHelpers.EnsurePositive(request.SiteId, "SiteId");

            return SiteV3Mapper.MapByIdAsync(_context, siteId, clientId, entityId, cancellationToken);
        }
    }

    public sealed class GetSiteTypesV3QueryHandler : IRequestHandler<GetSiteTypesV3Query, List<SiteLookupDto>>
    {
        private readonly OrganizationDbContext _context;

        public GetSiteTypesV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SiteLookupDto>> Handle(GetSiteTypesV3Query request, CancellationToken cancellationToken)
        {
            return await _context.Lovs
                .AsNoTracking()
                .Where(l => l.vtype != null && l.vtype.ToUpper() == "STYP")
                .OrderBy(l => l.stxt)
                .Select(l => new SiteLookupDto
                {
                    Id = l.Id,
                    Code = l.code ?? string.Empty,
                    Name = l.stxt ?? l.ltxt ?? string.Empty,
                    ShortName = l.stxt ?? string.Empty,
                    Description = l.ltxt ?? l.stxt ?? string.Empty,
                })
                .ToListAsync(cancellationToken);
        }
    }

    public sealed class GetSiteForOptionsV3QueryHandler : IRequestHandler<GetSiteForOptionsV3Query, List<SiteLookupDto>>
    {
        private readonly OrganizationDbContext _context;

        public GetSiteForOptionsV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SiteLookupDto>> Handle(GetSiteForOptionsV3Query request, CancellationToken cancellationToken)
        {
            return await _context.Lovs
                .AsNoTracking()
                .Where(l => l.vtype != null && l.vtype.ToUpper() == "SFOR")
                .OrderBy(l => l.stxt)
                .Select(l => new SiteLookupDto
                {
                    Id = l.Id,
                    Code = l.code ?? string.Empty,
                    Name = l.stxt ?? l.ltxt ?? string.Empty,
                    ShortName = l.stxt ?? string.Empty,
                    Description = l.ltxt ?? l.stxt ?? string.Empty,
                })
                .ToListAsync(cancellationToken);
        }
    }

    public sealed class GetSiteParentOptionsV3QueryHandler : IRequestHandler<GetSiteParentOptionsV3Query, List<SiteLookupDto>>
    {
        private readonly OrganizationDbContext _context;

        public GetSiteParentOptionsV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<List<SiteLookupDto>> Handle(GetSiteParentOptionsV3Query request, CancellationToken cancellationToken)
        {
            var clientId = SiteV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = SiteV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");

            return await _context.OrgSites
                .AsNoTracking()
                .Where(s => s.ClientId == clientId && s.EntityId == entityId)
                .OrderBy(s => s.SiteName)
                .Select(s => new SiteLookupDto
                {
                    Id = s.Id,
                    Code = s.Code ?? string.Empty,
                    Name = s.SiteName ?? string.Empty,
                    ShortName = s.SiteName ?? string.Empty,
                    Description = s.SiteName ?? string.Empty,
                })
                .ToListAsync(cancellationToken);
        }
    }
}
