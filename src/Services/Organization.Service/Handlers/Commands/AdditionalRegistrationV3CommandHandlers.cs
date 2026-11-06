using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Infrastructure.Data;

namespace Organization.Service.Handlers.Commands
{
    public sealed class CreateAdditionalRegistrationV3CommandHandler
        : IRequestHandler<CreateAdditionalRegistrationV3Command, AdditionalRegistrationV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public CreateAdditionalRegistrationV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<AdditionalRegistrationV3Dto> Handle(CreateAdditionalRegistrationV3Command request, CancellationToken cancellationToken)
        {
            var clientId = RoomV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = RoomV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");
            OrganizationValidation.Text(request.Caption, "Caption", 200);
            if (request.Sequance < 0)
                throw new InvalidOperationException("Sequence cannot be negative");

            var configId = RoomV3AccessHelpers.EnsurePositive(request.ConfigId, "ConfigId");
            var infoForId = RoomV3AccessHelpers.EnsurePositive(request.InfoForId, "InfoForId");

            var cfg = await _context.SMAdditionalinfoconfigLegacy
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == configId && !x.utblock, cancellationToken);
            if (cfg == null)
                throw new InvalidOperationException("Invalid configuration");
            if (cfg.infoforid.HasValue && cfg.infoforid.Value != infoForId)
                throw new InvalidOperationException("Configuration does not belong to selected Additional For");

