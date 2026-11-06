using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Entities;
using Organization.Service.Infrastructure.Data;
using System.Data;
using Organization.Service.Mappers;

namespace Organization.Service.Handlers.Commands
{
    public class CreateCountryV3CommandHandler : IRequestHandler<CreateCountryV3Command, CountryV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public CreateCountryV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<CountryV3Dto> Handle(CreateCountryV3Command request, CancellationToken cancellationToken)
        {
            OrganizationValidation.Text(request.CountryCode, "Country Code", 2);
            OrganizationValidation.Text(request.CountryName, "Country Name", 100);
            OrganizationValidation.Text(request.IsoCode, "ISO Code", 5);
            OrganizationValidation.Text(request.DialCode, "Dial Code", 10);
            var code = request.CountryCode?.Trim();
            var name = request.CountryName?.Trim();
            var iso = request.IsoCode?.Trim();
            var dial = request.DialCode?.Trim();

            if (string.IsNullOrWhiteSpace(code))
                throw new InvalidOperationException("Country Code is required");

            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Country Name is required");

            var normalizedCode = code.ToUpperInvariant();
            var normalizedName = name;

            var codeExists = await _context.Country
                .AnyAsync(c => !c.UtBlock && (c.CountryCode ?? "").Trim().ToUpper() == normalizedCode.ToUpper(), cancellationToken);

            if (codeExists)
                throw new InvalidOperationException("Country Code already exists");

            var nameExists = await _context.Country
                .AnyAsync(c => !c.UtBlock && (c.CountryName ?? "").Trim().ToUpper() == normalizedName.ToUpper(), cancellationToken);

            if (nameExists)
                throw new InvalidOperationException("Country Name already exists");

            // Legacy dbo.Country table uses non-identity Id in some environments.
            // Generate the next key in-app to avoid NULL Id insert failures.
            var nextCountryId = (await _context.Country.IgnoreQueryFilters()
                .MaxAsync(c => (int?)c.Id, cancellationToken) ?? 0) + 1;

            var country = new Country
            {
                Id = nextCountryId,
                CountryCode = normalizedCode,
                CountryName = normalizedName,
                CountryCode3 = string.IsNullOrWhiteSpace(iso) ? null : iso.ToUpperInvariant(),
                DialCode = string.IsNullOrWhiteSpace(dial) ? null : dial,
                UtBlock = false,
                //CurrencyCode = currencyCode.ToUpperInvariant(),
                //CurrencyName = string.IsNullOrWhiteSpace(currencyName) ? null : currencyName,
                //CurrencySymbol = string.IsNullOrWhiteSpace(currencySymbol) ? null : currencySymbol,
                //IsActive = request.IsActive,
                //CreatedBy = request.CreatedBy,
                //CreatedAt = DateTime.UtcNow
            };

            _context.Country.Add(country);
            await _context.SaveChangesAsync(cancellationToken);

