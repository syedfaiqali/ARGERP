using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Queries
{
    public class GetDocumentTypesV3QueryHandler : IRequestHandler<GetDocumentTypesV3Query, PaginationResponse<DocumentTypeV3Dto>>
    {
        private readonly HumanResourceDbContext _context;

        public GetDocumentTypesV3QueryHandler(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<DocumentTypeV3Dto>> Handle(GetDocumentTypesV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.DocumentTypes
                .Where(x => x.ClientId == request.ClientId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var lowerSearch = request.Search.ToLower();
                query = query.Where(x => 
                    (x.Code != null && x.Code.ToLower().Contains(lowerSearch)) ||
                    (x.Stxt != null && x.Stxt.ToLower().Contains(lowerSearch))
                );
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderByDescending(x => x.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new DocumentTypeV3Dto(
                    x.Id,
                    x.Code ?? string.Empty,
                    x.Stxt ?? string.Empty,
                    x.Ltxt ?? string.Empty,
                    x.UtBlock != true
                ))
                .ToListAsync(cancellationToken);

            return new PaginationResponse<DocumentTypeV3Dto>
            {
                TotalCount = totalCount,
                Items = items
            };
        }
    }

    public class GetDocumentTypeByIdV3QueryHandler : IRequestHandler<GetDocumentTypeByIdV3Query, DocumentTypeV3Dto?>
    {
        private readonly HumanResourceDbContext _context;

        public GetDocumentTypeByIdV3QueryHandler(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<DocumentTypeV3Dto?> Handle(GetDocumentTypeByIdV3Query request, CancellationToken cancellationToken)
        {
            var entity = await _context.DocumentTypes
                .Where(x => x.ClientId == request.ClientId && x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                return null;
            }

            return new DocumentTypeV3Dto(
                entity.Id,
                entity.Code ?? string.Empty,
                entity.Stxt ?? string.Empty,
                entity.Ltxt ?? string.Empty,
                entity.UtBlock != true
            );
        }
    }
}
