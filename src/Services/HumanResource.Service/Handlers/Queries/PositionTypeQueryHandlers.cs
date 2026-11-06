using MediatR;
using Microsoft.EntityFrameworkCore;
using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class PositionTypeQueryHandlers :
        IRequestHandler<GetPositionTypesV3Query, PaginationResponse<PositionTypeV3Dto>>,
        IRequestHandler<GetPositionTypeByIdV3Query, PositionTypeV3Dto?>,
        IRequestHandler<GetEmploymentTypesV3LookupQuery, IEnumerable<EmploymentTypeLookupDto>>
    {
        private readonly HumanResourceDbContext _context;

        public PositionTypeQueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<PositionTypeV3Dto>> Handle(GetPositionTypesV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.PositionTypes
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
                .GroupJoin(_context.EmploymentTypes,
                    p => p.EmpTypeId,
                    et => et.Id,
                    (p, etGroup) => new { p, etGroup })
                .SelectMany(
                    x => x.etGroup.DefaultIfEmpty(),
                    (x, et) => new PositionTypeV3Dto(
                        x.p.Id,
                        x.p.Code ?? string.Empty,
                        x.p.EmpTypeId,
                        et != null ? et.Stxt : string.Empty,
                        x.p.Stxt ?? string.Empty,
                        x.p.Ltxt,
                        !(x.p.UtBlock ?? false)
                    ))
                .ToListAsync(cancellationToken);

            return new PaginationResponse<PositionTypeV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<PositionTypeV3Dto?> Handle(GetPositionTypeByIdV3Query request, CancellationToken cancellationToken)
        {
            return await _context.PositionTypes
                .Where(x => x.Id == request.Id && x.ClientId == request.ClientId && (!request.EntityId.HasValue || x.EntityId == request.EntityId.Value))
                .GroupJoin(_context.EmploymentTypes,
                    p => p.EmpTypeId,
                    et => et.Id,
                    (p, etGroup) => new { p, etGroup })
                .SelectMany(
                    x => x.etGroup.DefaultIfEmpty(),
                    (x, et) => new PositionTypeV3Dto(
                        x.p.Id,
                        x.p.Code ?? string.Empty,
                        x.p.EmpTypeId,
                        et != null ? et.Stxt : string.Empty,
                        x.p.Stxt ?? string.Empty,
                        x.p.Ltxt,
                        !(x.p.UtBlock ?? false)
                    ))
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<EmploymentTypeLookupDto>> Handle(GetEmploymentTypesV3LookupQuery request, CancellationToken cancellationToken)
        {
            return await _context.EmploymentTypes
                .Select(x => new EmploymentTypeLookupDto(x.Id, x.Stxt ?? string.Empty))
                .ToListAsync(cancellationToken);
        }
    }
}
