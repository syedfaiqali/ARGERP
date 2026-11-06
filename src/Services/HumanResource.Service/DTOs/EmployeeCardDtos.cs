namespace HumanResource.Service.DTOs
{
    public sealed record EmployeeCardV3Dto(
        int Id,
        string? Code,
        int? EmployeeId,
        int? CardTypeId,
        string? CardType,
        int? AuthorityId,
        string? Authority,
        string? CardNo,
        DateTime? ValidFrom,
        DateTime? ValidTill);

    public sealed record EmployeeCardLookupV3Dto(
        int Id,
        string? Code,
        string Name);

    public sealed record UpsertEmployeeCardV3Request(
        int? Id,
        string? Code,
        int EmployeeId,
        int? CardTypeId,
        int? AuthorityId,
        string? CardNo,
        DateTime? ValidFrom,
        DateTime? ValidTill);
}
