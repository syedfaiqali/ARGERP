using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Entities;
using Organization.Service.Infrastructure.Data;

namespace Organization.Service.Handlers.Commands
{
    public sealed class CreateCityV3CommandHandler : IRequestHandler<CreateCityV3Command, CityV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public CreateCityV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<CityV3Dto> Handle(CreateCityV3Command request, CancellationToken cancellationToken)
        {
            CityV3AccessHelpers.ValidateRequiredFields(request.CityCode, request.CityName, request.CountryId, request.StateId);

            var normalizedCode = request.CityCode.Trim().ToUpperInvariant();
            var cityName = request.CityName.Trim();

            if (normalizedCode.Length > 10)
                throw new InvalidOperationException("City Code must be 10 characters or less");

            if (cityName.Length > 100)
                throw new InvalidOperationException("City Name must be 100 characters or less");

            var state = await _context.State
                .Include(s => s.Country)
                .FirstOrDefaultAsync(s => s.Id == request.StateId, cancellationToken);

            if (state == null)
                throw new KeyNotFoundException($"State with ID {request.StateId} not found");

            if (state.CountryId != request.CountryId)
                throw new InvalidOperationException("Selected state does not belong to the selected country");

            var codeExists = await _context.City
                .AnyAsync(c => !c.UtBlock && (c.CityCode ?? "").Trim().ToUpper() == normalizedCode.ToUpper(), cancellationToken);
            if (codeExists)
                throw new InvalidOperationException("City Code already exists");

            var nameExists = await _context.City
                .AnyAsync(c => !c.UtBlock && c.CountryId == request.CountryId && c.StateId == request.StateId && (c.Citystxt ?? "").Trim().ToUpper() == cityName.ToUpper(), cancellationToken);
            if (nameExists)
                throw new InvalidOperationException("City already exists against selected country and state");

            var nextCityId = (await _context.City.IgnoreQueryFilters()
                .MaxAsync(c => (int?)c.Id, cancellationToken) ?? 0) + 1;

            var city = new City
            {
                Id = nextCityId,
                CountryId = request.CountryId,
                StateId = request.StateId,
                CityCode = normalizedCode,
                Citystxt = cityName,
                DialCode = request.DialCode,
                UtBlock = false,
                //CreatedAt = DateTime.UtcNow,
                //CreatedBy = request.CreatedBy,
            };

            _context.City.Add(city);
            await _context.SaveChangesAsync(cancellationToken);

