using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Queries
{
    public sealed class GetFloorsV3Query : IRequest<PaginationResponse<FloorV3Dto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public int? BuildingId { get; set; }
        public int ClientId { get; set; }
        public int EntityId { get; set; }
    }

    public sealed class GetFloorByIdV3Query : IRequest<FloorV3Dto>
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int EntityId { get; set; }
    }
}
