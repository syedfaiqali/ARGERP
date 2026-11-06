using MediatR;
using Microsoft.EntityFrameworkCore;
using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class SkillQueryHandlers :
        IRequestHandler<GetSkillsV3Query, PaginationResponse<SkillV3Dto>>,
        IRequestHandler<GetSkillByIdV3Query, SkillV3Dto?>,
        IRequestHandler<GetSkillRatingsV3LookupQuery, IEnumerable<SkillRatingLookupDto>>
    {
        private readonly HumanResourceDbContext _context;

        public SkillQueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<SkillV3Dto>> Handle(GetSkillsV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.Skills
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
                .Select(s => new SkillV3Dto(
                    s.Id,
                    s.Code ?? string.Empty,
                    s.SkillTypeId,
                    _context.SkillTypes.Where(st => st.Id == s.SkillTypeId).Select(st => st.Stxt).FirstOrDefault(),
                    s.Stxt ?? string.Empty,
                    s.Ltxt,
                    s.SkillRatingId,
                    _context.SkillRatings.Where(rm => rm.Id == s.SkillRatingId).Select(rm => rm.Stxt).FirstOrDefault(),
                    !(s.UtBlock ?? false)
                ))
                .ToListAsync(cancellationToken);

            return new PaginationResponse<SkillV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<SkillV3Dto?> Handle(GetSkillByIdV3Query request, CancellationToken cancellationToken)
        {
            return await _context.Skills
                .Where(x => x.Id == request.Id && x.ClientId == request.ClientId && (!request.EntityId.HasValue || x.EntityId == request.EntityId.Value))
                .Select(s => new SkillV3Dto(
                    s.Id,
                    s.Code ?? string.Empty,
                    s.SkillTypeId,
                    _context.SkillTypes.Where(st => st.Id == s.SkillTypeId).Select(st => st.Stxt).FirstOrDefault(),
                    s.Stxt ?? string.Empty,
                    s.Ltxt,
                    s.SkillRatingId,
                    _context.SkillRatings.Where(rm => rm.Id == s.SkillRatingId).Select(rm => rm.Stxt).FirstOrDefault(),
                    !(s.UtBlock ?? false)
                ))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<SkillRatingLookupDto>> Handle(GetSkillRatingsV3LookupQuery request, CancellationToken cancellationToken)
        {
            return await _context.SkillRatings
                .Where(x => x.ClientId == request.ClientId)
                .Select(x => new SkillRatingLookupDto(x.Id, x.Code ?? string.Empty, x.Stxt ?? string.Empty))
                .ToListAsync(cancellationToken);
        }
    }
}
