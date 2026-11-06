using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Queries
{
    public sealed class GetStatesV3Query : IRequest<PaginationResponse<StateV3Dto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; }
        public bool IncludeInactive { get; set; }
        public int? CountryId { get; set; }
        public int? ClientId { get; set; }
    }

    public sealed class GetStateByIdV3Query : IRequest<StateV3Dto>
    {
        public int StateId { get; set; }
        public int? ClientId { get; set; }
    }
}
