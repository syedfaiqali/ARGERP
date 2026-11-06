using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Queries
{
    public sealed class GetCitiesV3Query : IRequest<PaginationResponse<CityV3Dto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; }
        public bool IncludeInactive { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? ClientId { get; set; }
    }

    public sealed class GetCityByIdV3Query : IRequest<CityV3Dto>
    {
        public int CityId { get; set; }
        public int? ClientId { get; set; }
    }
}
