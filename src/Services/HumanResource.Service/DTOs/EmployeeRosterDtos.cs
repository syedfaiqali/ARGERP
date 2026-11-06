namespace HumanResource.Service.DTOs
{
    public sealed record EmployeeRosterV3Dto(
        int Id,
        int? EmployeeId,
        int? ShiftId,
        string? ShiftName,
        int? LeaveGroupId,
        string? LeaveGroupName,
        bool LateAllowed,
        bool EarlyDeparture,
        bool OvertimeAllowed,
        DateTime? StartDate,
        DateTime? EndDate,
        int? TaxId,
        string? TaxName,
        bool TaxExempt
    );

    public sealed record UpsertEmployeeRosterV3Request(
        int? Id,
        int EmployeeId,
        int? ShiftId,
        int? LeaveGroupId,
        bool LateAllowed,
        bool EarlyDeparture,
        bool OvertimeAllowed,
        DateTime? StartDate,
        DateTime? EndDate,
        int? TaxId,
        bool TaxExempt
    );
}
