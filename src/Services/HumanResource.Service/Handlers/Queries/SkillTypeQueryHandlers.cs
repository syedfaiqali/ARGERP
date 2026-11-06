using MediatR;
using Microsoft.EntityFrameworkCore;
using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class SkillTypeQueryHandlers :
        IRequestHandler<GetSkillTypesV3Query, PaginationResponse<SkillTypeV3Dto>>,
        IRequestHandler<GetSkillTypeByIdV3Query, SkillTypeV3Dto?>
    {
        private readonly HumanResourceDbContext _context;

        public SkillTypeQueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<SkillTypeV3Dto>> Handle(GetSkillTypesV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.SkillTypes
                .Where(x => x.ClientId == request.ClientId)
                .AsNoTracking();

            if (request.EntityId.HasValue)
            {
                query = query.Where(x => x.EntityId == request.EntityId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(x => (x.Stxt != null && x.Stxt.Contains(request.SearchTerm)) || (x.Code != null && x.Code.Contains(request.SearchTerm)));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(x => x.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new SkillTypeV3Dto(
                    x.Id,
                    x.Code ?? string.Empty,
                    x.Stxt ?? string.Empty,
                    x.Ltxt,
                    !(x.UtBlock ?? false)
                ))
                .ToListAsync(cancellationToken);

            return new PaginationResponse<SkillTypeV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<SkillTypeV3Dto?> Handle(GetSkillTypeByIdV3Query request, CancellationToken cancellationToken)
        {
            return await _context.SkillTypes
                .Where(x => x.Id == request.Id && x.ClientId == request.ClientId && (!request.EntityId.HasValue || x.EntityId == request.EntityId.Value))
                .Select(x => new SkillTypeV3Dto(
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
