using HumanResource.Service.DTOs;
using MediatR;
using System.Collections.Generic;

namespace HumanResource.Service.Queries
{
    public record GetLeaveTransactionListV3Query(int? EntityId, int? ClientId) : IRequest<List<LeaveTransactionV3Dto>>;

    public record GetScopedLeaveTransactionListV3Query(
        int? EntityId,
        int? ClientId,
        int? UserId,
        int? ReferenceEmployeeId,
        bool IncludeAttachments = true) : IRequest<LeaveTransactionScopeV3Dto>;

    public record GetLeaveTransactionByIdV3Query(int Id) : IRequest<LeaveTransactionV3Dto?>;
}
