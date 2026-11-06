using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Queries
{
    public sealed class GetBuildingsV3Query : IRequest<PaginationResponse<BuildingV3Dto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public int ClientId { get; set; }
        public int EntityId { get; set; }
    }

    public sealed class GetBuildingByIdV3Query : IRequest<BuildingV3Dto>
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int EntityId { get; set; }
    }
}
