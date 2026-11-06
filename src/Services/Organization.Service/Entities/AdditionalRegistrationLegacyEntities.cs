namespace Organization.Service.Entities
{
    public class SMAdditionalregistrationLegacy
    {
        public int Id { get; set; }
        public string? caption { get; set; }
        public int? configid { get; set; }
        public int? entityid { get; set; }
        public int? clientid { get; set; }
        public bool hide { get; set; }
        public int? infoforid { get; set; }
        public int? sequance { get; set; }
        public bool utsysgen { get; set; } = false;
        public bool utblock { get; set; } = false;
        public bool utispartial { get; set; } = false;
        public int utrecordversion { get; set; } = 0;
    }

    public class SMAdditionalinfoconfigLegacy
    {
        public int Id { get; set; }
        public string? code { get; set; }
        public string? stxt { get; set; }
        public int indx { get; set; }
        public string? webicon { get; set; }
        public string? route { get; set; }
        public bool visible { get; set; }
        public string? _class { get; set; }
        public int? grponid { get; set; }
        public int? infoforid { get; set; }
        public bool utblock { get; set; }
    }
}
