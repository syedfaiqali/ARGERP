using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Entities;
using Organization.Service.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Organization.Service.Handlers.Commands
{
    // ==================== Country Command Handlers ====================
    public class CreateCountryCommandHandler : IRequestHandler<CreateCountryCommand, CountryDto>
    {
        private readonly OrganizationDbContext _context;

        public CreateCountryCommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<CountryDto> Handle(CreateCountryCommand request, CancellationToken cancellationToken)
        {
            // Check for duplicate country code
            var existingCountry = await _context.Country
                .FirstOrDefaultAsync(c => c.CountryCode == request.CountryCode, cancellationToken);
            
            if (existingCountry != null)
                throw new InvalidOperationException($"Country with code '{request.CountryCode}' already exists");

            var country = new Country
            {
                CountryName = request.CountryName,
                CountryCode = request.CountryCode.ToUpper(),
                //CountryCode3 = request.CountryCode3?.ToUpper(),
                //PhoneCode = request.PhoneCode,
                //CurrencyCode = request.CurrencyCode?.ToUpper(),
                //CurrencyName = request.CurrencyName,
                //CurrencySymbol = request.CurrencySymbol,
                //CreatedBy = request.CreatedBy,
                //CreatedAt = DateTime.UtcNow
            };

            _context.Country.Add(country);
            await _context.SaveChangesAsync(cancellationToken);

            return MapToDto(country);
        }

        private CountryDto MapToDto(Country country)
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

    public class UpdateCountryCommandHandler : IRequestHandler<UpdateCountryCommand, CountryDto>
    {
        private readonly OrganizationDbContext _context;

        public UpdateCountryCommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<CountryDto> Handle(UpdateCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _context.Country.FirstOrDefaultAsync(x => x.Id == request.CountryId, cancellationToken);
            if (country == null)
                throw new KeyNotFoundException($"Country with ID {request.CountryId} not found");

            country.CountryName = request.CountryName;
            //country.PhoneCode = request.PhoneCode;
            //country.CurrencyCode = request.CurrencyCode?.ToUpper();
            //country.CurrencyName = request.CurrencyName;
            //country.CurrencySymbol = request.CurrencySymbol;
            //country.IsActive = request.IsActive;
            //country.UpdatedBy = request.UpdatedBy;
            //country.UpdatedAt = DateTime.UtcNow;

            _context.Country.Update(country);
            await _context.SaveChangesAsync(cancellationToken);

            return MapToDto(country);
        }

        private CountryDto MapToDto(Country country)
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

    public class DeleteCountryCommandHandler : IRequestHandler<DeleteCountryCommand, bool>
    {
        private readonly OrganizationDbContext _context;

        public DeleteCountryCommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteCountryCommand request, CancellationToken cancellationToken)
        {
            var country = await _context.Country.FirstOrDefaultAsync(x => x.Id == request.CountryId, cancellationToken);
            if (country == null)
                throw new KeyNotFoundException($"Country with ID {request.CountryId} not found");

            // Check for child records (States)
            var hasStates = await _context.State.AnyAsync(s => s.CountryId == request.CountryId, cancellationToken);
            if (hasStates)
                throw new InvalidOperationException("Cannot delete country with existing states");

            country.UtBlock = true;
            //country.UpdatedBy = request.DeletedBy;
            //country.UpdatedAt = DateTime.UtcNow;

            _context.Country.Update(country);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

    // ==================== State Command Handlers ====================
    public class CreateStateCommandHandler : IRequestHandler<CreateStateCommand, StateDto>
    {
        private readonly OrganizationDbContext _context;

        public CreateStateCommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<StateDto> Handle(CreateStateCommand request, CancellationToken cancellationToken)
        {
            // Verify country exists
            var country = await _context.Country.FirstOrDefaultAsync(x => x.Id == request.CountryId, cancellationToken);
            if (country == null)
                throw new KeyNotFoundException($"Country with ID {request.CountryId} not found");

            var state = new State
            {
                CountryId = request.CountryId,
                stxt = request.StateName,
                StateCode = request.StateCode?.ToUpper(),
                //CreatedBy = request.CreatedBy,
                //CreatedAt = DateTime.UtcNow
            };

            _context.State.Add(state);
            await _context.SaveChangesAsync(cancellationToken);

            return MapToDto(state, country);
        }

        private StateDto MapToDto(State state, Country country)
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

    public class UpdateStateCommandHandler : IRequestHandler<UpdateStateCommand, StateDto>
    {
        private readonly OrganizationDbContext _context;

        public UpdateStateCommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<StateDto> Handle(UpdateStateCommand request, CancellationToken cancellationToken)
        {
            var state = await _context.State.Include(s => s.Country).FirstOrDefaultAsync(s => s.Id == request.StateId, cancellationToken);
            if (state == null)
                throw new KeyNotFoundException($"State with ID {request.StateId} not found");

            state.stxt = request.StateName;
            state.StateCode = request.StateCode?.ToUpper();
            //state.IsActive = request.IsActive;
            //state.UpdatedBy = request.UpdatedBy;
            //state.UpdatedAt = DateTime.UtcNow;

            _context.State.Update(state);
            await _context.SaveChangesAsync(cancellationToken);

            return MapToDto(state, state.Country);
        }

        private StateDto MapToDto(State state, Country country)
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

    public class DeleteStateCommandHandler : IRequestHandler<DeleteStateCommand, bool>
    {
        private readonly OrganizationDbContext _context;

        public DeleteStateCommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteStateCommand request, CancellationToken cancellationToken)
        {
            var state = await _context.State.FirstOrDefaultAsync(x => x.Id == request.StateId, cancellationToken);
            if (state == null)
                throw new KeyNotFoundException($"State with ID {request.StateId} not found");

            // Check for child records (Cities)
            var hasCities = await _context.City.AnyAsync(c => c.StateId == request.StateId, cancellationToken);
            if (hasCities)
                throw new InvalidOperationException("Cannot delete state with existing cities");

            _context.Entry(state).Property<bool>("utblock").CurrentValue = true;
            //state.UpdatedBy = request.DeletedBy;
            //state.UpdatedAt = DateTime.UtcNow;

            _context.State.Update(state);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }

    // ==================== City Command Handlers ====================
    public class CreateCityCommandHandler : IRequestHandler<CreateCityCommand, CityDto>
    {
        private readonly OrganizationDbContext _context;

        public CreateCityCommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<CityDto> Handle(CreateCityCommand request, CancellationToken cancellationToken)
        {
            // Verify state exists
            var state = await _context.State.Include(s => s.Country).FirstOrDefaultAsync(s => s.Id == request.StateId, cancellationToken);
            if (state == null)
                throw new KeyNotFoundException($"State with ID {request.StateId} not found");

            var normalizedCode = request.CityCode?.Trim().ToUpper();
            var cityName = request.CityName?.Trim();

            if (!string.IsNullOrWhiteSpace(normalizedCode))
            {
                var codeExists = await _context.City
                    .AnyAsync(c => !c.UtBlock && c.CityCode == normalizedCode, cancellationToken);
                if (codeExists)
                    throw new InvalidOperationException("City Code already exists");
            }

            var nameExists = await _context.City
                .AnyAsync(c => !c.UtBlock && c.CountryId == state.CountryId && c.StateId == request.StateId && c.Citystxt == cityName, cancellationToken);
            if (nameExists)
                throw new InvalidOperationException("City already exists against selected country and state");

            var city = new City
            {
                StateId = request.StateId,
                CountryId = state.CountryId,
                Citystxt = cityName,
                CityCode = normalizedCode,
                UtBlock = false,
                //CreatedBy = request.CreatedBy,
                //CreatedAt = DateTime.UtcNow
            };

            _context.City.Add(city);
            await _context.SaveChangesAsync(cancellationToken);

            return MapToDto(city, state);
        }

        private CityDto MapToDto(City city, State state)
        {
            return new CityDto
            {
                CityId = city.Id,
                StateId = city.StateId,
                CountryId = city.CountryId,
                CityName = city.Citystxt,
                CityCode = city.CityCode,
                StateName = state.stxt,
                CountryName = state.Country.CountryName,
                //IsActive = city.IsActive
            };
        }
    }

    public class UpdateCityCommandHandler : IRequestHandler<UpdateCityCommand, CityDto>
    {
        private readonly OrganizationDbContext _context;

        public UpdateCityCommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<CityDto> Handle(UpdateCityCommand request, CancellationToken cancellationToken)
        {
            var city = await _context.City.Include(c => c.State).ThenInclude(s => s.Country).FirstOrDefaultAsync(c => c.Id == request.CityId, cancellationToken);
            if (city == null)
                throw new KeyNotFoundException($"City with ID {request.CityId} not found");

            var normalizedCode = request.CityCode?.Trim().ToUpper();
            var cityName = request.CityName?.Trim();

            if (!string.IsNullOrWhiteSpace(normalizedCode))
            {
                var codeExists = await _context.City
                    .AnyAsync(c => !c.UtBlock && c.Id != request.CityId && c.CityCode == normalizedCode, cancellationToken);
                if (codeExists)
                    throw new InvalidOperationException("City Code already exists");
            }

            var nameExists = await _context.City
                .AnyAsync(c => !c.UtBlock && c.Id != request.CityId && c.CountryId == city.CountryId && c.StateId == city.StateId && c.Citystxt == cityName, cancellationToken);
            if (nameExists)
                throw new InvalidOperationException("City already exists against selected country and state");

            city.Citystxt = cityName;
            city.CityCode = normalizedCode;
            //city.IsActive = request.IsActive;
            //city.UpdatedBy = request.UpdatedBy;
            //city.UpdatedAt = DateTime.UtcNow;

            _context.City.Update(city);
            await _context.SaveChangesAsync(cancellationToken);

            return MapToDto(city, city.State);
        }

        private CityDto MapToDto(City city, State state)
        {
            return new CityDto
            {
                CityId = city.Id,
                StateId = city.StateId,
                CountryId = city.CountryId,
                CityName = city.Citystxt,
                CityCode = city.CityCode,
                StateName = state.stxt,
                CountryName = state.Country.CountryName,
                //IsActive = city.IsActive
            };
        }
    }

    public class DeleteCityCommandHandler : IRequestHandler<DeleteCityCommand, bool>
    {
        private readonly OrganizationDbContext _context;

        public DeleteCityCommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteCityCommand request, CancellationToken cancellationToken)
        {
            var city = await _context.City.FirstOrDefaultAsync(x => x.Id == request.CityId, cancellationToken);
            if (city == null)
                throw new KeyNotFoundException($"City with ID {request.CityId} not found");

            city.UtBlock = true;
            _context.City.Update(city);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
