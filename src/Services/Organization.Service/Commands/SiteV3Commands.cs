using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Commands
{
    public sealed class CreateSiteV3Command : IRequest<SiteV3Dto>
    {
        public string Code { get; set; } = string.Empty;
        public string SiteName { get; set; } = string.Empty;
        public int SiteTypeId { get; set; }
        public int SiteForId { get; set; }
        public int RegionId { get; set; }
        public int? ForId { get; set; }
        public int ClientId { get; set; }
        public int EntityId { get; set; }
        public int CreatedBy { get; set; }
    }

    public sealed class UpdateSiteV3Command : IRequest<SiteV3Dto>
    {
        public int SiteId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string SiteName { get; set; } = string.Empty;
        public int SiteTypeId { get; set; }
        public int SiteForId { get; set; }
        public int RegionId { get; set; }
        public int? ForId { get; set; }
        public int ClientId { get; set; }
        public int EntityId { get; set; }
        public int UpdatedBy { get; set; }
    }

    public sealed class DeleteSiteV3Command : IRequest<bool>
    {
        public int SiteId { get; set; }
        public int ClientId { get; set; }
        public int EntityId { get; set; }
        public int DeletedBy { get; set; }
    }
}