            return new CityV3Dto
            {
                CityId = city.Id,
                CityCode = city.CityCode,
                CityName = city.Citystxt,
                DialCode = city.DialCode,
                StateId = city.StateId,
                StateName = state.stxt,
                CountryId = city.CountryId,
                CountryName = state.Country?.CountryName ?? string.Empty,
                IsActive = request.IsActive,
            };
        }
    }

    public sealed class UpdateCityV3CommandHandler : IRequestHandler<UpdateCityV3Command, CityV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public UpdateCityV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<CityV3Dto> Handle(UpdateCityV3Command request, CancellationToken cancellationToken)
        {
            if (request.CityId <= 0)
                throw new InvalidOperationException("CityId is required");

            CityV3AccessHelpers.ValidateRequiredFields(request.CityCode, request.CityName, request.CountryId, request.StateId);

            var normalizedCode = request.CityCode.Trim().ToUpperInvariant();
            var cityName = request.CityName.Trim();

            if (normalizedCode.Length > 10)
                throw new InvalidOperationException("City Code must be 10 characters or less");

            if (cityName.Length > 100)
                throw new InvalidOperationException("City Name must be 100 characters or less");

            var city = await _context.City
                .Include(c => c.State)
                .ThenInclude(s => s.Country)
                .FirstOrDefaultAsync(c => c.Id == request.CityId && !c.UtBlock, cancellationToken);

            if (city == null)
                throw new KeyNotFoundException($"City with ID {request.CityId} not found");

            var state = await _context.State
                .Include(s => s.Country)
                .FirstOrDefaultAsync(s => s.Id == request.StateId, cancellationToken);

            if (state == null)
                throw new KeyNotFoundException($"State with ID {request.StateId} not found");

            if (state.CountryId != request.CountryId)
                throw new InvalidOperationException("Selected state does not belong to the selected country");

            var codeExists = await _context.City
                .AnyAsync(c => !c.UtBlock && c.Id != request.CityId && (c.CityCode ?? "").Trim().ToUpper() == normalizedCode.ToUpper(), cancellationToken);
            if (codeExists)
                throw new InvalidOperationException("City Code already exists");

            var nameExists = await _context.City
                .AnyAsync(c => !c.UtBlock && c.Id != request.CityId && c.CountryId == request.CountryId && c.StateId == request.StateId && (c.Citystxt ?? "").Trim().ToUpper() == cityName.ToUpper(), cancellationToken);
            if (nameExists)
                throw new InvalidOperationException("City already exists against selected country and state");

            city.CountryId = request.CountryId;
            city.StateId = request.StateId;
            city.CityCode = normalizedCode;
            city.Citystxt = cityName;
            city.DialCode = request.DialCode;
            //city.UpdatedAt = DateTime.UtcNow;
            //city.UpdatedBy = request.UpdatedBy;

            _context.City.Update(city);
            await _context.SaveChangesAsync(cancellationToken);

            return new CityV3Dto
            {
                CityId = city.Id,
                CityCode = city.CityCode,
                CityName = city.Citystxt,
                DialCode = city.DialCode,
                StateId = city.StateId,
                StateName = state.stxt,
                CountryId = city.CountryId,
                CountryName = state.Country?.CountryName ?? string.Empty,
                IsActive = request.IsActive,
            };
        }
    }

    public sealed class DeleteCityV3CommandHandler : IRequestHandler<DeleteCityV3Command, bool>
    {
        private readonly OrganizationDbContext _context;

        public DeleteCityV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteCityV3Command request, CancellationToken cancellationToken)
        {
            if (request.CityId <= 0)
                throw new InvalidOperationException("CityId is required");

            var city = await _context.City
                .FirstOrDefaultAsync(c => c.Id == request.CityId && !c.UtBlock, cancellationToken);

            if (city == null)
                throw new KeyNotFoundException($"City with ID {request.CityId} not found");

            city.UtBlock = true;
            _context.City.Update(city);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    internal static class CityV3AccessHelpers
    {
        internal static int EnsureClientId(int? clientId)
        {
            if (!clientId.HasValue || clientId.Value <= 0)
                throw new InvalidOperationException("X-Client-Id header is required");
            return clientId.Value;
        }

        internal static void ValidateRequiredFields(string cityCode, string cityName, int countryId, int stateId)
        {
            OrganizationValidation.Text(cityCode, "City Code", 10);
            OrganizationValidation.Text(cityName, "City Name", 100);
            if (countryId <= 0)
                throw new InvalidOperationException("Country is required");
            if (stateId <= 0)
                throw new InvalidOperationException("State is required");
            if (string.IsNullOrWhiteSpace(cityCode))
                throw new InvalidOperationException("City Code is required");
            if (string.IsNullOrWhiteSpace(cityName))
                throw new InvalidOperationException("City Name is required");
        }

        internal static IQueryable<int> GetAccessibleCityIdsQuery(this OrganizationDbContext context, int clientId)
        {
            var entityCities = context.OrgEntities
                .Where(e => e.ClientId == clientId && !e.IsDeleted && e.CityId.HasValue)
                .Select(e => e.CityId!.Value);

            var campusCities = context.OrgCampuses
                .Where(c => c.ClientId == clientId && !c.IsDeleted && c.CityId.HasValue)
                .Select(c => c.CityId!.Value);

            return entityCities.Union(campusCities).Distinct();
        }

        internal static async Task<bool> HasClientLocationScopeAsync(this OrganizationDbContext context, int clientId, int countryId, int stateId, CancellationToken cancellationToken)
        {
            var hasEntityScope = await context.OrgEntities
                .AnyAsync(e => e.ClientId == clientId && !e.IsDeleted && e.CountryId == countryId && e.StateId == stateId, cancellationToken);
            if (hasEntityScope)
                return true;

            return await context.OrgCampuses
                .AnyAsync(c => c.ClientId == clientId && !c.IsDeleted && c.CountryId == countryId && c.StateId == stateId, cancellationToken);
        }
    }
}
