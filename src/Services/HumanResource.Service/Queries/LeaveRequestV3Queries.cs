using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Queries
{
    public sealed record GetLeaveRequestsV3Query(
        int PageNumber,
        int PageSize,
        int ClientId,
        int? EntityId,
        string? SearchTerm,
        int? StatusId) : IRequest<PaginationResponse<LeaveRequestV3Dto>>;

    public sealed record GetLeaveRequestByIdV3Query(
        int Id,
        int ClientId,
        int? EntityId) : IRequest<LeaveRequestV3Dto?>;
}
