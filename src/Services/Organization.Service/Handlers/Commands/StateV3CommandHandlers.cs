using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Entities;
using Organization.Service.Infrastructure.Data;
using System.Data;

namespace Organization.Service.Handlers.Commands
{
    public sealed class CreateStateV3CommandHandler : IRequestHandler<CreateStateV3Command, StateV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public CreateStateV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<StateV3Dto> Handle(CreateStateV3Command request, CancellationToken cancellationToken)
        {
            if (request.CountryId <= 0)
                throw new InvalidOperationException("Country is required");

            var stateCode = request.StateCode?.Trim();
            var stateName = request.StateName?.Trim();

            if (string.IsNullOrWhiteSpace(stateCode))
                throw new InvalidOperationException("State Code is required");

            if (string.IsNullOrWhiteSpace(stateName))
                throw new InvalidOperationException("State Name is required");

            var normalizedCode = stateCode.ToUpperInvariant();

            if (normalizedCode.Length > 5)
                throw new InvalidOperationException("State Code must be 5 characters or less");

            if (stateName.Length > 150)
                throw new InvalidOperationException("State Name must be 150 characters or less");

            var country = await _context.Country
                .FirstOrDefaultAsync(c => c.Id == request.CountryId, cancellationToken);

            if (country == null)
                throw new KeyNotFoundException($"Country with ID {request.CountryId} not found");

            var codeExists = await _context.State
                .AnyAsync(s => s.CountryId == request.CountryId
                    && (s.StateCode ?? "").Trim().ToUpper() == normalizedCode.ToUpper()
                    , cancellationToken);

            if (codeExists)
                throw new InvalidOperationException("State Code already exists for the selected country");

            if (await _context.State.AnyAsync(s => s.CountryId == request.CountryId
                && (s.stxt ?? "").Trim().ToUpper() == stateName.ToUpper(), cancellationToken))
                throw new InvalidOperationException("State Name already exists for the selected country");

            // Legacy dbo.State table uses non-identity Id in some environments.
            // Generate the next key in-app to avoid NULL Id insert failures.
            var nextStateId = (await _context.State.IgnoreQueryFilters()
                .MaxAsync(s => (int?)s.Id, cancellationToken) ?? 0) + 1;

            var state = new State
            {
                Id = nextStateId,
                CountryId = request.CountryId,
                StateCode = normalizedCode,
                stxt = stateName,
                //IsActive = request.IsActive,
                //CreatedAt = DateTime.UtcNow,
                //CreatedBy = request.CreatedBy,
            };

            _context.State.Add(state);
            await _context.SaveChangesAsync(cancellationToken);

            return new StateV3Dto
            {
                StateId = state.Id,
                CountryId = state.CountryId,
                StateCode = state.StateCode,
                StateName = state.stxt,
                CountryName = country.CountryName,
                //IsActive = state.IsActive,
            };
        }
    }

    public sealed class UpdateStateV3CommandHandler : IRequestHandler<UpdateStateV3Command, StateV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public UpdateStateV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<StateV3Dto> Handle(UpdateStateV3Command request, CancellationToken cancellationToken)
        {
            if (request.StateId <= 0)
                throw new InvalidOperationException("StateId is required");

            if (request.CountryId <= 0)
                throw new InvalidOperationException("Country is required");

            var stateCode = request.StateCode?.Trim();
            var stateName = request.StateName?.Trim();

            if (string.IsNullOrWhiteSpace(stateCode))
                throw new InvalidOperationException("State Code is required");

            if (string.IsNullOrWhiteSpace(stateName))
                throw new InvalidOperationException("State Name is required");

            var normalizedCode = stateCode.ToUpperInvariant();

            if (normalizedCode.Length > 5)
                throw new InvalidOperationException("State Code must be 5 characters or less");

            if (stateName.Length > 150)
                throw new InvalidOperationException("State Name must be 150 characters or less");

            var state = await _context.State
                .FirstOrDefaultAsync(s => s.Id == request.StateId, cancellationToken);

            if (state == null)
                throw new KeyNotFoundException($"State with ID {request.StateId} not found");

            var country = await _context.Country
                .FirstOrDefaultAsync(c => c.Id == request.CountryId, cancellationToken);

            if (country == null)
                throw new KeyNotFoundException($"Country with ID {request.CountryId} not found");

            var codeExists = await _context.State
                .AnyAsync(s => s.Id != request.StateId
                    && s.CountryId == request.CountryId
                    && (s.StateCode ?? "").Trim().ToUpper() == normalizedCode.ToUpper()
                    , cancellationToken);

            if (codeExists)
                throw new InvalidOperationException("State Code already exists for the selected country");

            if (await _context.State.AnyAsync(s => s.CountryId == request.CountryId && s.Id != request.StateId
                && (s.stxt ?? "").Trim().ToUpper() == stateName.ToUpper(), cancellationToken))
                throw new InvalidOperationException("State Name already exists for the selected country");

            state.CountryId = request.CountryId;
            state.StateCode = normalizedCode;
            state.stxt = stateName;
            //state.IsActive = request.IsActive;
            //state.UpdatedAt = DateTime.UtcNow;
            //state.UpdatedBy = request.UpdatedBy;

            _context.State.Update(state);
            await _context.SaveChangesAsync(cancellationToken);

            return new StateV3Dto
            {
                StateId = state.Id,
                CountryId = state.CountryId,
                StateCode = state.StateCode,
                StateName = state.stxt,
                CountryName = country.CountryName,
                //IsActive = state.IsActive,
            };
        }
    }

    public sealed class DeleteStateV3CommandHandler : IRequestHandler<DeleteStateV3Command, bool>
    {
        private readonly OrganizationDbContext _context;

        public DeleteStateV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteStateV3Command request, CancellationToken cancellationToken)
        {
            if (request.StateId <= 0)
                throw new InvalidOperationException("StateId is required");

            var state = await _context.State
                .FirstOrDefaultAsync(s => s.Id == request.StateId, cancellationToken);

            if (state == null)
                throw new KeyNotFoundException($"State with ID {request.StateId} not found");

            var hasCities = await _context.City
                .AnyAsync(c => !c.UtBlock && c.StateId == request.StateId, cancellationToken);

            if (hasCities)
                throw new InvalidOperationException("Cannot delete state with existing cities");

            var hasEntities = false;
            if (await TableExistsAsync("OrgEntities", cancellationToken))
            {
                hasEntities = await _context.OrgEntities
                    .AnyAsync(e => e.StateId == request.StateId && !e.IsDeleted, cancellationToken);
            }

            if (hasEntities)
                throw new InvalidOperationException("Cannot delete state with existing entities");

            var hasCampuses = false;
            if (await TableExistsAsync("OrgCampuses", cancellationToken))
            {
                hasCampuses = await _context.OrgCampuses
                    .AnyAsync(c => c.StateId == request.StateId && !c.IsDeleted, cancellationToken);
            }

            if (hasCampuses)
                throw new InvalidOperationException("Cannot delete state with existing campuses");

            _context.Entry(state).Property<bool>("utblock").CurrentValue = true;
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
