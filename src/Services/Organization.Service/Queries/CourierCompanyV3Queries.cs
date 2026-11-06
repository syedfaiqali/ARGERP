using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Queries
{
    public record GetCourierCompaniesV3Query(int PageNumber, int PageSize, string? SearchTerm) : IRequest<PaginationResponse<CourierCompanyV3Dto>>;
    public record GetCourierCompanyByIdV3Query(int Id) : IRequest<CourierCompanyV3Dto?>;
}
