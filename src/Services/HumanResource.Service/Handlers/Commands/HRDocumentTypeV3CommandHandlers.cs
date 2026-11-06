using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Commands
{
    public class CreateDocumentTypeV3CommandHandler : IRequestHandler<CreateDocumentTypeV3Command, DocumentTypeV3Dto>
    {
        private readonly HumanResourceDbContext _context;

        public CreateDocumentTypeV3CommandHandler(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<DocumentTypeV3Dto> Handle(CreateDocumentTypeV3Command request, CancellationToken cancellationToken)
        {
            var codeExists = await _context.DocumentTypes
                .AnyAsync(x => x.Code == request.Code && x.ClientId == request.ClientId, cancellationToken);
            if (codeExists)
                throw new InvalidOperationException("Document Type code already exists.");

            var nameExists = await _context.DocumentTypes
                .AnyAsync(x => x.Stxt == request.DocumentType && x.ClientId == request.ClientId, cancellationToken);
            if (nameExists)
                throw new InvalidOperationException("Document Type name already exists.");

            var nextId = (await _context.DocumentTypes.IgnoreQueryFilters()
                .MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            var entity = new HRDocTypeLegacy
            {
                Id = nextId,
                Code = request.Code?.Trim(),
                Stxt = request.DocumentType?.Trim(),
                Ltxt = request.Description?.Trim(),
                UtBlock = !request.IsActive,
                ClientId = request.ClientId,
                UtSubDocType = 600109 // GenEnum.UTDocSubtype.HRDocumentType
            };

            _context.DocumentTypes.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new DocumentTypeV3Dto(
                entity.Id,
                entity.Code ?? string.Empty,
                entity.Stxt ?? string.Empty,
                entity.Ltxt ?? string.Empty,
                entity.UtBlock != true
            );
        }
    }

    public class UpdateDocumentTypeV3CommandHandler : IRequestHandler<UpdateDocumentTypeV3Command, DocumentTypeV3Dto>
    {
        private readonly HumanResourceDbContext _context;

        public UpdateDocumentTypeV3CommandHandler(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<DocumentTypeV3Dto> Handle(UpdateDocumentTypeV3Command request, CancellationToken cancellationToken)
        {
            var entity = await _context.DocumentTypes
                .Where(x => x.ClientId == request.ClientId && x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException("Document Type not found.");

            var codeExists = await _context.DocumentTypes
                .AnyAsync(x => x.Code == request.Code && x.Id != request.Id && x.ClientId == request.ClientId, cancellationToken);
            if (codeExists)
                throw new InvalidOperationException("Document Type code already exists.");

            var nameExists = await _context.DocumentTypes
                .AnyAsync(x => x.Stxt == request.DocumentType && x.Id != request.Id && x.ClientId == request.ClientId, cancellationToken);
            if (nameExists)
                throw new InvalidOperationException("Document Type name already exists.");

            entity.Code = request.Code?.Trim();
            entity.Stxt = request.DocumentType?.Trim();
            entity.Ltxt = request.Description?.Trim();
            entity.UtBlock = !request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);

            return new DocumentTypeV3Dto(
                entity.Id,
                entity.Code ?? string.Empty,
                entity.Stxt ?? string.Empty,
                entity.Ltxt ?? string.Empty,
                entity.UtBlock != true
            );
        }
    }

    public class DeleteDocumentTypeV3CommandHandler : IRequestHandler<DeleteDocumentTypeV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;

        public DeleteDocumentTypeV3CommandHandler(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteDocumentTypeV3Command request, CancellationToken cancellationToken)
        {
            var entity = await _context.DocumentTypes
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.ClientId == request.ClientId, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException("Document Type not found.");

            _context.DocumentTypes.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
