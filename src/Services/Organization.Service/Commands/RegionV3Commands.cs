using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Commands
{
    public sealed class CreateRegionV3Command : IRequest<RegionV3Dto>
    {
        public string RegionCode { get; set; }
        public string RegionName { get; set; }
        public int CountryId { get; set; }
        public bool IsActive { get; set; } = true;
        public int CreatedBy { get; set; }
        public int? ClientId { get; set; }
    }

    public sealed class UpdateRegionV3Command : IRequest<RegionV3Dto>
    {
        public int RegionId { get; set; }
        public string RegionCode { get; set; }
        public string RegionName { get; set; }
        public int CountryId { get; set; }
        public bool IsActive { get; set; }
        public int UpdatedBy { get; set; }
        public int? ClientId { get; set; }
    }

    public sealed class DeleteRegionV3Command : IRequest<bool>
    {
        public int RegionId { get; set; }
        public int DeletedBy { get; set; }
        public int? ClientId { get; set; }
    }
}
