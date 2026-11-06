using MediatR;
using Microsoft.EntityFrameworkCore;
using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class QualificationQueryHandlers :
        IRequestHandler<GetQualificationsV3Query, PaginationResponse<QualificationV3Dto>>,
        IRequestHandler<GetQualificationByIdV3Query, QualificationV3Dto?>
    {
        private readonly HumanResourceDbContext _context;

        public QualificationQueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<QualificationV3Dto>> Handle(GetQualificationsV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.Qualifications
                .Where(x => x.clientid == request.ClientId)
                .AsNoTracking();

            if (request.EntityId.HasValue)
            {
                query = query.Where(x => x.entityid == request.EntityId.Value);
            }

            if (request.LovtypeId.HasValue)
            {
                query = query.Where(x => x.lovtypeid == request.LovtypeId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(x => (x.stxt != null && x.stxt.Contains(request.SearchTerm)) || (x.code != null && x.code.Contains(request.SearchTerm)));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(x => x.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .GroupJoin(_context.QualificationTypes,
                    q => q.quatypeid,
                    qt => qt.Id,
                    (q, qtGroup) => new { q, qtGroup })
                .SelectMany(
                    x => x.qtGroup.DefaultIfEmpty(),
                    (x, qt) => new QualificationV3Dto(
                        x.q.Id,
                        x.q.code ?? string.Empty,
                        x.q.quatypeid,
                        qt != null ? qt.Stxt : string.Empty,
                        x.q.stxt ?? string.Empty,
                        x.q.ltxt,
                        x.q.lovtypeid,
                        !(x.q.utblock ?? false)
                    ))
                .ToListAsync(cancellationToken);

            return new PaginationResponse<QualificationV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<QualificationV3Dto?> Handle(GetQualificationByIdV3Query request, CancellationToken cancellationToken)
        {
            return await _context.Qualifications
                .Where(x => x.Id == request.Id && x.clientid == request.ClientId && (!request.EntityId.HasValue || x.entityid == request.EntityId.Value))
                .GroupJoin(_context.QualificationTypes,
                    q => q.quatypeid,
                    qt => qt.Id,
                    (q, qtGroup) => new { q, qtGroup })
                .SelectMany(
                    x => x.qtGroup.DefaultIfEmpty(),
                    (x, qt) => new QualificationV3Dto(
                        x.q.Id,
                        x.q.code ?? string.Empty,
                        x.q.quatypeid,
                        qt != null ? qt.Stxt : string.Empty,
                        x.q.stxt ?? string.Empty,
                        x.q.ltxt,
                        x.q.lovtypeid,
                        !(x.q.utblock ?? false)
                    ))
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
