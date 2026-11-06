using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Queries
{
    public record GetLeaveGroupsV3Query(int ClientId, int? EntityId, string? SearchTerm, int PageNumber = 1, int PageSize = 10) : IRequest<PaginationResponse<LeaveGroupV3Dto>>;
    public record GetLeaveGroupByIdV3Query(int Id, int ClientId, int? EntityId = null) : IRequest<LeaveGroupHeaderV3Dto?>;
}
