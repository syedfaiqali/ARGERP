using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Queries
{
    public class GetPublishDocumentsV3Query : IRequest<PaginationResponse<PublishDocumentV3Dto>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public int? DocumentTypeId { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTill { get; set; }
        public int ClientId { get; set; }
    }

    public class GetPublishDocumentByIdV3Query : IRequest<PublishDocumentV3Dto>
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
    }
}
