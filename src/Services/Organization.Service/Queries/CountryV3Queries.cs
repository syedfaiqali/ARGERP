using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Queries
{
    public class GetCountriesV3Query : IRequest<PaginationResponse<CountryV3Dto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; }
        public bool IncludeInactive { get; set; }
        public int? ClientId { get; set; }
    }

    public class GetCountryByIdV3Query : IRequest<CountryV3Dto>
    {
        public int CountryId { get; set; }
        public int? ClientId { get; set; }
    }
}
