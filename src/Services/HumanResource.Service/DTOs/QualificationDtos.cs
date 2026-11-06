namespace HumanResource.Service.DTOs
{

    public sealed record QualificationV3Dto(
        int Id,
        string Code,
        int? QualificationTypeId,
        string? QualificationTypeName,
        string Qualification,
        string? Description,
        int? LovtypeId,
        bool IsActive
    );

    public sealed record CreateQualificationV3Request(
        string Code,
        int? QualificationTypeId,
        string Qualification,
        string? Description,
        int? LovtypeId,
        bool IsActive
    );

    public sealed record UpdateQualificationV3Request(
        int Id,
        string Code,
        int? QualificationTypeId,
        string Qualification,
        string? Description,
        int? LovtypeId,
        bool IsActive
    );

}
