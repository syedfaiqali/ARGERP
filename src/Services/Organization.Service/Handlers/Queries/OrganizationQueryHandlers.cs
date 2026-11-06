using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.DTOs;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Queries;

namespace Organization.Service.Handlers.Queries
{
    // ==================== Country Query Handlers ====================
    public class GetAllCountriesQueryHandler : IRequestHandler<GetAllCountriesQuery, PaginationResponse<CountryDto>>
    {
        private readonly OrganizationDbContext _context;

        public GetAllCountriesQueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<CountryDto>> Handle(GetAllCountriesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Country.AsQueryable();

            //if (!request.IncludeInactive)
            //    query = query.Where(c => c.IsActive);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.ToLower();
                query = query.Where(c => c.CountryName.ToLower().Contains(search) || 
                                         c.CountryCode.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(c => c.CountryName)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => MapToDto(c))
                .ToListAsync(cancellationToken);

            return new PaginationResponse<CountryDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        private CountryDto MapToDto(Organization.Service.Entities.Country country)
        {
            return new CountryDto
            {
                CountryId = country.Id,
                CountryName = country.CountryName,
                CountryCode = country.CountryCode,
                //CountryCode3 = country.CountryCode3,
                //PhoneCode = country.PhoneCode,
                //CurrencyCode = country.CurrencyCode,
                //CurrencyName = country.CurrencyName,
                //CurrencySymbol = country.CurrencySymbol,
                //IsActive = country.IsActive
            };
        }
    }

    public class GetCountryByIdQueryHandler : IRequestHandler<GetCountryByIdQuery, CountryDto>
    {
        private readonly OrganizationDbContext _context;

        public GetCountryByIdQueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<CountryDto> Handle(GetCountryByIdQuery request, CancellationToken cancellationToken)
        {
            var country = await _context.Country
                .FirstOrDefaultAsync(c => c.Id == request.CountryId, cancellationToken);

            if (country == null)
                throw new KeyNotFoundException($"Country with ID {request.CountryId} not found");

            return MapToDto(country);
        }

        private CountryDto MapToDto(Organization.Service.Entities.Country country)
        {
            return new CountryDto
            {
                CountryId = country.Id,
                CountryName = country.CountryName,
                CountryCode = country.CountryCode,
                //CountryCode3 = country.CountryCode3,
                //PhoneCode = country.PhoneCode,
                //CurrencyCode = country.CurrencyCode,
                //CurrencyName = country.CurrencyName,
                //CurrencySymbol = country.CurrencySymbol,
                //IsActive = country.IsActive
            };
        }
    }

    // ==================== State Query Handlers ====================
    public class GetStatesByCountryQueryHandler : IRequestHandler<GetStatesByCountryQuery, PaginationResponse<StateDto>>
    {
        private readonly OrganizationDbContext _context;

