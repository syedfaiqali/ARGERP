using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.DTOs;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Queries;

namespace Organization.Service.Handlers.Queries
{
    public sealed class DonorV3QueryHandlers :
        IRequestHandler<GetDonorsV3Query, PaginationResponse<DonorV3Dto>>,
        IRequestHandler<GetDonorByIdV3Query, DonorV3Dto?>
    {
        private readonly OrganizationDbContext _context;

        public DonorV3QueryHandlers(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<DonorV3Dto>> Handle(GetDonorsV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.OrgDonorLegacy
                .Where(x =>
                    x.clientid == request.ClientId &&
                    x.utblock != true)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(x =>
                    (x.code != null && x.code.Contains(request.SearchTerm)) ||
                    (x.stxt != null && x.stxt.Contains(request.SearchTerm)) ||
                    (x.contactno != null && x.contactno.Contains(request.SearchTerm)));
            }

            var totalItems = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(x => x.stxt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new DonorV3Dto
                {
                    Id = x.Id,
                    Code = x.code,
                    DonorName = x.stxt,
                    Description = x.ltxt,
                    ContactNo = x.contactno
                })
                .ToListAsync(cancellationToken);

            return new PaginationResponse<DonorV3Dto>
            {
                Items = items,
                TotalCount = totalItems,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<DonorV3Dto?> Handle(GetDonorByIdV3Query request, CancellationToken cancellationToken)
        {
            return await _context.OrgDonorLegacy
                .Where(x =>
                    x.clientid == request.ClientId &&
                    x.Id == request.Id &&
                    x.utblock != true)
                .Select(x => new DonorV3Dto
                {
                    Id = x.Id,
                    Code = x.code,
                    DonorName = x.stxt,
                    Description = x.ltxt,
                    ContactNo = x.contactno
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
