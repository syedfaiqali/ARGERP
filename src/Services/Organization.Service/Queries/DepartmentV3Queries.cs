using MediatR;
using Organization.Service.DTOs;


namespace Organization.Service.Queries
{
    public sealed record GetDepartmentsV3Query(
        int PageNumber,
        int PageSize,
        string? SearchTerm,
        int ClientId) : IRequest<PaginationResponse<DepartmentV3Dto>>;

    public sealed record GetDepartmentByIdV3Query(int Id, int ClientId) : IRequest<DepartmentV3Dto?>;

    public sealed record GetFicoasV3Query(string? SearchTerm, int ClientId) : IRequest<IEnumerable<FicoaV3Dto>>;
}
