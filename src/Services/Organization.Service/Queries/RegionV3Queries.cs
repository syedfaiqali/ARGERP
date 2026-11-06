using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Queries
{
    public sealed class GetRegionsV3Query : IRequest<PaginationResponse<RegionV3Dto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; }
        public bool IncludeInactive { get; set; }
        public int? CountryId { get; set; }
        public int? ClientId { get; set; }
    }

    public sealed class GetRegionByIdV3Query : IRequest<RegionV3Dto>
    {
        public int RegionId { get; set; }
        public int? ClientId { get; set; }
    }
}
