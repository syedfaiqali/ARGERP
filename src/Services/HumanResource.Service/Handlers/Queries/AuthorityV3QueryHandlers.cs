using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Queries
{
    public class GetAuthoritiesV3QueryHandler : IRequestHandler<GetAuthoritiesV3Query, PaginationResponse<AuthorityV3Dto>>
    {
        private readonly HumanResourceDbContext _context;

        public GetAuthoritiesV3QueryHandler(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<AuthorityV3Dto>> Handle(GetAuthoritiesV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.EmployeeAuthorities
                .Where(x => x.ClientId == request.ClientId)
                .AsQueryable();

            if (request.EntityId.HasValue)
            {
                query = query.Where(x => x.EntityId == request.EntityId.Value);
            }

            var joinedQuery = from auth in query
                             join lov in _context.Lovs on auth.AuthTypeId equals lov.Id into lovGroup
                             from lov in lovGroup.DefaultIfEmpty()
                             select new { auth, AuthorityTypeName = lov != null ? lov.Stxt : string.Empty };

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var lowerSearch = request.Search.ToLower();
                joinedQuery = joinedQuery.Where(x => 
                    (x.auth.Code != null && x.auth.Code.ToLower().Contains(lowerSearch)) ||
                    (x.auth.Stxt != null && x.auth.Stxt.ToLower().Contains(lowerSearch)) ||
                    (x.AuthorityTypeName != null && x.AuthorityTypeName.ToLower().Contains(lowerSearch))
                );
            }

            var totalCount = await joinedQuery.CountAsync(cancellationToken);

            var items = await joinedQuery
                .OrderByDescending(x => x.auth.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new AuthorityV3Dto(
                    x.auth.Id,
                    x.auth.Code ?? string.Empty,
                    x.auth.AuthTypeId,
                    x.AuthorityTypeName ?? string.Empty,
                    x.auth.Stxt ?? string.Empty,
                    x.auth.Ltxt ?? string.Empty,
                    x.auth.ContactNo ?? string.Empty,
                    x.auth.Phone ?? string.Empty,
                    x.auth.Web ?? string.Empty,
                    x.auth.Fax ?? string.Empty,
                    x.auth.Email ?? string.Empty,
                    x.auth.UtBlock != true
                ))
                .ToListAsync(cancellationToken);

            return new PaginationResponse<AuthorityV3Dto>
            {
                TotalCount = totalCount,
                Items = items
            };
        }
    }

    public class GetAuthorityByIdV3QueryHandler : IRequestHandler<GetAuthorityByIdV3Query, AuthorityV3Dto?>
    {
        private readonly HumanResourceDbContext _context;

        public GetAuthorityByIdV3QueryHandler(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<AuthorityV3Dto?> Handle(GetAuthorityByIdV3Query request, CancellationToken cancellationToken)
        {
            var result = await (from auth in _context.EmployeeAuthorities
                               join lov in _context.Lovs on auth.AuthTypeId equals lov.Id into lovGroup
                               from lov in lovGroup.DefaultIfEmpty()
                               where auth.ClientId == request.ClientId 
                                     && auth.Id == request.Id
                                     && (!request.EntityId.HasValue || auth.EntityId == request.EntityId.Value)
                               select new AuthorityV3Dto(
                                   auth.Id,
                                   auth.Code ?? string.Empty,
                                   auth.AuthTypeId,
                                   lov != null ? lov.Stxt ?? string.Empty : string.Empty,
                                   auth.Stxt ?? string.Empty,
                                   auth.Ltxt ?? string.Empty,
                                   auth.ContactNo ?? string.Empty,
                                   auth.Phone ?? string.Empty,
                                   auth.Web ?? string.Empty,
                                   auth.Fax ?? string.Empty,
                                   auth.Email ?? string.Empty,
                                   auth.UtBlock != true
                               )).FirstOrDefaultAsync(cancellationToken);

            return result;
        }
    }
}
