using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.DTOs;
using Organization.Service.Handlers.Commands;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Queries;

namespace Organization.Service.Handlers.Queries
{
    public sealed class GetCitiesV3QueryHandler : IRequestHandler<GetCitiesV3Query, PaginationResponse<CityV3Dto>>
    {
        private readonly OrganizationDbContext _context;

        public GetCitiesV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<CityV3Dto>> Handle(GetCitiesV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.City
                .AsNoTracking()
                .Where(c => !c.UtBlock)
                .Include(c => c.State)
                .ThenInclude(s => s.Country)
                .AsQueryable();

            if (request.CountryId.HasValue)
                query = query.Where(c => c.CountryId == request.CountryId.Value);

            if (request.StateId.HasValue)
                query = query.Where(c => c.StateId == request.StateId.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.Trim().ToLower();
                query = query.Where(c =>
                    c.Citystxt.ToLower().Contains(search)
                    || c.CityCode.ToLower().Contains(search)
                    || (c.State != null && c.State.stxt.ToLower().Contains(search))
                    || (c.State != null && c.State.Country != null && c.State.Country.CountryName.ToLower().Contains(search)));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(c => c.Citystxt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new CityV3Dto
                {
                    CityId = c.Id,
                    CityCode = c.CityCode,
                    CityName = c.Citystxt,
                    DialCode = c.DialCode,
                    StateId = c.StateId,
                    StateName = c.State != null ? c.State.stxt : string.Empty,
                    CountryId = c.CountryId,
                    CountryName = c.State != null && c.State.Country != null ? c.State.Country.CountryName : string.Empty,
                    IsActive = !c.UtBlock,
                })
                .ToListAsync(cancellationToken);

            return new PaginationResponse<CityV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            };
        }
    }

    public sealed class GetCityByIdV3QueryHandler : IRequestHandler<GetCityByIdV3Query, CityV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public GetCityByIdV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<CityV3Dto> Handle(GetCityByIdV3Query request, CancellationToken cancellationToken)
        {
            var city = await _context.City
                .AsNoTracking()
                .Include(c => c.State)
                .ThenInclude(s => s.Country)
                .FirstOrDefaultAsync(c => c.Id == request.CityId && !c.UtBlock, cancellationToken);

            if (city == null)
                throw new KeyNotFoundException($"City with ID {request.CityId} not found");

            return new CityV3Dto
            {
                CityId = city.Id,
                CityCode = city.CityCode,
                CityName = city.Citystxt,
                DialCode = city.DialCode,
                StateId = city.StateId,
                StateName = city.State != null ? city.State.stxt : string.Empty,
                CountryId = city.CountryId,
                CountryName = city.State != null && city.State.Country != null ? city.State.Country.CountryName : string.Empty,
                IsActive = !city.UtBlock,
            };
        }
    }
}
