using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Commands
{
    public sealed class CreateRoomV3Command : IRequest<RoomV3Dto>
    {
        public string Code { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public int RoomTypeId { get; set; }
        public int BuildingId { get; set; }
        public int FloorId { get; set; }
        public int? Strength { get; set; }
        public int ClientId { get; set; }
        public int EntityId { get; set; }
        public int CreatedBy { get; set; }
    }

    public sealed class UpdateRoomV3Command : IRequest<RoomV3Dto>
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public int RoomTypeId { get; set; }
        public int BuildingId { get; set; }
        public int FloorId { get; set; }
        public int? Strength { get; set; }
        public int ClientId { get; set; }
        public int EntityId { get; set; }
        public int UpdatedBy { get; set; }
    }

    public sealed class DeleteRoomV3Command : IRequest<bool>
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int EntityId { get; set; }
        public int DeletedBy { get; set; }
    }
}
