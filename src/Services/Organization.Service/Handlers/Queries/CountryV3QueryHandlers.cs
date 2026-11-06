using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.DTOs;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Mappers;
using Organization.Service.Queries;
using EDXS.Shared.Common.Regional;

namespace Organization.Service.Handlers.Queries
{
    public class GetCountriesV3QueryHandler : IRequestHandler<GetCountriesV3Query, PaginationResponse<CountryV3Dto>>
    {
        private readonly OrganizationDbContext _context;

        public GetCountriesV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<CountryV3Dto>> Handle(GetCountriesV3Query request, CancellationToken cancellationToken)
        {
            // Read-only query: skip EF change tracking.
            var source=_context.Country.AsQueryable();
            if(!request.IncludeInactive && _context.Database.IsRelational() &&
                await _context.Database.SqlQueryRaw<int>("SELECT CASE WHEN OBJECT_ID('dbo.SMRegionalCountry') IS NULL THEN 0 ELSE 1 END AS Value").SingleAsync(cancellationToken)==1)
                source=_context.Country.FromSqlRaw("SELECT c.* FROM dbo.Country c WHERE EXISTS(SELECT 1 FROM dbo.SMRegionalCountry r WHERE r.countryid=c.Id AND r.utblock=0) OR NOT EXISTS(SELECT 1 FROM dbo.SMRegionalCountry)");
            var query = source.AsNoTracking().Where(c => !c.UtBlock);

            //if (!request.IncludeInactive)
            //    query = query.Where(c => c.IsActive);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.Trim().ToLower();
                query = query.Where(c => c.CountryName.ToLower().Contains(search)
                    || c.CountryCode.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(c => c.CountryName)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new CountryV3Dto
                {
                    CountryId = c.Id,
                    CountryCode = c.CountryCode,
                    CountryName = c.CountryName,
                    IsoCode = c.CountryCode3 ?? string.Empty,
                    DialCode = c.DialCode ?? string.Empty,
                    //CurrencyCode = c.CurrencyCode,
                    //CurrencyName = c.CurrencyName,
                    //CurrencySymbol = c.CurrencySymbol,
                    //IsActive = c.IsActive
                })
                .ToListAsync(cancellationToken);

            var catalog = _context.Database.IsRelational() ? await RegionalReferenceCatalog.Countries(_context.Database.GetDbConnection(),cancellationToken) : new Dictionary<int,RegionalCurrencyReference>();
            foreach(var item in items)
                if(catalog.TryGetValue(item.CountryId,out var currency))
                {
                    item.CurrencyCode=currency.Code; item.CurrencyName=currency.Name; item.CurrencySymbol=currency.Symbol;
                }
            return new PaginationResponse<CountryV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }
    }

    public class GetCountryByIdV3QueryHandler : IRequestHandler<GetCountryByIdV3Query, CountryV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public GetCountryByIdV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<CountryV3Dto> Handle(GetCountryByIdV3Query request, CancellationToken cancellationToken)
        {
            var country = await _context.Country
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == request.CountryId && !c.UtBlock, cancellationToken);

            if (country == null)
                throw new KeyNotFoundException($"Country with ID {request.CountryId} not found");

            var item=CountryV3Mapper.ToDto(country);
            var catalog = _context.Database.IsRelational() ? await RegionalReferenceCatalog.Countries(_context.Database.GetDbConnection(),cancellationToken) : new Dictionary<int,RegionalCurrencyReference>();
            if(catalog.TryGetValue(item.CountryId,out var currency))
            { item.CurrencyCode=currency.Code; item.CurrencyName=currency.Name; item.CurrencySymbol=currency.Symbol; }
            return item;
        }
    }
}
