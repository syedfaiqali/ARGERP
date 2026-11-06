using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Commands
{
    public sealed class CreateFloorV3Command : IRequest<FloorV3Dto>
    {
        public string Code { get; set; } = string.Empty;
        public string FloorName { get; set; } = string.Empty;
        public int BuildingId { get; set; }
        public int ClientId { get; set; }
        public int EntityId { get; set; }
        public int CreatedBy { get; set; }
    }

    public sealed class UpdateFloorV3Command : IRequest<FloorV3Dto>
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string FloorName { get; set; } = string.Empty;
        public int BuildingId { get; set; }
        public int ClientId { get; set; }
        public int EntityId { get; set; }
        public int UpdatedBy { get; set; }
    }

    public sealed class DeleteFloorV3Command : IRequest<bool>
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int EntityId { get; set; }
        public int DeletedBy { get; set; }
    }
}
