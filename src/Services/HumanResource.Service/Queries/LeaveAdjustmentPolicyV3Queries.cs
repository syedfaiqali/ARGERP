using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Queries
{
    public record GetLeaveAdjustmentPoliciesV3Query(
        int PageNumber,
        int PageSize,
        int ClientId,
        int? EntityId,
        string? SearchTerm) : IRequest<PaginationResponse<LeaveAdjustmentPolicyV3Dto>>;

    public record GetLeaveAdjustmentPolicyByIdV3Query(
        int Id,
        int ClientId,
        int? EntityId) : IRequest<LeaveAdjustmentPolicyV3Dto?>;
}
