namespace HumanResource.Service.DTOs
{
    public sealed record EmployeeEducationV3Dto(
        int Id,
        int? EmployeeId,
        int? QualificationTypeId,
        string? QualificationType,
        int? QualificationId,
        string? Qualification,
        string? Institute,
        double? Cgpa,
        double? Percentage,
        string? Grade,
        string? Division,
        int? PassingYear);

    public sealed record UpsertEmployeeEducationV3Request(
        int? Id,
        int EmployeeId,
        int? QualificationTypeId,
        int? QualificationId,
        string? Institute,
        double? Cgpa,
        double? Percentage,
        string? Grade,
        string? Division,
        int? PassingYear);
}