            var existing = await _context.SMAdditionalregistrationLegacy
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    !x.utblock &&
                    x.clientid == clientId &&
                    x.entityid == entityId &&
                    x.infoforid == infoForId &&
                    x.configid == configId, cancellationToken);
            if (existing != null)
                throw new InvalidOperationException("Record already exists for this section");

            if (request.Sequance.HasValue)
            {
                var seqExists = await _context.SMAdditionalregistrationLegacy
                    .AsNoTracking()
                    .AnyAsync(x =>
                        !x.utblock &&
                        x.clientid == clientId &&
                        x.entityid == entityId &&
                        x.infoforid == infoForId &&
                        x.sequance == request.Sequance.Value, cancellationToken);
                if (seqExists)
                    throw new InvalidOperationException($"Sequance {request.Sequance.Value} already exists");
            }

            var nextId = (await _context.SMAdditionalregistrationLegacy.IgnoreQueryFilters()
                .MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            var row = new Entities.SMAdditionalregistrationLegacy
            {
                Id = nextId,
                configid = configId,
                clientid = clientId,
                entityid = entityId,
                infoforid = infoForId,
                caption = request.Caption?.Trim(),
                sequance = request.Sequance,
                hide = request.Visible,
                utsysgen = false,
                utblock = false,
                utispartial = false,
                utrecordversion = 0,
            };

            _context.SMAdditionalregistrationLegacy.Add(row);
            await _context.SaveChangesAsync(cancellationToken);

            return await AdditionalRegistrationV3Mapper.MapByIdAsync(_context, row.Id, clientId, entityId, cancellationToken);
        }
    }

    public sealed class UpdateAdditionalRegistrationV3CommandHandler
        : IRequestHandler<UpdateAdditionalRegistrationV3Command, AdditionalRegistrationV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public UpdateAdditionalRegistrationV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<AdditionalRegistrationV3Dto> Handle(UpdateAdditionalRegistrationV3Command request, CancellationToken cancellationToken)
        {
            var id = RoomV3AccessHelpers.EnsurePositive(request.Id, "Id");
            var clientId = RoomV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = RoomV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");
            OrganizationValidation.Text(request.Caption, "Caption", 200);
            if (request.Sequance < 0)
                throw new InvalidOperationException("Sequence cannot be negative");

            var configId = RoomV3AccessHelpers.EnsurePositive(request.ConfigId, "ConfigId");
            var infoForId = RoomV3AccessHelpers.EnsurePositive(request.InfoForId, "InfoForId");

            var row = await _context.SMAdditionalregistrationLegacy
                .FirstOrDefaultAsync(x => x.Id == id && !x.utblock && x.clientid == clientId && x.entityid == entityId, cancellationToken);
            if (row == null)
                throw new KeyNotFoundException($"Additional registration with ID {id} not found");

            var cfg = await _context.SMAdditionalinfoconfigLegacy
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == configId && !x.utblock, cancellationToken);
            if (cfg == null)
                throw new InvalidOperationException("Invalid configuration");
            if (cfg.infoforid.HasValue && cfg.infoforid.Value != infoForId)
                throw new InvalidOperationException("Configuration does not belong to selected Additional For");

            if (await _context.SMAdditionalregistrationLegacy.AnyAsync(x =>
                x.Id != id && x.clientid == clientId && x.entityid == entityId
                && x.infoforid == infoForId && x.configid == configId, cancellationToken))
                throw new InvalidOperationException("Record already exists for this section");

            if (request.Sequance.HasValue)
            {
                var seqExists = await _context.SMAdditionalregistrationLegacy
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.Id != id &&
                        !x.utblock &&
                        x.clientid == clientId &&
                        x.entityid == entityId &&
                        x.infoforid == infoForId &&
                        x.sequance == request.Sequance.Value, cancellationToken);
                if (seqExists)
                    throw new InvalidOperationException($"Sequance {request.Sequance.Value} already exists");
            }

            row.configid = configId;
            row.infoforid = infoForId;
            row.caption = request.Caption?.Trim();
            row.sequance = request.Sequance;
            row.hide = request.Visible;
            row.utrecordversion += 1;

            _context.SMAdditionalregistrationLegacy.Update(row);
            await _context.SaveChangesAsync(cancellationToken);

            return await AdditionalRegistrationV3Mapper.MapByIdAsync(_context, id, clientId, entityId, cancellationToken);
        }
    }

    public sealed class DeleteAdditionalRegistrationV3CommandHandler
        : IRequestHandler<DeleteAdditionalRegistrationV3Command, bool>
    {
        private readonly OrganizationDbContext _context;

        public DeleteAdditionalRegistrationV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteAdditionalRegistrationV3Command request, CancellationToken cancellationToken)
        {
            var id = RoomV3AccessHelpers.EnsurePositive(request.Id, "Id");
            var clientId = RoomV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = RoomV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");

            var row = await _context.SMAdditionalregistrationLegacy
                .FirstOrDefaultAsync(x => x.Id == id && !x.utblock && x.clientid == clientId && x.entityid == entityId, cancellationToken);
            if (row == null)
                throw new KeyNotFoundException($"Additional registration with ID {id} not found");

            row.utblock = true;
            row.utrecordversion += 1;
            _context.SMAdditionalregistrationLegacy.Update(row);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    internal static class AdditionalRegistrationV3Mapper
    {
        internal static async Task<AdditionalRegistrationV3Dto> MapByIdAsync(
            OrganizationDbContext context,
            int id,
            int clientId,
            int entityId,
            CancellationToken cancellationToken)
        {
            var dto = await (
                from reg in context.SMAdditionalregistrationLegacy.AsNoTracking()
                where reg.Id == id && !reg.utblock && reg.clientid == clientId && reg.entityid == entityId
                join cfg in context.SMAdditionalinfoconfigLegacy.AsNoTracking()
                    on reg.configid equals cfg.Id
                select new AdditionalRegistrationV3Dto
                {
                    Id = reg.Id,
                    ConfigId = reg.configid ?? 0,
                    Code = cfg.code ?? string.Empty,
                    SectionName = cfg.stxt ?? string.Empty,
                    Caption = string.IsNullOrWhiteSpace(reg.caption) ? (cfg.stxt ?? string.Empty) : reg.caption!,
                    InfoForId = reg.infoforid ?? (cfg.infoforid ?? 0),
                    Sequance = reg.sequance,
                    Visible = reg.hide,
                    Route = cfg.route ?? string.Empty,
                    Icon = cfg.webicon ?? string.Empty,
                    CssClass = cfg._class ?? string.Empty,
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (dto == null)
                throw new KeyNotFoundException($"Additional registration with ID {id} not found");

            return dto;
        }
    }
}
