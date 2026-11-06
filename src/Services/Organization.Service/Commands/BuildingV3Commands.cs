using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Commands
{
    public sealed class CreateBuildingV3Command : IRequest<BuildingV3Dto>
    {
        public string Code { get; set; } = string.Empty;
        public string BuildingName { get; set; } = string.Empty;
        public int ClientId { get; set; }
        public int EntityId { get; set; }
        public int CreatedBy { get; set; }
    }

    public sealed class UpdateBuildingV3Command : IRequest<BuildingV3Dto>
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string BuildingName { get; set; } = string.Empty;
        public int ClientId { get; set; }
        public int EntityId { get; set; }
        public int UpdatedBy { get; set; }
    }

    public sealed class DeleteBuildingV3Command : IRequest<bool>
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int EntityId { get; set; }
        public int DeletedBy { get; set; }
    }
}
