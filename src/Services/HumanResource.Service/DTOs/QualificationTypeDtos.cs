namespace HumanResource.Service.DTOs
{
    public sealed record QualificationTypeV3Dto(
        int Id,
        string Code,
        string Name,
        string? Description,
        bool IsActive
    );

    public sealed record CreateQualificationTypeV3Request(
        string Code,
        string Name,
        string? Description,
        bool IsActive
    );

    public sealed record UpdateQualificationTypeV3Request(
        int Id,
        string Code,
        string Name,
        string? Description,
        bool IsActive
    );
}
