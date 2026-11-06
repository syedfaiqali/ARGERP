namespace HumanResource.Service.DTOs
{
    public sealed record EmployeeSkillV3Dto(
        int Id,
        int? EmployeeId,
        int? SkillTypeId,
        string? SkillType,
        int? SkillId,
        string? Skill,
        int? SkillRatingId,
        string? SkillRating,
        int? SkillRatingDetailId,
        string? SkillRatingDetail,
        int? PeriodUnitId,
        string? PeriodUnit,
        int? Period,
        string? Remarks);

    public sealed record EmployeeSkillLookupV3Dto(
        int Id,
        string? Code,
        string Name,
        int? SkillTypeId = null,
        int? SkillRatingId = null);

    public sealed record UpsertEmployeeSkillV3Request(
        int? Id,
        int EmployeeId,
        int? SkillTypeId,
        int? SkillId,
        int? SkillRatingId,
        int? SkillRatingDetailId,
        int? PeriodUnitId,
        int? Period,
        string? Remarks);
}
