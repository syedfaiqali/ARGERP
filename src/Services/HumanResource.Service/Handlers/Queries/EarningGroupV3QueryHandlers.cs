using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class EarningGroupV3QueryHandlers :
        IRequestHandler<GetEarningGroupsV3Query, List<EarningGroupV3Dto>>,
        IRequestHandler<GetEarningGroupByIdV3Query, EarningGroupHeaderV3Dto?>,
        IRequestHandler<GetEarningGroupLookupsV3Query, EarningGroupLookupsV3Dto>
    {
        private readonly HumanResourceDbContext _context;

        public EarningGroupV3QueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<List<EarningGroupV3Dto>> Handle(
            GetEarningGroupsV3Query request,
            CancellationToken cancellationToken)
        {
            var query = _context.EarningGroups
                .AsNoTracking()
                .Where(x => x.clientid == request.ClientId);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(x =>
                    (x.code != null && x.code.Contains(term)) ||
                    (x.stxt != null && x.stxt.Contains(term)));
            }

            return await query
                .OrderBy(x => x.code)
                .Select(x => new EarningGroupV3Dto(
                    x.Id,
                    x.code ?? string.Empty,
                    x.stxt ?? string.Empty,
                    !x.utblock))
                .ToListAsync(cancellationToken);
        }

        public async Task<EarningGroupHeaderV3Dto?> Handle(
            GetEarningGroupByIdV3Query request,
            CancellationToken cancellationToken)
        {
            var header = await _context.EarningGroups
                .AsNoTracking()
                .Include(x => x.details)
                .FirstOrDefaultAsync(x =>
                    x.Id == request.Id &&
                    x.clientid == request.ClientId,
                    cancellationToken);

            if (header == null) return null;

            var earningTypeMap = await _context.EarningTypes
                .AsNoTracking()
                .Where(x => x.ClientId == request.ClientId && !x.UtBlock)
                .Select(x => new { x.Id, x.Stxt })
                .ToDictionaryAsync(x => x.Id, x => x.Stxt ?? string.Empty, cancellationToken);

            var lovMap = await _context.Lovs
                .AsNoTracking()
                .Where(x => !x.UtBlock)
                .Select(x => new { x.Id, x.Stxt })
                .ToDictionaryAsync(x => x.Id, x => x.Stxt ?? string.Empty, cancellationToken);

            string? EarningTypeName(int? id) =>
                id.HasValue && earningTypeMap.TryGetValue(id.Value, out var name) ? name : null;
            string? LovName(int? id) =>
                id.HasValue && lovMap.TryGetValue(id.Value, out var name) ? name : null;

            var details = (header.details ?? [])
                .Where(x => !x.utblock)
                .OrderBy(x => x.lineindex)
                .Select(x => new EarningGroupDetailV3Dto(
                    x.Id,
                    x.lineindex ?? 0,
                    x.earntypeid,
                    EarningTypeName(x.earntypeid),
                    x.ratebasisid,
                    LovName(x.ratebasisid),
                    x.basisearntypeid,
                    EarningTypeName(x.basisearntypeid),
                    x.value,
                    x.payslipindex,
                    x.paysliptxt,
                    x.statusid,
                    LovName(x.statusid),
                    x.absnoteffective,
                    x.levnoteffective ?? false,
                    (x.isotpart ?? 0) > 0
                ))
                .ToList();

            return new EarningGroupHeaderV3Dto(
                header.Id,
                header.code ?? string.Empty,
                header.stxt ?? string.Empty,
                !header.utblock,
                details);
        }

        public async Task<EarningGroupLookupsV3Dto> Handle(
            GetEarningGroupLookupsV3Query request,
            CancellationToken cancellationToken)
        {
            var earningTypes = await _context.EarningTypes
                .AsNoTracking()
                .Where(x => x.ClientId == request.ClientId && !x.UtBlock)
                .OrderBy(x => x.Stxt)
                .Select(x => new EarningGroupLookupOptionV3Dto(
                    x.Id,
                    x.Code ?? string.Empty,
                    x.Stxt ?? string.Empty))
                .ToListAsync(cancellationToken);

            var rateBasis = await _context.Lovs
                .AsNoTracking()
                .Where(x => x.Vtype == "RateB" && !x.UtBlock)
                .OrderBy(x => x.Stxt)
                .Select(x => new EarningGroupLookupOptionV3Dto(
                    x.Id,
                    x.Code ?? string.Empty,
                    x.Stxt ?? string.Empty))
                .ToListAsync(cancellationToken);

            var statuses = await _context.Lovs
                .AsNoTracking()
                .Where(x => x.Vtype == "AACT" && !x.UtBlock)
                .OrderBy(x => x.Stxt)
                .Select(x => new EarningGroupLookupOptionV3Dto(
                    x.Id,
                    x.Code ?? string.Empty,
                    x.Stxt ?? string.Empty))
                .ToListAsync(cancellationToken);

            return new EarningGroupLookupsV3Dto(earningTypes, rateBasis, statuses);
        }
    }
}
