using MediatR;
using HumanResource.Service.DTOs;

namespace HumanResource.Service.Queries
{
    public sealed record GetHolidayTypesV3Query(
        int PageNumber,
        int PageSize,
        int ClientId,
        int? EntityId,
        string? SearchTerm = null) : IRequest<PaginationResponse<HolidayTypeV3Dto>>;

    public sealed record GetHolidayTypeByIdV3Query(int Id, int ClientId, int? EntityId) : IRequest<HolidayTypeV3Dto?>;
}
