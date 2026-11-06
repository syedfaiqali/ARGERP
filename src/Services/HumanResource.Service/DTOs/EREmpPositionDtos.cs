using System;

namespace HumanResource.Service.DTOs
{
    public sealed record EREmpPositionV3Dto(
        int Id,
        int? EmployeeId,
        int? DepartmentId,
        string? DepartmentName,
        int? DesignationId,
        string? DesignationName,
        int? GradeId,
        string? GradeName,
        int? ActionId,
        string? ActionName,
        string? Remarks,
        DateTime? FromDate,
        DateTime? TillDate,
        string? Responsibility
    );

    public sealed record UpsertEREmpPositionV3Request(
        int? Id,
        int EmployeeId,
        int DepartmentId,
        int DesignationId,
        int? GradeId,
        int? ActionId,
        string? Remarks,
        DateTime FromDate,
        DateTime? TillDate,
        string? Responsibility
    );
}
