using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class DeductionTypeV3QueryHandlers :
        IRequestHandler<GetDeductionTypesV3Query, List<DeductionTypeV3Dto>>,
        IRequestHandler<GetDeductionTypeByIdV3Query, DeductionTypeV3Dto?>,
        IRequestHandler<GetDeductionTypeLookupsV3Query, DeductionTypeLookupsV3Dto>
    {
        private readonly HumanResourceDbContext _context;

        public DeductionTypeV3QueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<List<DeductionTypeV3Dto>> Handle(GetDeductionTypesV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.DeductionTypes
                .Where(x => x.ClientId == request.ClientId && !x.UtBlock);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(x =>
                    (x.Code != null && x.Code.Contains(term)) ||
                    (x.Stxt != null && x.Stxt.Contains(term)) ||
                    (x.Ltxt != null && x.Ltxt.Contains(term)));
            }

            var paymentRunTypeMap = await _context.Lovs
                .AsNoTracking()
                .Where(x => x.Vtype == "PRT" && !x.UtBlock)
                .Select(x => new { x.Id, x.Stxt })
                .ToDictionaryAsync(x => x.Id, x => x.Stxt ?? string.Empty, cancellationToken);

            var list = await query
                .OrderByDescending(x => x.Id)
                .ToListAsync(cancellationToken);

            return list.Select(x => MapToDto(x, paymentRunTypeMap)).ToList();
        }

        public async Task<DeductionTypeV3Dto?> Handle(GetDeductionTypeByIdV3Query request, CancellationToken cancellationToken)
        {
            var entity = await _context.DeductionTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.ClientId == request.ClientId && !x.UtBlock, cancellationToken);

            if (entity == null) return null;

            var paymentRunTypeMap = await _context.Lovs
                .AsNoTracking()
                .Where(x => x.Vtype == "PRT" && !x.UtBlock)
                .Select(x => new { x.Id, x.Stxt })
                .ToDictionaryAsync(x => x.Id, x => x.Stxt ?? string.Empty, cancellationToken);

            return MapToDto(entity, paymentRunTypeMap);
        }

        public async Task<DeductionTypeLookupsV3Dto> Handle(GetDeductionTypeLookupsV3Query request, CancellationToken cancellationToken)
        {
            var paymentRunTypes = await _context.Lovs
                .AsNoTracking()
                .Where(x => x.Vtype == "PRT" && !x.UtBlock)
                .OrderBy(x => x.Stxt)
                .Select(x => new DeductionTypeLookupOptionV3Dto(
                    x.Id,
                    x.Code ?? string.Empty,
                    x.Stxt ?? string.Empty))
                .ToListAsync(cancellationToken);

            return new DeductionTypeLookupsV3Dto(paymentRunTypes);
        }

        private static DeductionTypeV3Dto MapToDto(
            Entities.EPDeductiontypeLegacy x,
            IReadOnlyDictionary<int, string> paymentRunTypeMap)
        {
            var paymentRunType = x.PayRunTypeId.HasValue && paymentRunTypeMap.TryGetValue(x.PayRunTypeId.Value, out var name)
                ? name
                : null;

            return new DeductionTypeV3Dto(
                x.Id,
                x.Code ?? string.Empty,
                x.Stxt ?? string.Empty,
                x.Ltxt,
                x.PayRunTypeId,
                paymentRunType,
                x.Reimbursed,
                !x.UtBlock);
        }
    }
}
