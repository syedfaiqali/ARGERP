namespace HumanResource.Service.DTOs
{
    public sealed record DeductionTypeLookupOptionV3Dto(
        int Id,
        string Code,
        string Name
    );

    public sealed record DeductionTypeLookupsV3Dto(
        IReadOnlyList<DeductionTypeLookupOptionV3Dto> PaymentRunTypes
    );

    public sealed record DeductionTypeV3Dto(
        int Id,
        string Code,
        string DeductionType,
        string? Description,
        int? PaymentRunTypeId,
        string? PaymentRunType,
        bool Reimbursed,
        bool IsActive
    );

    public sealed record CreateDeductionTypeV3Request(
        string? Code,
        string DeductionType,
        string? Description,
        int? PaymentRunTypeId,
        bool Reimbursed,
        bool IsActive
    );

    public sealed record UpdateDeductionTypeV3Request(
        int Id,
        string? Code,
        string DeductionType,
        string? Description,
        int? PaymentRunTypeId,
        bool Reimbursed,
        bool IsActive
    );
}