            return CountryV3Mapper.ToDto(country);
        }
    }

    public class UpdateCountryV3CommandHandler : IRequestHandler<UpdateCountryV3Command, CountryV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public UpdateCountryV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<CountryV3Dto> Handle(UpdateCountryV3Command request, CancellationToken cancellationToken)
        {
            var country = await _context.Country
                .FirstOrDefaultAsync(c => c.Id == request.CountryId && !c.UtBlock, cancellationToken);

            if (country == null)
                throw new KeyNotFoundException($"Country with ID {request.CountryId} not found");

            OrganizationValidation.Text(request.CountryCode, "Country Code", 2);
            OrganizationValidation.Text(request.CountryName, "Country Name", 100);
            OrganizationValidation.Text(request.IsoCode, "ISO Code", 5);
            OrganizationValidation.Text(request.DialCode, "Dial Code", 10);
            var code = request.CountryCode?.Trim();
            var name = request.CountryName?.Trim();
            var iso = request.IsoCode?.Trim();
            var dial = request.DialCode?.Trim();

            if (string.IsNullOrWhiteSpace(code))
                throw new InvalidOperationException("Country Code is required");

            if (string.IsNullOrWhiteSpace(name))
                throw new InvalidOperationException("Country Name is required");

            var normalizedCode = code.ToUpperInvariant();
            var normalizedName = name;

            var codeExists = await _context.Country
                .AnyAsync(c => !c.UtBlock && (c.CountryCode ?? "").Trim().ToUpper() == normalizedCode.ToUpper() && c.Id != request.CountryId, cancellationToken);

            if (codeExists)
                throw new InvalidOperationException("Country Code already exists");

            var nameExists = await _context.Country
                .AnyAsync(c => !c.UtBlock && (c.CountryName ?? "").Trim().ToUpper() == normalizedName.ToUpper() && c.Id != request.CountryId, cancellationToken);

            if (nameExists)
                throw new InvalidOperationException("Country Name already exists");

            country.CountryCode = normalizedCode;
            country.CountryName = normalizedName;
            country.CountryCode3 = string.IsNullOrWhiteSpace(iso) ? null : iso.ToUpperInvariant();
            country.DialCode = string.IsNullOrWhiteSpace(dial) ? null : dial;
            //country.CurrencyCode = currencyCode.ToUpperInvariant();
            //country.CurrencyName = string.IsNullOrWhiteSpace(currencyName) ? null : currencyName;
            //country.CurrencySymbol = string.IsNullOrWhiteSpace(currencySymbol) ? null : currencySymbol;
            //country.IsActive = request.IsActive;
            //country.UpdatedBy = request.UpdatedBy;
            //country.UpdatedAt = DateTime.UtcNow;

            _context.Country.Update(country);
            await _context.SaveChangesAsync(cancellationToken);

            return CountryV3Mapper.ToDto(country);
        }

        private async Task<bool> TableExistsAsync(string tableName, CancellationToken cancellationToken)
        {
            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;

            if (shouldClose)
                await connection.OpenAsync(cancellationToken);

            try
            {
                await using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT 1
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_SCHEMA = 'dbo'
                      AND TABLE_NAME = @tableName";

                var parameter = command.CreateParameter();
                parameter.ParameterName = "@tableName";
                parameter.Value = tableName;
                command.Parameters.Add(parameter);

                var result = await command.ExecuteScalarAsync(cancellationToken);
                return result != null;
            }
            finally
            {
                if (shouldClose)
                    await connection.CloseAsync();
            }
        }
    }

    public class DeleteCountryV3CommandHandler : IRequestHandler<DeleteCountryV3Command, bool>
    {
        private readonly OrganizationDbContext _context;

        public DeleteCountryV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteCountryV3Command request, CancellationToken cancellationToken)
        {
            var country = await _context.Country
                .FirstOrDefaultAsync(c => c.Id == request.CountryId && !c.UtBlock, cancellationToken);

            if (country == null)
                throw new KeyNotFoundException($"Country with ID {request.CountryId} not found");

            var hasStates = await _context.State
                .AnyAsync(s => s.CountryId == request.CountryId, cancellationToken);

            if (hasStates)
                throw new InvalidOperationException("Cannot delete country with existing states");

            var hasEntities = false;
            if (await TableExistsAsync("OrgEntities", cancellationToken))
            {
                hasEntities = await _context.OrgEntities
                    .AnyAsync(e => e.CountryId == request.CountryId && !e.IsDeleted, cancellationToken);
            }

            if (hasEntities)
                throw new InvalidOperationException("Cannot delete country with existing entities");

            var hasCampuses = false;
            if (await TableExistsAsync("OrgCampuses", cancellationToken))
            {
                hasCampuses = await _context.OrgCampuses
                    .AnyAsync(c => c.CountryId == request.CountryId && !c.IsDeleted, cancellationToken);
            }

            if (hasCampuses)
                throw new InvalidOperationException("Cannot delete country with existing campuses");

            country.UtBlock = true;
            //country.UpdatedBy = request.DeletedBy;
            //country.UpdatedAt = DateTime.UtcNow;

            _context.Country.Update(country);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        private async Task<bool> TableExistsAsync(string tableName, CancellationToken cancellationToken)
        {
            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;

            if (shouldClose)
                await connection.OpenAsync(cancellationToken);

            try
            {
                await using var command = connection.CreateCommand();
                command.CommandText = @"
                    SELECT 1
                    FROM INFORMATION_SCHEMA.TABLES
                    WHERE TABLE_SCHEMA = 'dbo'
                      AND TABLE_NAME = @tableName";

                var parameter = command.CreateParameter();
                parameter.ParameterName = "@tableName";
                parameter.Value = tableName;
                command.Parameters.Add(parameter);

                var result = await command.ExecuteScalarAsync(cancellationToken);
                return result != null;
            }
            finally
            {
                if (shouldClose)
                    await connection.CloseAsync();
            }
        }
    }
}
