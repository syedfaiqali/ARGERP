namespace HumanResource.Service.DTOs
{
    public sealed record SkillTypeV3Dto(
        int Id,
        string Code,
        string Name,
        string? Description,
        bool IsActive
    );

    public sealed record CreateSkillTypeV3Request(
        string Code,
        string Name,
        string? Description,
        bool IsActive
    );

    public sealed record UpdateSkillTypeV3Request(
        int Id,
        string Code,
        string Name,
        string? Description,
        bool IsActive
    );
}
