namespace HumanResource.Service.DTOs
{
    public sealed record EarningGroupLookupOptionV3Dto(
        int Id,
        string Code,
        string ShortName
    );

    public sealed record EarningGroupLookupsV3Dto(
        List<EarningGroupLookupOptionV3Dto> EarningTypes,
        List<EarningGroupLookupOptionV3Dto> RateBasis,
        List<EarningGroupLookupOptionV3Dto> Statuses
    );

    public sealed record EarningGroupDetailV3Dto(
        int Id,
        int LineIndex,
        int? EarnTypeId,
        string? EarnTypeName,
        int? RateBasisId,
        string? RateBasisName,
        int? BasisEarnTypeId,
        string? BasisEarnTypeName,
        double? Value,
        int? PaySlipIndex,
        string? PaySlipText,
        int? StatusId,
        string? StatusName,
        bool AbsNotEffective,
        bool LevNotEffective,
        bool IsOtPart
    );

    public sealed record EarningGroupV3Dto(
        int Id,
        string Code,
        string EarningGroup,
        bool IsActive
    );

    public sealed record EarningGroupHeaderV3Dto(
        int Id,
        string Code,
        string EarningGroup,
        bool IsActive,
        List<EarningGroupDetailV3Dto> Details
    );

    public sealed record CreateEarningGroupDetailV3Request(
        int? Id,
        int LineIndex,
        int? EarnTypeId,
        int? RateBasisId,
        int? BasisEarnTypeId,
        double? Value,
        int? PaySlipIndex,
        string? PaySlipText,
        int? StatusId,
        bool AbsNotEffective,
        bool LevNotEffective,
        bool IsOtPart
    );

    public sealed record CreateEarningGroupV3Request(
        string? Code,
        string EarningGroup,
        bool IsActive,
        List<CreateEarningGroupDetailV3Request> Details
    );

    public sealed record UpdateEarningGroupV3Request(
        int Id,
        string? Code,
        string EarningGroup,
        bool IsActive,
        List<CreateEarningGroupDetailV3Request> Details
    );
}
