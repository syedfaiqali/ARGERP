namespace HumanResource.Service.DTOs
{
    public sealed record DeductionGroupLookupOptionV3Dto(
        int Id,
        string Code,
        string ShortName
    );

    public sealed record DeductionGroupLookupsV3Dto(
        List<DeductionGroupLookupOptionV3Dto> DeductionTypes,
        List<DeductionGroupLookupOptionV3Dto> EarningTypes,
        List<DeductionGroupLookupOptionV3Dto> RateBasis,
        List<DeductionGroupLookupOptionV3Dto> Statuses
    );

    public sealed record DeductionGroupDetailV3Dto(
        int Id,
        int LineIndex,
        int? DeductionTypeId,
        string? DeductionTypeName,
        int? RateBasisId,
        string? RateBasisName,
        int? BasisEarningTypeId,
        string? BasisEarningTypeName,
        double? Value,
        int? PaySlipOrder,
        string? PaySlipDescription,
        int? StatusId,
        string? StatusName
    );

    public sealed record DeductionGroupV3Dto(
        int Id,
        string Code,
        string DeductionGroup,
        bool IsActive
    );

    public sealed record DeductionGroupHeaderV3Dto(
        int Id,
        string Code,
        string DeductionGroup,
        bool IsActive,
        List<DeductionGroupDetailV3Dto> Details
    );

    public sealed record CreateDeductionGroupDetailV3Request(
        int? Id,
        int LineIndex,
        int? DeductionTypeId,
        int? RateBasisId,
        int? BasisEarningTypeId,
        double? Value,
        int? PaySlipOrder,
        string? PaySlipDescription,
        int? StatusId
    );

    public sealed record CreateDeductionGroupV3Request(
        string? Code,
        string DeductionGroup,
        bool IsActive,
        List<CreateDeductionGroupDetailV3Request> Details
    );

    public sealed record UpdateDeductionGroupV3Request(
        int Id,
        string? Code,
        string DeductionGroup,
        bool IsActive,
        List<CreateDeductionGroupDetailV3Request> Details
    );
}
