using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Queries
{
    public sealed class GetRoomsV3Query : IRequest<PaginationResponse<RoomV3Dto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public int? BuildingId { get; set; }
        public int? FloorId { get; set; }
        public int? RoomTypeId { get; set; }
        public int ClientId { get; set; }
        public int EntityId { get; set; }
    }

    public sealed class GetRoomByIdV3Query : IRequest<RoomV3Dto>
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int EntityId { get; set; }
    }

    public sealed class GetRoomTypesV3Query : IRequest<List<RoomLookupDto>>
    {
    }

    public sealed class GetRoomBuildingsV3Query : IRequest<List<RoomLookupDto>>
    {
        public int ClientId { get; set; }
        public int EntityId { get; set; }
    }

    public sealed class GetRoomFloorsV3Query : IRequest<List<RoomLookupDto>>
    {
        public int ClientId { get; set; }
        public int EntityId { get; set; }
        public int? BuildingId { get; set; }
    }
}
