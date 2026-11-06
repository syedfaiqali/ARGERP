namespace HumanResource.Service.DTOs
{
    public sealed record EmploymentV3Dto(
        int Id,
        int? EmployeeId,
        int? EmploymentTypeId,
        string EmploymentType,
        DateTime? StartDate,
        DateTime? EndDate,
        bool IsActive
    );

    public sealed record EmploymentUpsertV3Request(
        int? Id,
        int? EmployeeId,
        int? EmploymentTypeId,
        DateTime? StartDate,
        DateTime? EndDate
    );
}
