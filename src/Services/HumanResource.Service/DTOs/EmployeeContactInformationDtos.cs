namespace HumanResource.Service.DTOs
{
    public sealed record EmployeeContactInformationV3Dto(
        int Id,
        string? Code,
        int? EmployeeId,
        int? RelationTypeId,
        string? RelationType,
        int? GenderId,
        string? Gender,
        string? FirstName,
        string? LastName,
        string? Address,
        string? PhoneNo,
        string? MobileNo,
        string? Email,
        bool Emergency,
        bool NextOfKin);

    public sealed record UpsertEmployeeContactInformationV3Request(
        int? Id,
        string? Code,
        int EmployeeId,
        int? RelationTypeId,
        int? GenderId,
        string? FirstName,
        string? LastName,
        string? Address,
        string? PhoneNo,
        string? MobileNo,
        string? Email,
        bool? Emergency,
        bool? NextOfKin);

    public sealed record ContactLookupV3Dto(int Id, string? Code, string Name);
}
