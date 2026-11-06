using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class PayrollRegisterV3QueryHandlers :
        IRequestHandler<GetPayrollRegistersV3Query, List<PayrollRegisterV3Dto>>,
        IRequestHandler<GetPayrollRegisterByIdV3Query, PayrollRegisterV3Dto?>
    {
        private readonly HumanResourceDbContext _context;

        public PayrollRegisterV3QueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<List<PayrollRegisterV3Dto>> Handle(GetPayrollRegistersV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.PayrollRegisterLegacy
                .Where(x => x.ClientId == request.ClientId && !x.Utblock);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(x =>
                    (x.Code != null && x.Code.Contains(term)) ||
                    (x.Stxt != null && x.Stxt.Contains(term)) ||
                    (x.Ltxt != null && x.Ltxt.Contains(term)));
            }

            return await query
                .OrderBy(x => x.Id)
                .Select(x => new PayrollRegisterV3Dto(
                    x.Id,
                    x.Code ?? string.Empty,
                    x.Stxt ?? string.Empty,
                    x.Ltxt))
                .ToListAsync(cancellationToken);
        }

        public async Task<PayrollRegisterV3Dto?> Handle(GetPayrollRegisterByIdV3Query request, CancellationToken cancellationToken)
        {
            var entity = await _context.PayrollRegisterLegacy
                .Where(x => x.Id == request.Id && x.ClientId == request.ClientId && !x.Utblock)
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                return null;
            }

            return new PayrollRegisterV3Dto(
                entity.Id,
                entity.Code ?? string.Empty,
                entity.Stxt ?? string.Empty,
                entity.Ltxt);
        }
    }
}
