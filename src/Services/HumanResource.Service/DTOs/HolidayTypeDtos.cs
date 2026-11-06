namespace HumanResource.Service.DTOs
{
    public sealed record HolidayTypeV3Dto(
        int Id,
        string Code,
        string HolidayType,
        string? Description,
        string? ColorCode,
        bool IsActive
    );

    public sealed record CreateHolidayTypeV3Request(
        string Code,
        string HolidayType,
        string? Description,
        string? ColorCode,
        bool IsActive
    );

    public sealed record UpdateHolidayTypeV3Request(
        int Id,
        string Code,
        string HolidayType,
        string? Description,
        string? ColorCode,
        bool IsActive
    );
}
