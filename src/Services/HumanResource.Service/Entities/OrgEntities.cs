namespace HumanResource.Service.Entities
{
    public sealed class OrgLegalentityLegacy
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public string? EntityNo { get; set; }
        public string? ShortName { get; set; }
        public string? Stxt { get; set; }
        public string? CampusName { get; set; }
        public int? HeadEntityId { get; set; }
        public int? CampusTypeId { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public bool UtBlock { get; set; }
    }

    public sealed class SMPolicyLegacy
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public int? EntityId { get; set; }
        public int? DefaultCountryId { get; set; }
        public int? DefaultCityId { get; set; }
        public bool? UtBlock { get; set; }
    }

    public sealed class SMAcadPolicyVacancyLegacy
    {
        public int Id { get; set; }
        public int? ClientId { get; set; }
        public int? EntityId { get; set; }
        public bool? UseVacancyProcess { get; set; }
        public bool? UtBlock { get; set; }
    }

    public sealed class UtinstanceLegacy
    {
        public int instanceid { get; set; }
        public string? dbserver { get; set; }
        public string? dbuserid { get; set; }
        public string? dbpassword { get; set; }
        public string? dbname { get; set; }
        public bool utblock { get; set; }
    }
}
