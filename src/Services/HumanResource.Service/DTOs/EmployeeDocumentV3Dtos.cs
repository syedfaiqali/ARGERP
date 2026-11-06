namespace HumanResource.Service.DTOs
{
    public sealed record EmployeeDocumentTypeV3Dto(
        int DocumentTypeId,
        string Code,
        string DocumentType,
        string Description,
        int EmployeeDocumentId,
        int AttachmentCount
    );

    public sealed record EmployeeDocumentAttachmentV3Dto(
        int Id,
        string AttachmentNo,
        int EmployeeDocumentId,
        int DocumentTypeId,
        string DocumentType,
        string DocumentName,
        string FileName,
        string UploadPath,
        string ExternalLink
    );

    public sealed class SaveEmployeeDocumentAttachmentV3Request
    {
        public int? Id { get; set; }
        public int EmployeeId { get; set; }
        public int DocumentTypeId { get; set; }
        public string? AttachmentNo { get; set; }
        public string? DocumentName { get; set; }
        public string? UploadPath { get; set; }
        public string? ExternalLink { get; set; }
        public string? FileName { get; set; }
        public string? FileContentBase64 { get; set; }
    }
}
