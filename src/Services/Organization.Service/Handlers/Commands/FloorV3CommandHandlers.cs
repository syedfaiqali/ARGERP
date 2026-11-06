using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Handlers;

namespace Organization.Service.Handlers.Commands
{
    public sealed class CreateFloorV3CommandHandler : IRequestHandler<CreateFloorV3Command, FloorV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public CreateFloorV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<FloorV3Dto> Handle(CreateFloorV3Command request, CancellationToken cancellationToken)
        {
            var clientId = RoomV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = RoomV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");
            var code = RoomV3AccessHelpers.EnsureRequiredText(request.Code, "Code").ToUpperInvariant();
            var floorName = RoomV3AccessHelpers.EnsureRequiredText(request.FloorName, "Floor Name");
            var buildingId = RoomV3AccessHelpers.EnsurePositive(request.BuildingId, "Building");

            if (code.Length > 10)
                throw new InvalidOperationException("Code must be 10 characters or less");
            if (floorName.Length > 100)
                throw new InvalidOperationException("Floor Name must be 100 characters or less");

            await RoomV3AccessHelpers.EnsureBuildingAsync(_context, clientId, entityId, buildingId, cancellationToken);

            var codeExists = await _context.OrgFloorLegacy
                .AsNoTracking()
                .AnyAsync(f =>
                    f.clientid == clientId
                    && f.entityid == entityId
                    && !f.utblock
                    && f.code != null
                    && (f.code ?? "").Trim().ToUpper() == code.ToUpper(), cancellationToken);
            if (codeExists)
                throw new InvalidOperationException($"Code : {code} already exists");

            var nameExists = await _context.OrgFloorLegacy
                .AsNoTracking()
                .AnyAsync(f =>
                    f.clientid == clientId
                    && f.entityid == entityId
                    && f.buildingid == buildingId
                    && !f.utblock
                    && f.stxt != null
                    && (f.stxt ?? "").Trim().ToUpper() == floorName.ToUpper(), cancellationToken);
            if (nameExists)
                throw new InvalidOperationException($"Floor : {floorName} already exists in this building");

            var nextId = (await _context.OrgFloorLegacy.IgnoreQueryFilters()
                .MaxAsync(f => (int?)f.Id, cancellationToken) ?? 0) + 1;

            var floor = new Entities.OrgFloorLegacy
            {
                Id = nextId,
                code = code,
                stxt = floorName,
                buildingid = buildingId,
                clientid = clientId,
                entityid = entityId,
                utsysgen = false,
                utblock = false,
                utispartial = false,
                utrecordversion = 0
            };

            _context.OrgFloorLegacy.Add(floor);
            await _context.SaveChangesAsync(cancellationToken);

            var buildingName = await _context.OrgBuildingLegacy
                .Where(b => b.Id == buildingId && b.clientid == clientId && b.entityid == entityId)
                .Select(b => b.stxt)
                .FirstOrDefaultAsync(cancellationToken);

            return new FloorV3Dto
            {
                Id = floor.Id,
                Code = floor.code,
                FloorName = floor.stxt,
                BuildingId = floor.buildingid ?? 0,
                BuildingName = buildingName ?? string.Empty,
                ClientId = floor.clientid,
                EntityId = floor.entityid
            };
        }
    }

    public sealed class UpdateFloorV3CommandHandler : IRequestHandler<UpdateFloorV3Command, FloorV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public UpdateFloorV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<FloorV3Dto> Handle(UpdateFloorV3Command request, CancellationToken cancellationToken)
        {
            var id = RoomV3AccessHelpers.EnsurePositive(request.Id, "Id");
            var clientId = RoomV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = RoomV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");
            var code = RoomV3AccessHelpers.EnsureRequiredText(request.Code, "Code").ToUpperInvariant();
            var floorName = RoomV3AccessHelpers.EnsureRequiredText(request.FloorName, "Floor Name");
            var buildingId = RoomV3AccessHelpers.EnsurePositive(request.BuildingId, "Building");

            if (code.Length > 10)
                throw new InvalidOperationException("Code must be 10 characters or less");
            if (floorName.Length > 100)
                throw new InvalidOperationException("Floor Name must be 100 characters or less");

            await RoomV3AccessHelpers.EnsureBuildingAsync(_context, clientId, entityId, buildingId, cancellationToken);

            var floor = await _context.OrgFloorLegacy
                .FirstOrDefaultAsync(f => f.Id == id && f.clientid == clientId && f.entityid == entityId && !f.utblock, cancellationToken);
            if (floor == null)
                throw new KeyNotFoundException($"Floor with ID {id} not found");

            var codeExists = await _context.OrgFloorLegacy
                .AsNoTracking()
                .AnyAsync(f =>
                    f.Id != id
                    && f.clientid == clientId
                    && f.entityid == entityId
                    && !f.utblock
                    && f.code != null
                    && (f.code ?? "").Trim().ToUpper() == code.ToUpper(), cancellationToken);
            if (codeExists)
                throw new InvalidOperationException($"Code : {code} already exists");

            var nameExists = await _context.OrgFloorLegacy
                .AsNoTracking()
                .AnyAsync(f =>
                    f.Id != id
                    && f.clientid == clientId
                    && f.entityid == entityId
                    && f.buildingid == buildingId
                    && !f.utblock
                    && f.stxt != null
                    && (f.stxt ?? "").Trim().ToUpper() == floorName.ToUpper(), cancellationToken);
            if (nameExists)
                throw new InvalidOperationException($"Floor : {floorName} already exists in this building");

            floor.code = code;
            floor.stxt = floorName;
            floor.buildingid = buildingId;

            _context.OrgFloorLegacy.Update(floor);
            await _context.SaveChangesAsync(cancellationToken);

            var buildingName = await _context.OrgBuildingLegacy
                .Where(b => b.Id == buildingId && b.clientid == clientId && b.entityid == entityId)
                .Select(b => b.stxt)
                .FirstOrDefaultAsync(cancellationToken);

            return new FloorV3Dto
            {
                Id = floor.Id,
                Code = floor.code ?? string.Empty,
                FloorName = floor.stxt ?? string.Empty,
                BuildingId = floor.buildingid ?? 0,
                BuildingName = buildingName ?? string.Empty,
                ClientId = floor.clientid,
                EntityId = floor.entityid
            };
        }
    }

    public sealed class DeleteFloorV3CommandHandler : IRequestHandler<DeleteFloorV3Command, bool>
    {
        private readonly OrganizationDbContext _context;

        public DeleteFloorV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteFloorV3Command request, CancellationToken cancellationToken)
        {
            var id = RoomV3AccessHelpers.EnsurePositive(request.Id, "Id");
            var clientId = RoomV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = RoomV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");

            var floor = await _context.OrgFloorLegacy
                .FirstOrDefaultAsync(f => f.Id == id && f.clientid == clientId && f.entityid == entityId && !f.utblock, cancellationToken);
            if (floor == null)
                throw new KeyNotFoundException($"Floor with ID {id} not found");

            // Check dependent legacy rooms (Orgroom)
            var hasLegacyRooms = await _context.OrgRoomLegacy.AnyAsync(
                r => r.lovfloorid == id && r.clientid == clientId && r.entityid == entityId && !r.utblock,
                cancellationToken);
            if (hasLegacyRooms)
                throw new InvalidOperationException("Cannot delete floor as it has associated rooms");

            floor.utblock = true;
            floor.utrecordversion += 1;
            _context.OrgFloorLegacy.Update(floor);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
