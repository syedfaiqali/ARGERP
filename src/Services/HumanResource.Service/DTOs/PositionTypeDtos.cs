namespace HumanResource.Service.DTOs
{
    public sealed record PositionTypeV3Dto(
        int Id,
        string Code,
        int? EmployeeTypeId,
        string? EmployeeTypeName,
        string Name,
        string? Description,
        bool IsActive
    );

    public sealed record CreatePositionTypeV3Request(
        string Code,
        int? EmployeeTypeId,
        string Name,
        string? Description,
        bool IsActive
    );

    public sealed record UpdatePositionTypeV3Request(
        int Id,
        string Code,
        int? EmployeeTypeId,
        string Name,
        string? Description,
        bool IsActive
    );
}
