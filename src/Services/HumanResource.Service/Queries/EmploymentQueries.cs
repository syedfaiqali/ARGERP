using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Queries
{
    public sealed record GetEmploymentsV3Query(
        int PageNumber,
        int PageSize,
        int ClientId,
        int? EntityId,
        int? EmployeeId
    ) : IRequest<PaginationResponse<EmploymentV3Dto>>;

    public sealed record GetEmploymentByIdV3Query(
        int Id,
        int ClientId,
        int? EntityId
    ) : IRequest<EmploymentV3Dto?>;
}
