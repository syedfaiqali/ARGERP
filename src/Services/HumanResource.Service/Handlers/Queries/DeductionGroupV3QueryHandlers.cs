using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class DeductionGroupV3QueryHandlers :
        IRequestHandler<GetDeductionGroupsV3Query, List<DeductionGroupV3Dto>>,
        IRequestHandler<GetDeductionGroupByIdV3Query, DeductionGroupHeaderV3Dto?>,
        IRequestHandler<GetDeductionGroupLookupsV3Query, DeductionGroupLookupsV3Dto>
    {
        private readonly HumanResourceDbContext _context;

        public DeductionGroupV3QueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<List<DeductionGroupV3Dto>> Handle(
            GetDeductionGroupsV3Query request,
            CancellationToken cancellationToken)
        {
            var query = _context.DeductionGroups
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
                .Select(x => new DeductionGroupV3Dto(
                    x.Id,
                    x.code ?? string.Empty,
                    x.stxt ?? string.Empty,
                    !x.utblock))
                .ToListAsync(cancellationToken);
        }

        public async Task<DeductionGroupHeaderV3Dto?> Handle(
            GetDeductionGroupByIdV3Query request,
            CancellationToken cancellationToken)
        {
            var header = await _context.DeductionGroups
                .AsNoTracking()
                .Include(x => x.details)
                .FirstOrDefaultAsync(x =>
                    x.Id == request.Id &&
                    x.clientid == request.ClientId,
                    cancellationToken);

            if (header == null) return null;

            var deductionTypeMap = await _context.DeductionTypes
                .AsNoTracking()
                .Where(x => x.ClientId == request.ClientId && !x.UtBlock)
                .Select(x => new { x.Id, x.Stxt })
                .ToDictionaryAsync(x => x.Id, x => x.Stxt ?? string.Empty, cancellationToken);

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

            string? DeductionName(int? id) =>
                id.HasValue && deductionTypeMap.TryGetValue(id.Value, out var name) ? name : null;
            string? EarningName(int? id) =>
                id.HasValue && earningTypeMap.TryGetValue(id.Value, out var name) ? name : null;
            string? LovName(int? id) =>
                id.HasValue && lovMap.TryGetValue(id.Value, out var name) ? name : null;

            var details = (header.details ?? [])
                .Where(x => !x.utblock)
                .OrderBy(x => x.lineindex)
                .Select(x => new DeductionGroupDetailV3Dto(
                    x.Id,
                    x.lineindex ?? 0,
                    x.dedtypeid,
                    DeductionName(x.dedtypeid),
                    x.ratebasisid,
                    LovName(x.ratebasisid),
                    x.basisearntypeid,
                    EarningName(x.basisearntypeid),
                    x.value,
                    x.payslipindex,
                    x.paysliptxt,
                    x.statusid,
                    LovName(x.statusid)
                ))
                .ToList();

            return new DeductionGroupHeaderV3Dto(
                header.Id,
                header.code ?? string.Empty,
                header.stxt ?? string.Empty,
                !header.utblock,
                details);
        }

        public async Task<DeductionGroupLookupsV3Dto> Handle(
            GetDeductionGroupLookupsV3Query request,
            CancellationToken cancellationToken)
        {
            var deductionTypes = await _context.DeductionTypes
                .AsNoTracking()
                .Where(x => x.ClientId == request.ClientId && !x.UtBlock)
                .OrderBy(x => x.Stxt)
                .Select(x => new DeductionGroupLookupOptionV3Dto(
                    x.Id,
                    x.Code ?? string.Empty,
                    x.Stxt ?? string.Empty))
                .ToListAsync(cancellationToken);

            var earningTypes = await _context.EarningTypes
                .AsNoTracking()
                .Where(x => x.ClientId == request.ClientId && !x.UtBlock)
                .OrderBy(x => x.Stxt)
                .Select(x => new DeductionGroupLookupOptionV3Dto(
                    x.Id,
                    x.Code ?? string.Empty,
                    x.Stxt ?? string.Empty))
                .ToListAsync(cancellationToken);

            var rateBasis = await _context.Lovs
                .AsNoTracking()
                .Where(x => x.Vtype == "RateB" && !x.UtBlock)
                .OrderBy(x => x.Stxt)
                .Select(x => new DeductionGroupLookupOptionV3Dto(
                    x.Id,
                    x.Code ?? string.Empty,
                    x.Stxt ?? string.Empty))
                .ToListAsync(cancellationToken);

            var statuses = await _context.Lovs
                .AsNoTracking()
                .Where(x => x.Vtype == "AACT" && !x.UtBlock)
                .OrderBy(x => x.Stxt)
                .Select(x => new DeductionGroupLookupOptionV3Dto(
                    x.Id,
                    x.Code ?? string.Empty,
                    x.Stxt ?? string.Empty))
                .ToListAsync(cancellationToken);

            return new DeductionGroupLookupsV3Dto(deductionTypes, earningTypes, rateBasis, statuses);
        }
    }
}
