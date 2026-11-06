using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Queries
{
    public record GetDocumentTypesV3Query(
        int ClientId,
        int EntityId,
        int PageNumber = 1,
        int PageSize = 10,
        string? Search = null) : IRequest<PaginationResponse<DocumentTypeV3Dto>>;

    public record GetDocumentTypeByIdV3Query(
        int ClientId,
        int EntityId,
        int Id) : IRequest<DocumentTypeV3Dto?>;
}
