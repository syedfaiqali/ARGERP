namespace HumanResource.Service.DTOs
{
    public sealed record EmployeeAddressV3Dto(
        int Id,
        string? Code,
        int? EmployeeId,
        string? Name,
        int? AddressTypeId,
        int? CountryId,
        int? StateId,
        int? CityId,
        string? Address,
        bool PrimaryAddress,
        string? ContactName,
        string? Landline,
        string? Ext,
        string? Mobile);

    public sealed record UpsertEmployeeAddressV3Request(
        int? Id,
        string? Code,
        int EmployeeId,
        string? Name,
        int? AddressTypeId,
        int? CountryId,
        int? StateId,
        int? CityId,
        string? Address,
        bool PrimaryAddress,
        string? ContactName,
        string? Landline,
        string? Ext,
        string? Mobile);
}
