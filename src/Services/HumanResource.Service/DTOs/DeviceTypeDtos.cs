namespace HumanResource.Service.DTOs
{
    public sealed record DeviceTypeV3Dto(
        int Id,
        string Code,
        string Name,
        string? Description,
        bool IsActive
    );

    public sealed record CreateDeviceTypeV3Request(
        string Code,
        string Name,
        string? Description,
        bool IsActive
    );

    public sealed record UpdateDeviceTypeV3Request(
        int Id,
        string Code,
        string Name,
        string? Description,
        bool IsActive
    );
}
