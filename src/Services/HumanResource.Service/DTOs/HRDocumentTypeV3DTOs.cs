namespace HumanResource.Service.DTOs
{
    public record DocumentTypeV3Dto(
        int Id,
        string Code,
        string DocumentType,
        string? Description,
        bool IsActive
    );

    public record CreateDocumentTypeV3Request(
        string Code,
        string DocumentType,
        string? Description,
        bool IsActive
    );

    public record UpdateDocumentTypeV3Request(
        int Id,
        string Code,
        string DocumentType,
        string? Description,
        bool IsActive
    );
}
