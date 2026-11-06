namespace HumanResource.Service.DTOs
{
    public sealed record SkillV3Dto(
        int Id,
        string Code,
        int? SkillTypeId,
        string? SkillTypeName,
        string Name,
        string? Description,
        int? RatingModelId,
        string? RatingModelName,
        bool IsActive
    );

    public sealed record CreateSkillV3Request(
        string Code,
        int? SkillTypeId,
        string Name,
        string? Description,
        int? RatingModelId,
        bool IsActive
    );

    public sealed record UpdateSkillV3Request(
        int Id,
        string Code,
        int? SkillTypeId,
        string Name,
        string? Description,
        int? RatingModelId,
        bool IsActive
    );
}
