using MediatR;
using HumanResource.Service.DTOs;

namespace HumanResource.Service.Queries
{
    public sealed record GetShiftsV3Query(
        int PageNumber,
        int PageSize,
        int ClientId,
        int? EntityId,
        string? SearchTerm = null,
        int? EmployeeId = null) : IRequest<PaginationResponse<ShiftV3Dto>>;

    public sealed record GetShiftByIdV3Query(int Id, int ClientId, int? EntityId, int? EmployeeId = null) : IRequest<ShiftHeaderV3Dto?>;
}
