using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Queries
{
    public sealed class GetSitesV3Query : IRequest<PaginationResponse<SiteV3Dto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public int? RegionId { get; set; }
        public int ClientId { get; set; }
        public int EntityId { get; set; }
    }

    public sealed class GetSiteByIdV3Query : IRequest<SiteV3Dto>
    {
        public int SiteId { get; set; }
        public int ClientId { get; set; }
        public int EntityId { get; set; }
    }

    public sealed class GetSiteTypesV3Query : IRequest<List<SiteLookupDto>>
    {
    }

    public sealed class GetSiteForOptionsV3Query : IRequest<List<SiteLookupDto>>
    {
    }

    public sealed class GetSiteParentOptionsV3Query : IRequest<List<SiteLookupDto>>
    {
        public int ClientId { get; set; }
        public int EntityId { get; set; }
    }
}
