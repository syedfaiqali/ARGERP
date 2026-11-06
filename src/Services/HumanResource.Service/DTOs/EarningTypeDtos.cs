namespace HumanResource.Service.DTOs
{
    public sealed record EarningTypeLookupOptionV3Dto(
        int Id,
        string Code,
        string Name
    );

    public sealed record EarningTypeLookupsV3Dto(
        IReadOnlyList<EarningTypeLookupOptionV3Dto> PaymentRunTypes,
        IReadOnlyList<EarningTypeLookupOptionV3Dto> TaxMethods,
        IReadOnlyList<EarningTypeLookupOptionV3Dto> RateBasis,
        IReadOnlyList<EarningTypeLookupOptionV3Dto> Uoms,
        IReadOnlyList<EarningTypeLookupOptionV3Dto> BasisEarningTypes
    );

    public sealed record EarningTypeV3Dto(
        int Id,
        string Code,
        string EarningType,
        string? Description,
        bool BaseSalary,
        string? PayslipIndex,
        string? PayslipDescription,
        int? UomId,
        string? Uom,
        int? PaymentRunTypeId,
        string? PaymentRunType,
        int? TaxMethodId,
        string? TaxMethod,
        int? RateBasisId,
        string? RateBasis,
        int? BasisEarningTypeId,
        string? BasisEarningType,
        bool IsActive
    );

    public sealed record CreateEarningTypeV3Request(
        string? Code,
        string EarningType,
        string? Description,
        bool BaseSalary,
        string? PayslipIndex,
        string? PayslipDescription,
        int? UomId,
        int? PaymentRunTypeId,
        int? TaxMethodId,
        int? RateBasisId,
        int? BasisEarningTypeId,
        bool IsActive
    );

    public sealed record UpdateEarningTypeV3Request(
        int Id,
        string? Code,
        string EarningType,
        string? Description,
        bool BaseSalary,
        string? PayslipIndex,
        string? PayslipDescription,
        int? UomId,
        int? PaymentRunTypeId,
        int? TaxMethodId,
        int? RateBasisId,
        int? BasisEarningTypeId,
        bool IsActive
    );
}
