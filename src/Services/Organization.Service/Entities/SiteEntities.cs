using System;
using System.Linq;

namespace Organization.Service.Entities
{
    public class OrgSite
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? SiteName { get; set; }
        public int? SiteTypeId { get; set; }
        public int? RegionId { get; set; }
        public int? SiteForId { get; set; }
        public int? ForId { get; set; }
        public int? EntityId { get; set; }
        public int? ClientId { get; set; }
        public int? ParentSiteId { get; set; }
        public bool utsysgen { get; set; } = false;
        public bool utblock { get; set; } = false;
        public bool utispartial { get; set; } = false;
        public int utrecordversion { get; set; } = 0;

        public virtual OrgRegion? Region { get; set; }
        public virtual OrgSite? ParentSite { get; set; }
    }

    public class Lov
    {
        public int Id { get; set; }
        public string? code { get; set; }
        public string? stxt { get; set; }
        public string? ltxt { get; set; }
        public int? plovid { get; set; }
        public string? vtype { get; set; }
        public bool utblock { get; set; }
        public int? indx { get; set; }
    }
}