        public GetStatesByCountryQueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<StateDto>> Handle(GetStatesByCountryQuery request, CancellationToken cancellationToken)
        {
            // Verify country exists
            var countryExists = await _context.Country.AnyAsync(c => c.Id == request.CountryId, cancellationToken);
            if (!countryExists)
                throw new KeyNotFoundException($"Country with ID {request.CountryId} not found");

            var query = _context.State.Where(s => s.CountryId == request.CountryId);

            //if (!request.IncludeInactive)
            //    query = query.Where(s => s.IsActive);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Include(s => s.Country)
                .OrderBy(s => s.stxt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(s => MapToDto(s, s.Country))
                .ToListAsync(cancellationToken);

            return new PaginationResponse<StateDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        private StateDto MapToDto(Organization.Service.Entities.State state, Organization.Service.Entities.Country country)
        {
            return new StateDto
            {
                StateId = state.Id,
                CountryId = state.CountryId,
                StateName = state.stxt,
                StateCode = state.StateCode,
                CountryName = country.CountryName,
                //IsActive = state.IsActive
            };
        }
    }

    // ==================== City Query Handlers ====================
    public class GetCitiesByStateQueryHandler : IRequestHandler<GetCitiesByStateQuery, PaginationResponse<CityDto>>
    {
        private readonly OrganizationDbContext _context;

        public GetCitiesByStateQueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<CityDto>> Handle(GetCitiesByStateQuery request, CancellationToken cancellationToken)
        {
            var query = _context.City.Where(c => c.StateId == request.StateId);

            //if (!request.IncludeInactive)
            //    query = query.Where(c => c.IsActive);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Include(c => c.State).ThenInclude(s => s.Country)
                .OrderBy(c => c.Citystxt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => MapToDto(c))
                .ToListAsync(cancellationToken);

            return new PaginationResponse<CityDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        private CityDto MapToDto(Organization.Service.Entities.City city)
        {
            return new CityDto
            {
                CityId = city.Id,
                StateId = city.StateId,
                CountryId = city.CountryId,
                CityName = city.Citystxt,
                CityCode = city.CityCode,
                StateName = city.State?.stxt,
                CountryName = city.State?.Country?.CountryName,
                //IsActive = city.IsActive
            };
        }
    }

    public class GetCityByIdQueryHandler : IRequestHandler<GetCityByIdQuery, CityDto>
    {
        private readonly OrganizationDbContext _context;

        public GetCityByIdQueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<CityDto> Handle(GetCityByIdQuery request, CancellationToken cancellationToken)
        {
            var city = await _context.City
                .Include(c => c.State).ThenInclude(s => s.Country)
                .FirstOrDefaultAsync(c => c.Id == request.CityId, cancellationToken);

            if (city == null)
                throw new KeyNotFoundException($"City with ID {request.CityId} not found");

            return MapToDto(city);
        }

        private CityDto MapToDto(Organization.Service.Entities.City city)
        {
            return new CityDto
            {
                CityId = city.Id,
                StateId = city.StateId,
                CountryId = city.CountryId,
                CityName = city.Citystxt,
                CityCode = city.CityCode,
                StateName = city.State?.stxt,
                CountryName = city.State?.Country?.CountryName,
                //IsActive = city.IsActive
            };
        }
    }

    // ==================== OrgCampus Query Handlers ====================
    public class GetAllOrgCampusesQueryHandler : IRequestHandler<GetAllOrgCampusesQuery, PaginationResponse<OrgCampusDto>>
    {
        private readonly OrganizationDbContext _context;

        public GetAllOrgCampusesQueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<OrgCampusDto>> Handle(GetAllOrgCampusesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.OrgCampuses.Where(c => !c.IsDeleted);

            if (request.ClientId.HasValue)
                query = query.Where(c => c.ClientId == request.ClientId);

            if (!request.IncludeInactive)
                query = query.Where(c => c.IsActive);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.ToLower();
                query = query.Where(c => c.CampusName.ToLower().Contains(search) || 
                                         c.CampusCode.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Include(c => c.Entity)
                .Include(c => c.City)
                .OrderBy(c => c.CampusName)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => MapToDto(c))
                .ToListAsync(cancellationToken);

            return new PaginationResponse<OrgCampusDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        private OrgCampusDto MapToDto(Organization.Service.Entities.OrgCampus campus)
        {
            return new OrgCampusDto
            {
                CampusId = campus.CampusId,
                EntityId = campus.EntityId,
                ClientId = campus.ClientId,
                EntityName = campus.Entity?.EntityName,
                CampusCode = campus.CampusCode,
                CampusName = campus.CampusName,
                CampusType = campus.CampusType,
                Address = campus.Address,
                CityName = campus.City?.Citystxt,
                CountryName = campus.Country?.CountryName,
                PhoneNumber = campus.PhoneNumber,
                EmailAddress = campus.EmailAddress,
                PrincipalName = campus.PrincipalName,
                PrincipalEmail = campus.PrincipalEmail,
                EstablishedDate = campus.EstablishedDate,
                IsActive = campus.IsActive
            };
        }
    }

    public class GetOrgCampusByIdQueryHandler : IRequestHandler<GetOrgCampusByIdQuery, OrgCampusDto>
    {
        private readonly OrganizationDbContext _context;

        public GetOrgCampusByIdQueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<OrgCampusDto> Handle(GetOrgCampusByIdQuery request, CancellationToken cancellationToken)
        {
            var campus = await _context.OrgCampuses
                .Include(c => c.Entity)
                .Include(c => c.Country)
                .Include(c => c.State)
                .Include(c => c.City)
                .FirstOrDefaultAsync(c => c.CampusId == request.CampusId && !c.IsDeleted, cancellationToken);

            if (campus == null)
                throw new KeyNotFoundException($"Campus with ID {request.CampusId} not found");

            return MapToDto(campus);
        }

        private OrgCampusDto MapToDto(Organization.Service.Entities.OrgCampus campus)
        {
            return new OrgCampusDto
            {
                CampusId = campus.CampusId,
                EntityId = campus.EntityId,
                ClientId = campus.ClientId,
                EntityName = campus.Entity?.EntityName,
                CampusCode = campus.CampusCode,
                CampusName = campus.CampusName,
                CampusType = campus.CampusType,
                Address = campus.Address,
                CityName = campus.City?.Citystxt,
                CountryName = campus.Country?.CountryName,
                PhoneNumber = campus.PhoneNumber,
                EmailAddress = campus.EmailAddress,
                PrincipalName = campus.PrincipalName,
                PrincipalEmail = campus.PrincipalEmail,
                EstablishedDate = campus.EstablishedDate,
                IsActive = campus.IsActive
            };
        }
    }

    // ==================== OrgDepartment Query Handlers ====================
    public class GetAllOrgDepartmentsQueryHandler : IRequestHandler<GetAllOrgDepartmentsQuery, PaginationResponse<OrgDepartmentDto>>
    {
        private readonly OrganizationDbContext _context;

        public GetAllOrgDepartmentsQueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<OrgDepartmentDto>> Handle(GetAllOrgDepartmentsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.OrgDepartments.Where(d => !d.IsDeleted);

            if (request.ClientId.HasValue)
                query = query.Where(d => d.ClientId == request.ClientId);

            if (!request.IncludeInactive)
                query = query.Where(d => d.IsActive);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.ToLower();
                query = query.Where(d => d.DepartmentName.ToLower().Contains(search) || 
                                         d.DepartmentCode.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Include(d => d.Campus)
                .OrderBy(d => d.DepartmentName)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(d => MapToDto(d))
                .ToListAsync(cancellationToken);

            return new PaginationResponse<OrgDepartmentDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        private OrgDepartmentDto MapToDto(Organization.Service.Entities.OrgDepartment department)
        {
            return new OrgDepartmentDto
            {
                DepartmentId = department.DepartmentId,
                CampusId = department.CampusId,
                ClientId = department.ClientId,
                CampusName = department.Campus?.CampusName,
                DepartmentCode = department.DepartmentCode,
                DepartmentName = department.DepartmentName,
                DepartmentType = department.DepartmentType,
                HeadName = department.HeadName,
                HeadEmail = department.HeadEmail,
                HeadPhone = department.HeadPhone,
                Description = department.Description,
                IsActive = department.IsActive
            };
        }
    }

    // ==================== Reference Data Query Handlers ====================
    public class GetAllLanguagesQueryHandler : IRequestHandler<GetAllLanguagesQuery, IEnumerable<OrgLanguageDto>>
    {
        private readonly OrganizationDbContext _context;

        public GetAllLanguagesQueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrgLanguageDto>> Handle(GetAllLanguagesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.OrgLanguages.Where(l => !l.IsDeleted);

            if (!request.IncludeInactive)
                query = query.Where(l => l.IsActive);

            return await query
                .OrderBy(l => l.IsDefault ? 0 : 1)
                .ThenBy(l => l.LanguageName)
                .Select(l => new OrgLanguageDto
                {
                    LanguageId = l.LanguageId,
                    LanguageName = l.LanguageName,
                    LanguageCode = l.LanguageCode,
                    IsDefault = l.IsDefault,
                    IsActive = l.IsActive
                })
                .ToListAsync(cancellationToken);
        }
    }

    public class GetAllCastesQueryHandler : IRequestHandler<GetAllCastesQuery, IEnumerable<OrgCasteDto>>
    {
        private readonly OrganizationDbContext _context;

        public GetAllCastesQueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrgCasteDto>> Handle(GetAllCastesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.OrgCastes.Where(c => !c.IsDeleted);

            if (!request.IncludeInactive)
                query = query.Where(c => c.IsActive);

            return await query
                .OrderBy(c => c.CasteName)
                .Select(c => new OrgCasteDto
                {
                    CasteId = c.CasteId,
                    CasteName = c.CasteName,
                    CasteCode = c.CasteCode,
                    IsActive = c.IsActive
                })
                .ToListAsync(cancellationToken);
        }
    }

    public class GetAllOccupationsQueryHandler : IRequestHandler<GetAllOccupationsQuery, IEnumerable<OccupationDto>>
    {
        private readonly OrganizationDbContext _context;

        public GetAllOccupationsQueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OccupationDto>> Handle(GetAllOccupationsQuery request, CancellationToken cancellationToken)
        {
            // Occupation uses legacy schema in this deployment (Id/code/stxt/ltxt).
            return await _context.OccupationLegacy
                .Where(o => !o.utblock)
                .OrderBy(o => o.stxt)
                .Select(o => new OccupationDto
                {
                    OccupationId = o.Id,
                    OccupationName = o.stxt ?? string.Empty,
                    OccupationCode = o.code ?? string.Empty,
                    Description = o.ltxt ?? string.Empty,
                    IsActive = true
                })
                .ToListAsync(cancellationToken);
        }
    }
}
