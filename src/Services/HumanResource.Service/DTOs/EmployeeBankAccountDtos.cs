namespace HumanResource.Service.DTOs
{
    public sealed record EmployeeBankAccountV3Dto(
        int Id,
        string? Code,
        int? EmployeeId,
        int? BankGroupId,
        string? BankGroupName,
        string? BranchCode,
        string? BranchName,
        string? AccountNo,
        string? IbanNo,
        string? Title,
        bool PrimaryAccount
    );

    public sealed record UpsertEmployeeBankAccountV3Request(
        int? Id,
        string? Code,
        int EmployeeId,
        int? BankGroupId,
        string? BranchCode,
        string? BranchName,
        string? AccountNo,
        string? IbanNo,
        string? Title,
        bool PrimaryAccount
    );
}
