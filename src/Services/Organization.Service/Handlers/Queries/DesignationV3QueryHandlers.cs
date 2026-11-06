using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.DTOs;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Queries;

namespace Organization.Service.Handlers.Queries
{
    public sealed class DesignationV3QueryHandlers :
        IRequestHandler<GetDesignationsV3Query, PaginationResponse<DesignationV3Dto>>,
        IRequestHandler<GetDesignationByIdV3Query, DesignationV3Dto?>
    {
        private readonly OrganizationDbContext _context;

        public DesignationV3QueryHandlers(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<DesignationV3Dto>> Handle(GetDesignationsV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.OrgDesignationLegacy
                .Where(x => x.clientid == request.ClientId)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(x =>
                    (x.desgcode != null && x.desgcode.Contains(request.SearchTerm)) ||
                    (x.stxt != null && x.stxt.Contains(request.SearchTerm)));
            }

            var totalItems = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(x => x.stxt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new DesignationV3Dto
                {
                    Id = x.Id,
                    Code = x.desgcode,
                    DesignationName = x.stxt,
                    Description = x.ltxt,
                    Inactive = x.blank == true
                })
                .ToListAsync(cancellationToken);

            return new PaginationResponse<DesignationV3Dto>
            {
                Items = items,
                TotalCount = totalItems,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<DesignationV3Dto?> Handle(GetDesignationByIdV3Query request, CancellationToken cancellationToken)
        {
            return await _context.OrgDesignationLegacy
                .Where(x => x.clientid == request.ClientId && x.Id == request.Id)
                .Select(x => new DesignationV3Dto
                {
                    Id = x.Id,
                    Code = x.desgcode,
                    DesignationName = x.stxt,
                    Description = x.ltxt,
                    Inactive = x.blank == true
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
