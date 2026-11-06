namespace HumanResource.Service.DTOs
{
    public sealed record EmployeeExperienceV3Dto(
        int Id,
        string? Code,
        int? EmployeeId,
        int? BusinessNatureId,
        string? BusinessNature,
        string? Company,
        string? Department,
        string? Designation,
        DateTime? StartDate,
        DateTime? EndDate,
        int? Salary,
        string? Address,
        string? Phone,
        string? Email,
        string? Web,
        string? Responsibility,
        string? ContactPerson,
        string? PersonDesignation,
        string? PersonAddress,
        string? PersonPhone,
        string? PersonEmail,
        string? PersonWeb);

    public sealed record EmployeeExperienceLookupV3Dto(
        int Id,
        string? Code,
        string Name);

    public sealed record UpsertEmployeeExperienceV3Request(
        int? Id,
        string? Code,
        int EmployeeId,
        int? BusinessNatureId,
        string? Company,
        string? Department,
        string? Designation,
        DateTime? StartDate,
        DateTime? EndDate,
        int? Salary,
        string? Address,
        string? Phone,
        string? Email,
        string? Web,
        string? Responsibility,
        string? ContactPerson,
        string? PersonDesignation,
        string? PersonAddress,
        string? PersonPhone,
        string? PersonEmail,
        string? PersonWeb);
}
