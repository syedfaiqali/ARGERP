using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class GetRatingModelsV3QueryHandler : IRequestHandler<GetRatingModelsV3Query, PaginationResponse<RatingModelV3Dto>>
    {
        private readonly HumanResourceDbContext _context;

        public GetRatingModelsV3QueryHandler(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<RatingModelV3Dto>> Handle(GetRatingModelsV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.SkillRatings
                .AsNoTracking()
                .Include(x => x.Details)
                .Where(x => x.ClientId == request.ClientId && x.EntityId == request.EntityId);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchTerm = request.SearchTerm.ToLower();
                query = query.Where(x =>
                    (x.Code != null && x.Code.ToLower().Contains(searchTerm)) ||
                    (x.Stxt != null && x.Stxt.ToLower().Contains(searchTerm)));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .OrderBy(x => x.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new RatingModelV3Dto(
                    x.Id,
                    x.Code ?? string.Empty,
                    x.Stxt ?? string.Empty,
                    x.UtBlock != true,
                    x.Details.Select(d => new RatingModelItemV3Dto(
                        d.Id,
                        d.Code ?? string.Empty,
                        d.Key ?? string.Empty,
                        d.Stxt ?? string.Empty,
                        d.Factor ?? 0
                    )).ToList()
                ))
                .ToListAsync(cancellationToken);

            return new PaginationResponse<RatingModelV3Dto>
            {
                TotalCount = totalCount,
                Items = items,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }

    public sealed class GetRatingModelByIdV3QueryHandler : IRequestHandler<GetRatingModelByIdV3Query, RatingModelV3Dto?>
    {
        private readonly HumanResourceDbContext _context;

        public GetRatingModelByIdV3QueryHandler(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<RatingModelV3Dto?> Handle(GetRatingModelByIdV3Query request, CancellationToken cancellationToken)
        {
            var entity = await _context.SkillRatings
                .AsNoTracking()
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.ClientId == request.ClientId && x.EntityId == request.EntityId, cancellationToken);

            if (entity == null) return null;

            return new RatingModelV3Dto(
                entity.Id,
                entity.Code ?? string.Empty,
                entity.Stxt ?? string.Empty,
                entity.UtBlock != true,
                entity.Details.Select(d => new RatingModelItemV3Dto(
                    d.Id,
                    d.Code ?? string.Empty,
                    d.Key ?? string.Empty,
                    d.Stxt ?? string.Empty,
                    d.Factor ?? 0
                )).ToList()
            );
        }
    }
}
