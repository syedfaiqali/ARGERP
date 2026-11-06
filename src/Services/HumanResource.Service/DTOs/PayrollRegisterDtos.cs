namespace HumanResource.Service.DTOs
{
    public sealed record PayrollRegisterV3Dto(
        int Id,
        string Code,
        string Stxt,
        string? Ltxt
    );

    public sealed record CreatePayrollRegisterV3Request(
        string? Code,
        string Stxt,
        string? Ltxt
    );

    public sealed record UpdatePayrollRegisterV3Request(
        int Id,
        string? Code,
        string Stxt,
        string? Ltxt
    );
}
