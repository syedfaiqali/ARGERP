using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class EarningTypeV3QueryHandlers :
        IRequestHandler<GetEarningTypesV3Query, List<EarningTypeV3Dto>>,
        IRequestHandler<GetEarningTypeByIdV3Query, EarningTypeV3Dto?>,
        IRequestHandler<GetEarningTypeLookupsV3Query, EarningTypeLookupsV3Dto>
    {
        private readonly HumanResourceDbContext _context;

        public EarningTypeV3QueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<List<EarningTypeV3Dto>> Handle(GetEarningTypesV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.EarningTypes
                .Where(x => x.ClientId == request.ClientId
                            && !x.UtBlock);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(x =>
                    (x.Code != null && x.Code.Contains(term)) ||
                    (x.Stxt != null && x.Stxt.Contains(term)) ||
                    (x.Ltxt != null && x.Ltxt.Contains(term)));
            }

            var lovMap = await _context.Lovs
                .AsNoTracking()
                .Where(x => !x.UtBlock)
                .Select(x => new { x.Id, x.Stxt })
                .ToDictionaryAsync(x => x.Id, x => x.Stxt ?? string.Empty, cancellationToken);

            var basisMap = await _context.EarningTypes
                .AsNoTracking()
                .Where(x => x.ClientId == request.ClientId && !x.UtBlock)
                .Select(x => new { x.Id, x.Stxt })
                .ToDictionaryAsync(x => x.Id, x => x.Stxt ?? string.Empty, cancellationToken);

            var list = await query
                .OrderByDescending(x => x.Id)
                .ToListAsync(cancellationToken);

            return list.Select(x => MapToDto(x, lovMap, basisMap)).ToList();
        }

        public async Task<EarningTypeV3Dto?> Handle(GetEarningTypeByIdV3Query request, CancellationToken cancellationToken)
        {
            var entity = await _context.EarningTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Id == request.Id
                    && x.ClientId == request.ClientId
                    && !x.UtBlock, cancellationToken);

            if (entity == null) return null;

            var lovMap = await _context.Lovs
                .AsNoTracking()
                .Where(x => !x.UtBlock)
                .Select(x => new { x.Id, x.Stxt })
                .ToDictionaryAsync(x => x.Id, x => x.Stxt ?? string.Empty, cancellationToken);

            var basisMap = await _context.EarningTypes
                .AsNoTracking()
                .Where(x => x.ClientId == request.ClientId && !x.UtBlock)
                .Select(x => new { x.Id, x.Stxt })
                .ToDictionaryAsync(x => x.Id, x => x.Stxt ?? string.Empty, cancellationToken);

            return MapToDto(entity, lovMap, basisMap);
        }

        public async Task<EarningTypeLookupsV3Dto> Handle(GetEarningTypeLookupsV3Query request, CancellationToken cancellationToken)
        {
            async Task<List<EarningTypeLookupOptionV3Dto>> LovByType(string vtype)
            {
                return await _context.Lovs
                    .AsNoTracking()
                    .Where(x => x.Vtype == vtype && !x.UtBlock)
                    .OrderBy(x => x.Stxt)
                    .Select(x => new EarningTypeLookupOptionV3Dto(
                        x.Id,
                        x.Code ?? string.Empty,
                        x.Stxt ?? string.Empty))
                    .ToListAsync(cancellationToken);
            }

            var paymentRunTypes = await LovByType("PRT");
            var taxMethods = await LovByType("PM");
            var rateBasis = await LovByType("RB");
            var uoms = await LovByType("UOM");

            var basisEarningTypes = await _context.EarningTypes
                .AsNoTracking()
                .Where(x => x.ClientId == request.ClientId
                            && !x.UtBlock)
                .OrderBy(x => x.Stxt)
                .Select(x => new EarningTypeLookupOptionV3Dto(
                    x.Id,
                    x.Code ?? string.Empty,
                    x.Stxt ?? string.Empty))
                .ToListAsync(cancellationToken);

            return new EarningTypeLookupsV3Dto(paymentRunTypes, taxMethods, rateBasis, uoms, basisEarningTypes);
        }

        private static EarningTypeV3Dto MapToDto(
            Entities.EPEarningtypeLegacy x,
            IReadOnlyDictionary<int, string> lovMap,
            IReadOnlyDictionary<int, string> basisMap)
        {
            string? GetLovName(int? id) => id.HasValue && lovMap.TryGetValue(id.Value, out var name) ? name : null;
            string? GetBasisName(int? id) => id.HasValue && basisMap.TryGetValue(id.Value, out var name) ? name : null;

            return new EarningTypeV3Dto(
                x.Id,
                x.Code ?? string.Empty,
                x.Stxt ?? string.Empty,
                x.Ltxt,
                x.BsSalary ?? false,
                x.PaySlipIndex,
                x.PaySlipDescription,
                x.UomId,
                GetLovName(x.UomId),
                x.PayRunTypeId,
                GetLovName(x.PayRunTypeId),
                x.TaxMethodId,
                GetLovName(x.TaxMethodId),
                x.RateBasisId,
                GetLovName(x.RateBasisId),
                x.EarningTypeId,
                GetBasisName(x.EarningTypeId),
                !x.UtBlock);
        }
    }
}
