using MediatR;
using HumanResource.Service.DTOs;

namespace HumanResource.Service.Queries
{
    public sealed record GetPositionTypesV3Query(int PageNumber, int PageSize, int ClientId, int? EntityId, string? SearchTerm) : IRequest<PaginationResponse<PositionTypeV3Dto>>;
    public sealed record GetPositionTypeByIdV3Query(int Id, int ClientId, int? EntityId) : IRequest<PositionTypeV3Dto?>;
    public sealed record GetEmploymentTypesV3LookupQuery() : IRequest<IEnumerable<EmploymentTypeLookupDto>>;

    public sealed record EmploymentTypeLookupDto(int Id, string Name);
}
