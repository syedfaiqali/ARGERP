using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.DTOs;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Queries;

namespace Organization.Service.Handlers.Queries
{
    public sealed class DepartmentV3QueryHandlers :
        IRequestHandler<GetDepartmentsV3Query, PaginationResponse<DepartmentV3Dto>>,
        IRequestHandler<GetDepartmentByIdV3Query, DepartmentV3Dto?>,
        IRequestHandler<GetFicoasV3Query, IEnumerable<FicoaV3Dto>>
    {
        private readonly OrganizationDbContext _context;

        public DepartmentV3QueryHandlers(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<DepartmentV3Dto>> Handle(GetDepartmentsV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.OrgDepartmentLegacy
                .Where(x => x.clientid == request.ClientId)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(x => 
                    (x.depstxt != null && x.depstxt.Contains(request.SearchTerm)) ||
                    (x.depcode != null && x.depcode.Contains(request.SearchTerm)));
            }

            var totalItems = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(x => x.depstxt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new DepartmentV3Dto
                {
                    Id = x.Id,
                    Code = x.depcode,
                    DepartmentName = x.depstxt,
                    Description = x.depltxt,
                    ChartOfAccountId = x.ficoaid,
                    ChartOfAccount = x.Ficoa != null ? x.Ficoa.Title : null,
                    ReceivingChartOfAccountId = x.ficoaid_rec,
                    ReceivingChartOfAccount = x.FicoaRec != null ? x.FicoaRec.Title : null
                })
                .ToListAsync(cancellationToken);

            return new PaginationResponse<DepartmentV3Dto>
            {
                Items = items,
                TotalCount = totalItems,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<DepartmentV3Dto?> Handle(GetDepartmentByIdV3Query request, CancellationToken cancellationToken)
        {
            return await _context.OrgDepartmentLegacy
                .Where(x => x.clientid == request.ClientId && x.Id == request.Id)
                .Select(x => new DepartmentV3Dto
                {
                    Id = x.Id,
                    Code = x.depcode,
                    DepartmentName = x.depstxt,
                    Description = x.depltxt,
                    ChartOfAccountId = x.ficoaid,
                    ChartOfAccount = x.Ficoa != null ? x.Ficoa.Title : null,
                    ReceivingChartOfAccountId = x.ficoaid_rec,
                    ReceivingChartOfAccount = x.FicoaRec != null ? x.FicoaRec.Title : null
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<FicoaV3Dto>> Handle(GetFicoasV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.FicoaLegacy.Where(x => x.Subsidiary == true)
                .Where(x => x.clientid == request.ClientId && x.Active == true)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(x => x.Title != null && x.Title.Contains(request.SearchTerm));
            }

            return await query
                .OrderBy(x => x.Title)
                .Take(100) // Limit for dropdowns
                .Select(x => new FicoaV3Dto
                {
                    Id = x.Id,
                    Title = x.Title,
                    Code = x.AccountCode
                })
                .ToListAsync(cancellationToken);
        }
    }
}
