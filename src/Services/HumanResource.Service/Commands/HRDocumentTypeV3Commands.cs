using HumanResource.Service.DTOs;
using MediatR;

namespace HumanResource.Service.Commands
{
    public record CreateDocumentTypeV3Command(
        string Code,
        string DocumentType,
        string? Description,
        bool IsActive,
        int ClientId,
        int EntityId) : IRequest<DocumentTypeV3Dto>;

    public record UpdateDocumentTypeV3Command(
        int Id,
        string Code,
        string DocumentType,
        string? Description,
        bool IsActive,
        int ClientId,
        int EntityId) : IRequest<DocumentTypeV3Dto>;

    public record DeleteDocumentTypeV3Command(
        int Id,
        int ClientId,
        int EntityId) : IRequest<bool>;
}
