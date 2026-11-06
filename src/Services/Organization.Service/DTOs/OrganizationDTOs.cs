namespace Organization.Service.DTOs
{
    // ==================== Country DTOs ====================
    public class CountryDto
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string CountryCode3 { get; set; }
        public string PhoneCode { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencySymbol { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCountryRequest
    {
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string CountryCode3 { get; set; }
        public string PhoneCode { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencySymbol { get; set; }
    }

    public class UpdateCountryRequest
    {
        public string CountryName { get; set; }
        public string PhoneCode { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencySymbol { get; set; }
        public bool IsActive { get; set; }
    }

    // ==================== State DTOs ====================
    public class StateDto
    {
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string StateCode { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateStateRequest
    {
        public int CountryId { get; set; }
        public string StateName { get; set; }
        public string StateCode { get; set; }
    }

    public class UpdateStateRequest
    {
        public string StateName { get; set; }
        public string StateCode { get; set; }
        public bool IsActive { get; set; }
    }

    // ==================== City DTOs ====================
    public class CityDto
    {
        public int CityId { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string StateName { get; set; }
        public string CityName { get; set; }
        public string CityCode { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCityRequest
    {
        public int StateId { get; set; }
        public string CityName { get; set; }
        public string CityCode { get; set; }
    }

    public class UpdateCityRequest
    {
        public string CityName { get; set; }
        public string CityCode { get; set; }
        public bool IsActive { get; set; }
    }

    // ==================== District DTOs ====================
    public class DistrictDto
    {
        public int DistrictId { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public string DistrictName { get; set; }
        public string DistrictCode { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateDistrictRequest
    {
        public int CityId { get; set; }
        public string DistrictName { get; set; }
        public string DistrictCode { get; set; }
    }

    public class UpdateDistrictRequest
    {
        public string DistrictName { get; set; }
        public string DistrictCode { get; set; }
        public bool IsActive { get; set; }
    }

    // ==================== OrgEntity DTOs ====================
    public class OrgEntityDto
    {
        public int EntityId { get; set; }
        public int ClientId { get; set; }
        public string EntityCode { get; set; }
        public string EntityName { get; set; }
        public string EntityType { get; set; }
        public string Address { get; set; }
        public string CityName { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string WebsiteUrl { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateOrgEntityRequest
    {
        public string EntityCode { get; set; }
        public string EntityName { get; set; }
        public string EntityType { get; set; }
        public int CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string WebsiteUrl { get; set; }
    }

    public class UpdateOrgEntityRequest
    {
        public string EntityName { get; set; }
        public string EntityType { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string WebsiteUrl { get; set; }
        public bool IsActive { get; set; }
    }

    // ==================== OrgCampus DTOs ====================
    public class OrgCampusDto
    {
        public int CampusId { get; set; }
        public int EntityId { get; set; }
        public int ClientId { get; set; }
        public string EntityName { get; set; }
        public string CampusCode { get; set; }
        public string CampusName { get; set; }
        public string CampusType { get; set; }
        public string Address { get; set; }
        public string CityName { get; set; }
        public string CountryName { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string PrincipalName { get; set; }
        public string PrincipalEmail { get; set; }
        public DateTime EstablishedDate { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateOrgCampusRequest
    {
        public int EntityId { get; set; }
        public string CampusCode { get; set; }
        public string CampusName { get; set; }
        public string CampusType { get; set; }
        public int CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string PrincipalName { get; set; }
        public string PrincipalEmail { get; set; }
        public string PrincipalPhone { get; set; }
        public DateTime EstablishedDate { get; set; }
    }

    public class UpdateOrgCampusRequest
    {
        public string CampusName { get; set; }
        public string CampusType { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string PrincipalName { get; set; }
        public string PrincipalEmail { get; set; }
        public string PrincipalPhone { get; set; }
        public bool IsActive { get; set; }
    }

    // ==================== OrgDepartment DTOs ====================
    public class OrgDepartmentDto
    {
        public int DepartmentId { get; set; }
        public int CampusId { get; set; }
        public int ClientId { get; set; }
        public string CampusName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentType { get; set; }
        public string HeadName { get; set; }
        public string HeadEmail { get; set; }
        public string HeadPhone { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateOrgDepartmentRequest
    {
        public int CampusId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentType { get; set; }
        public string HeadName { get; set; }
        public string HeadEmail { get; set; }
        public string HeadPhone { get; set; }
        public string Description { get; set; }
    }

    public class UpdateOrgDepartmentRequest
    {
        public string DepartmentName { get; set; }
        public string DepartmentType { get; set; }
        public string HeadName { get; set; }
        public string HeadEmail { get; set; }
        public string HeadPhone { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    // ==================== OrgDesignation DTOs ====================
    public class OrgDesignationDto
    {
        public int DesignationId { get; set; }
        public int DepartmentId { get; set; }
        public int ClientId { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationCode { get; set; }
        public string DesignationName { get; set; }
        public string DesignationType { get; set; }
        public int? ReportingToDesignationId { get; set; }
        public string ReportingToDesignationName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateOrgDesignationRequest
    {
        public int DepartmentId { get; set; }
        public string DesignationCode { get; set; }
        public string DesignationName { get; set; }
        public string DesignationType { get; set; }
        public int? ReportingToDesignationId { get; set; }
        public string Description { get; set; }
    }

    public class UpdateOrgDesignationRequest
    {
        public string DesignationName { get; set; }
        public string DesignationType { get; set; }
        public int? ReportingToDesignationId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    // ==================== Reference Data DTOs ====================
    public class OrgLanguageDto
    {
        public int LanguageId { get; set; }
        public string LanguageName { get; set; }
        public string LanguageCode { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }

    public class OrgCasteDto
    {
        public int CasteId { get; set; }
        public string CasteName { get; set; }
        public string CasteCode { get; set; }
        public bool IsActive { get; set; }
    }

    public class OccupationDto
    {
        public int OccupationId { get; set; }
        public string OccupationName { get; set; }
        public string OccupationCode { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }

    // ==================== Pagination Support ====================
    public class PaginationRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; }
        public string SortBy { get; set; }
        public bool SortDescending { get; set; } = true;
    }

    public class PaginationResponse<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

    public class LookupDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
    }
}
