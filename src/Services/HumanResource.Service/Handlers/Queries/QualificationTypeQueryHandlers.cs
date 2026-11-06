using MediatR;
using Microsoft.EntityFrameworkCore;
using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class QualificationTypeQueryHandlers :
        IRequestHandler<GetQualificationTypesV3Query, PaginationResponse<QualificationTypeV3Dto>>,
        IRequestHandler<GetQualificationTypeByIdV3Query, QualificationTypeV3Dto?>
    {
        private readonly HumanResourceDbContext _context;
        private const int HumanResourceLovTypeId = 368;

        public QualificationTypeQueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<QualificationTypeV3Dto>> Handle(GetQualificationTypesV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.QualificationTypes
                .Where(x => x.ClientId == request.ClientId && x.LovtypeId == HumanResourceLovTypeId)
                .AsNoTracking();

            if (request.EntityId.HasValue)
            {
                query = query.Where(x => x.EntityId == request.EntityId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(x =>
                    (x.Stxt != null && x.Stxt.Contains(request.SearchTerm)) ||
                    (x.Code != null && x.Code.Contains(request.SearchTerm)));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(x => x.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new QualificationTypeV3Dto(
                    x.Id,
                    x.Code ?? string.Empty,
                    x.Stxt ?? string.Empty,
                    x.Ltxt,
                    !(x.UtBlock ?? false)
                ))
                .ToListAsync(cancellationToken);

            return new PaginationResponse<QualificationTypeV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<QualificationTypeV3Dto?> Handle(GetQualificationTypeByIdV3Query request, CancellationToken cancellationToken)
        {
            return await _context.QualificationTypes
                .Where(x =>
                    x.Id == request.Id &&
                    x.ClientId == request.ClientId &&
                    x.LovtypeId == HumanResourceLovTypeId &&
                    (!request.EntityId.HasValue || x.EntityId == request.EntityId.Value))
                .Select(x => new QualificationTypeV3Dto(
                    x.Id,
                    x.Code ?? string.Empty,
                    x.Stxt ?? string.Empty,
                    x.Ltxt,
                    !(x.UtBlock ?? false)
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
