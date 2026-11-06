using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Queries
{
    public sealed record GetDonorsV3Query(
        int PageNumber,
        int PageSize,
        string? SearchTerm,
        int ClientId) : IRequest<PaginationResponse<DonorV3Dto>>;

    public sealed record GetDonorByIdV3Query(int Id, int ClientId) : IRequest<DonorV3Dto?>;
}
