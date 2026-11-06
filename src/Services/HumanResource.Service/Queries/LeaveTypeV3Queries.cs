using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Queries
{
    public record GetLeaveTypesV3Query(
        int PageNumber,
        int PageSize,
        int ClientId,
        int? EntityId,
        string? SearchTerm) : IRequest<PaginationResponse<LeaveTypeV3Dto>>;

    public record GetLeaveTypeByIdV3Query(
        int Id,
        int ClientId,
        int? EntityId) : IRequest<LeaveTypeV3Dto>;
}
