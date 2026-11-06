using System.Text.Json.Serialization;

namespace Organization.Service.DTOs
{
    public sealed class LocationPointV3Dto
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }


    public sealed class InstitutionV3Dto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("entityTypeId")]
        public int? EntityTypeId { get; set; }

        [JsonPropertyName("entityType")]
        public string? EntityType { get; set; }

        [JsonPropertyName("headEntityId")]
        public int? HeadEntityId { get; set; }

        [JsonPropertyName("headCampus")]
        public string? HeadCampus { get; set; }

        [JsonPropertyName("shortName")]
        public string? ShortName { get; set; }

        [JsonPropertyName("estDate")]
        public string? EstDate { get; set; }

        [JsonPropertyName("instituteName")]
        public string? InstituteName { get; set; }

        [JsonPropertyName("academicRegisterId")]
        public int? AcademicRegisterId { get; set; }

        [JsonPropertyName("academicRegister")]
        public string? AcademicRegister { get; set; }

        [JsonPropertyName("statusId")]
        public int? StatusId { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("institutionTypeId")]
        public int? InstitutionTypeId { get; set; }

        [JsonPropertyName("institutionType")]
        public string? InstitutionType { get; set; }

        [JsonPropertyName("instituteTypeId")]
        public int? InstituteTypeId { get; set; }

        [JsonPropertyName("instituteType")]
        public string? InstituteType { get; set; }

        [JsonPropertyName("campusTypeId")]
        public int? CampusTypeId { get; set; }

        [JsonPropertyName("campusType")]
        public string? CampusType { get; set; }

        [JsonPropertyName("slogan")]
        public string? Slogan { get; set; }

        [JsonPropertyName("owner")]
        public string? Owner { get; set; }

        [JsonPropertyName("contactNo")]
        public string? ContactNo { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("countryId")]
        public int? CountryId { get; set; }

        [JsonPropertyName("country")]
        public string? Country { get; set; }

        [JsonPropertyName("stateId")]
        public int? StateId { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("cityId")]
        public int? CityId { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("regionId")]
        public int? RegionId { get; set; }

        [JsonPropertyName("region")]
        public string? Region { get; set; }

        [JsonPropertyName("latitude")]
        public double? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double? Longitude { get; set; }

        [JsonPropertyName("locationPolygon")]
        public List<LocationPointV3Dto> LocationPolygon { get; set; } = new();

        [JsonPropertyName("logoImage")]
        public string? LogoImage { get; set; }

        [JsonPropertyName("watermarkImage")]
        public string? WatermarkImage { get; set; }

        [JsonPropertyName("imageId")]
        public long? ImageId { get; set; }

        [JsonPropertyName("watermarkLogo")]
        public long? WatermarkLogo { get; set; }


        [JsonPropertyName("employees")]
        public List<InstitutionEmployeeV3Dto> Employees { get; set; } = new();

        [JsonPropertyName("contacts")]
        public List<InstitutionContactV3Dto> Contacts { get; set; } = new();

        [JsonPropertyName("payments")]
        public List<InstitutionPaymentV3Dto> Payments { get; set; } = new();
    }

    public sealed class InstitutionEmployeeV3Dto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("typeId")]
        public int? TypeId { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("employeeId")]
        public int? EmployeeId { get; set; }

        [JsonPropertyName("employee")]
        public string? Employee { get; set; }
    }

    public sealed class InstitutionContactV3Dto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("contactTypeId")]
        public int? ContactTypeId { get; set; }

        [JsonPropertyName("contactType")]
        public string? ContactType { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }
    }

    public sealed class InstitutionPaymentV3Dto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("vendorId")]
        public int? VendorId { get; set; }

        [JsonPropertyName("vendor")]
        public string? Vendor { get; set; }

        [JsonPropertyName("prefix")]
        public string? Prefix { get; set; }

        [JsonPropertyName("payMethodId")]
        public int? PayMethodId { get; set; }

        [JsonPropertyName("payMethod")]
        public string? PayMethod { get; set; }

        [JsonPropertyName("bankId")]
        public int? BankId { get; set; }

        [JsonPropertyName("bank")]
        public string? Bank { get; set; }

        [JsonPropertyName("isDefault")]
        public bool IsDefault { get; set; }
    }

    public sealed class SaveInstitutionV3Request
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("entityTypeId")]
        public int? EntityTypeId { get; set; }

        [JsonPropertyName("headEntityId")]
        public int? HeadEntityId { get; set; }

        [JsonPropertyName("shortName")]
        public string? ShortName { get; set; }

        [JsonPropertyName("estDate")]
        public string? EstDate { get; set; }

        [JsonPropertyName("instituteName")]
        public string? InstituteName { get; set; }

        [JsonPropertyName("academicRegisterId")]
        public int? AcademicRegisterId { get; set; }

        [JsonPropertyName("statusId")]
        public int? StatusId { get; set; }

        [JsonPropertyName("institutionTypeId")]
        public int? InstitutionTypeId { get; set; }

        [JsonPropertyName("instituteTypeId")]
        public int? InstituteTypeId { get; set; }

        [JsonPropertyName("campusTypeId")]
        public int? CampusTypeId { get; set; }

        [JsonPropertyName("slogan")]
        public string? Slogan { get; set; }

        [JsonPropertyName("owner")]
        public string? Owner { get; set; }

        [JsonPropertyName("contactNo")]
        public string? ContactNo { get; set; }

        [JsonPropertyName("address")]
        public string? Address { get; set; }

        [JsonPropertyName("countryId")]
        public int? CountryId { get; set; }

        [JsonPropertyName("stateId")]
        public int? StateId { get; set; }

        [JsonPropertyName("cityId")]
        public int? CityId { get; set; }

        [JsonPropertyName("regionId")]
        public int? RegionId { get; set; }

        [JsonPropertyName("latitude")]
        public double? Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double? Longitude { get; set; }

        [JsonPropertyName("locationPolygon")]
        public List<LocationPointV3Dto> LocationPolygon { get; set; } = new();

        [JsonPropertyName("logoImage")]
        public string? LogoImage { get; set; }

        [JsonPropertyName("watermarkImage")]
        public string? WatermarkImage { get; set; }

        [JsonPropertyName("imageId")]
        public long? ImageId { get; set; }

        [JsonPropertyName("watermarkLogo")]
        public long? WatermarkLogo { get; set; }


        [JsonPropertyName("employees")]
        public List<InstitutionEmployeeV3Dto> Employees { get; set; } = new();

        [JsonPropertyName("contacts")]
        public List<InstitutionContactV3Dto> Contacts { get; set; } = new();

        [JsonPropertyName("payments")]
        public List<InstitutionPaymentV3Dto> Payments { get; set; } = new();
    }

    public sealed class InstitutionLookupsV3Dto
    {
        [JsonPropertyName("entityTypes")]
        public List<LookupItemDto> EntityTypes { get; set; } = new();

        [JsonPropertyName("headCampuses")]
        public List<LookupItemDto> HeadCampuses { get; set; } = new();

        [JsonPropertyName("institutionTypes")]
        public List<LookupItemDto> InstitutionTypes { get; set; } = new();

        [JsonPropertyName("instituteTypes")]
        public List<LookupItemDto> InstituteTypes { get; set; } = new();

        [JsonPropertyName("campusTypes")]
        public List<LookupItemDto> CampusTypes { get; set; } = new();

        [JsonPropertyName("statuses")]
        public List<LookupItemDto> Statuses { get; set; } = new();

        [JsonPropertyName("contactTypes")]
        public List<LookupItemDto> ContactTypes { get; set; } = new();

        [JsonPropertyName("payMethods")]
        public List<LookupItemDto> PayMethods { get; set; } = new();

        [JsonPropertyName("academicRegisters")]
        public List<LookupItemDto> AcademicRegisters { get; set; } = new();

        [JsonPropertyName("regions")]
        public List<LookupItemDto> Regions { get; set; } = new();

        [JsonPropertyName("paymentVendors")]
        public List<LookupItemDto> PaymentVendors { get; set; } = new();

        [JsonPropertyName("banks")]
        public List<LookupItemDto> Banks { get; set; } = new();

        [JsonPropertyName("employees")]
        public List<LookupItemDto> Employees { get; set; } = new();

        [JsonPropertyName("employeeTypes")]
        public List<LookupItemDto> EmployeeTypes { get; set; } = new();

        [JsonPropertyName("countries")]
        public List<LookupItemDto> Countries { get; set; } = new();

        [JsonPropertyName("states")]
        public List<LookupItemDto> States { get; set; } = new();

        [JsonPropertyName("cities")]
        public List<LookupItemDto> Cities { get; set; } = new();
    }

    public sealed class LookupItemDto
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
