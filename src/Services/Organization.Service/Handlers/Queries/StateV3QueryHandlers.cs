using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.DTOs;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Queries;

namespace Organization.Service.Handlers.Queries
{
    public sealed class GetStatesV3QueryHandler : IRequestHandler<GetStatesV3Query, PaginationResponse<StateV3Dto>>
    {
        private readonly OrganizationDbContext _context;

        public GetStatesV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<StateV3Dto>> Handle(GetStatesV3Query request, CancellationToken cancellationToken)
        {
            var source = _context.State.AsQueryable();
            if (!request.IncludeInactive && _context.Database.IsRelational() &&
                await _context.Database.SqlQueryRaw<int>("SELECT CASE WHEN OBJECT_ID('dbo.SMRegionalLocation') IS NULL THEN 0 ELSE 1 END AS Value").SingleAsync(cancellationToken) == 1)
                source = _context.State.FromSqlRaw("SELECT s.* FROM dbo.State s WHERE EXISTS(SELECT 1 FROM dbo.SMRegionalLocation r WHERE r.kind='state' AND r.localid=s.Id) OR NOT EXISTS(SELECT 1 FROM dbo.State other JOIN dbo.SMRegionalLocation r ON r.kind='state' AND r.localid=other.Id WHERE other.countryid=s.countryid AND other.stxt=s.stxt)");
            var query = source
                .AsNoTracking()
                .Include(s => s.Country)
                .AsQueryable();

            if (request.CountryId.HasValue)
                query = query.Where(s => s.CountryId == request.CountryId.Value);

            //if (!request.IncludeInactive)
            //    query = query.Where(s => s.IsActive);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.Trim().ToLower();
                query = query.Where(s =>
                    s.stxt.ToLower().Contains(search)
                    || s.StateCode.ToLower().Contains(search)
                    || (s.Country != null && s.Country.CountryName.ToLower().Contains(search)));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(s => s.stxt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => new StateV3Dto
                {
                    StateId = s.Id,
                    CountryId = s.CountryId,
                    StateName = s.stxt,
                    StateCode = s.StateCode,
                    CountryName = s.Country.CountryName,
                    //IsActive = s.IsActive,
                })
                .ToListAsync(cancellationToken);

            return new PaginationResponse<StateV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            };
        }
    }

    public sealed class GetStateByIdV3QueryHandler : IRequestHandler<GetStateByIdV3Query, StateV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public GetStateByIdV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<StateV3Dto> Handle(GetStateByIdV3Query request, CancellationToken cancellationToken)
        {
            var state = await _context.State
                .AsNoTracking()
                .Include(s => s.Country)
                .FirstOrDefaultAsync(s => s.Id == request.StateId, cancellationToken);

            if (state == null)
                throw new KeyNotFoundException($"State with ID {request.StateId} not found");

            return new StateV3Dto
            {
                StateId = state.Id,
                CountryId = state.CountryId,
                StateName = state.stxt,
                StateCode = state.StateCode,
                CountryName = state.Country?.CountryName,
                //IsActive = state.IsActive,
            };
        }
    }
}
