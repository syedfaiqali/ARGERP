namespace HumanResource.Service.DTOs
{
    public sealed record EmployeeSubjectRatingV3Dto(
        int Id,
        int? EmployeeId,
        int? SubjectTypeId,
        string? SubjectType,
        int? RatingDetailId,
        string? Rating,
        string? RatingModel,
        int? RatingValue);

    public sealed record EmployeeSubjectRatingLookupV3Dto(
        int Id,
        string? Code,
        string Name,
        int? RatingId = null,
        string? RatingModel = null,
        int? RatingValue = null);

    public sealed record UpsertEmployeeSubjectRatingV3Request(
        int? Id,
        int EmployeeId,
        int? SubjectTypeId,
        int? RatingDetailId,
        int? RatingValue);
}
