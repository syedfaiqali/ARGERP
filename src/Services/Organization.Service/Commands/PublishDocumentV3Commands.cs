using MediatR;
using Organization.Service.DTOs;

namespace Organization.Service.Commands
{
    public class CreatePublishDocumentV3Command : CreatePublishDocumentV3Request, IRequest<PublishDocumentV3Dto>
    {
        public int ClientId { get; set; }
        public int CreatedBy { get; set; }
    }

    public class UpdatePublishDocumentV3Command : UpdatePublishDocumentV3Request, IRequest<PublishDocumentV3Dto>
    {
        public int ClientId { get; set; }
        public int UpdatedBy { get; set; }
    }

    public class UpdateBulkPublishDocumentV3Command : UpdateBulkPublishDocumentV3Request, IRequest<bool>
    {
        public int ClientId { get; set; }
        public int UpdatedBy { get; set; }
    }

    public class DeletePublishDocumentV3Command : IRequest<bool>
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public int DeletedBy { get; set; }
    }
}
