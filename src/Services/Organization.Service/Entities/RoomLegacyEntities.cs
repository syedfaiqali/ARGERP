namespace Organization.Service.Entities
{
    public class OrgRoomLegacy
    {
        public int Id { get; set; }
        public string? code { get; set; }
        public string? stxt { get; set; }
        public int? entityid { get; set; }
        public int? clientid { get; set; }
        public int? lovfloorid { get; set; }
        public int? roomtypeid { get; set; }
        public int? buildingid { get; set; }
        public int? strength { get; set; }
        public bool utsysgen { get; set; } = false;
        public bool utblock { get; set; } = false;
        public bool utispartial { get; set; } = false;
        public int utrecordversion { get; set; } = 0;
    }

    public class OrgBuildingLegacy
    {
        public int Id { get; set; }
        public string? code { get; set; }
        public string? stxt { get; set; }
        public int? entityid { get; set; }
        public int? clientid { get; set; }
        public bool utsysgen { get; set; } = false;
        public bool utblock { get; set; } = false;
        public bool utispartial { get; set; } = false;
        public int utrecordversion { get; set; } = 0;
    }

    public class OrgFloorLegacy
    {
        public int Id { get; set; }
        public string? code { get; set; }
        public string? stxt { get; set; }
        public int? entityid { get; set; }
        public int? clientid { get; set; }
        public int? buildingid { get; set; }
        public bool utsysgen { get; set; } = false;
        public bool utblock { get; set; } = false;
        public bool utispartial { get; set; } = false;
        public int utrecordversion { get; set; } = 0;
    }
}
