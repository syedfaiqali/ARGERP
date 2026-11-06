using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Queries
{
    public sealed record GetCustomShiftsV3Query(
        int PageNumber,
        int PageSize,
        int ClientId,
        int? EntityId,
        string? SearchTerm = null) : IRequest<PaginationResponse<CustomShiftV3Dto>>;

    public sealed record GetCustomShiftByIdV3Query(int Id, int ClientId, int? EntityId) : IRequest<CustomShiftHeaderV3Dto?>;

    public sealed record GetCustomShiftsByEntityV3Query(int EntityId, int ClientId) : IRequest<List<CustomShiftV3Dto>>;
}
