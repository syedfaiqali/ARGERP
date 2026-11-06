using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Infrastructure.Data;

namespace Organization.Service.Handlers.Commands
{
    public sealed class CreateBuildingV3CommandHandler : IRequestHandler<CreateBuildingV3Command, BuildingV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public CreateBuildingV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<BuildingV3Dto> Handle(CreateBuildingV3Command request, CancellationToken cancellationToken)
        {
            var clientId = RoomV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = RoomV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");
            var code = RoomV3AccessHelpers.EnsureRequiredText(request.Code, "Code").ToUpperInvariant();
            var buildingName = RoomV3AccessHelpers.EnsureRequiredText(request.BuildingName, "Building Name");

            if (code.Length > 10)
                throw new InvalidOperationException("Code must be 10 characters or less");
            if (buildingName.Length > 100)
                throw new InvalidOperationException("Building Name must be 100 characters or less");

            var codeExists = await _context.OrgBuildingLegacy
                .AsNoTracking()
                .AnyAsync(b =>
                    b.clientid == clientId
                    && b.entityid == entityId
                    && !b.utblock
                    && b.code != null
                    && (b.code ?? "").Trim().ToUpper() == code.ToUpper(), cancellationToken);
            if (codeExists)
                throw new InvalidOperationException($"Code : {code} already exists");

            var nameExists = await _context.OrgBuildingLegacy
                .AsNoTracking()
                .AnyAsync(b =>
                    b.clientid == clientId
                    && b.entityid == entityId
                    && !b.utblock
                    && b.stxt != null
                    && (b.stxt ?? "").Trim().ToUpper() == buildingName.ToUpper(), cancellationToken);
            if (nameExists)
                throw new InvalidOperationException($"Building : {buildingName} already exists");

            var nextId = (await _context.OrgBuildingLegacy.IgnoreQueryFilters()
                .MaxAsync(b => (int?)b.Id, cancellationToken) ?? 0) + 1;

            var building = new Entities.OrgBuildingLegacy
            {
                Id = nextId,
                code = code,
                stxt = buildingName,
                clientid = clientId,
                entityid = entityId,
                utsysgen = false,
                utblock = false,
                utispartial = false,
                utrecordversion = 0
            };

            _context.OrgBuildingLegacy.Add(building);
            await _context.SaveChangesAsync(cancellationToken);

            return new BuildingV3Dto
            {
                Id = building.Id,
                Code = building.code,
                BuildingName = building.stxt,
                ClientId = building.clientid,
                EntityId = building.entityid
            };
        }
    }

    public sealed class UpdateBuildingV3CommandHandler : IRequestHandler<UpdateBuildingV3Command, BuildingV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public UpdateBuildingV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<BuildingV3Dto> Handle(UpdateBuildingV3Command request, CancellationToken cancellationToken)
        {
            var id = RoomV3AccessHelpers.EnsurePositive(request.Id, "Id");
            var clientId = RoomV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = RoomV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");
            var code = RoomV3AccessHelpers.EnsureRequiredText(request.Code, "Code").ToUpperInvariant();
            var buildingName = RoomV3AccessHelpers.EnsureRequiredText(request.BuildingName, "Building Name");

            if (code.Length > 10)
                throw new InvalidOperationException("Code must be 10 characters or less");
            if (buildingName.Length > 100)
                throw new InvalidOperationException("Building Name must be 100 characters or less");

            var building = await _context.OrgBuildingLegacy
                .FirstOrDefaultAsync(b => b.Id == id && b.clientid == clientId && b.entityid == entityId && !b.utblock, cancellationToken);
            if (building == null)
                throw new KeyNotFoundException($"Building with ID {id} not found");

            var codeExists = await _context.OrgBuildingLegacy
                .AsNoTracking()
                .AnyAsync(b =>
                    b.Id != id
                    && b.clientid == clientId
                    && b.entityid == entityId
                    && !b.utblock
                    && b.code != null
                    && (b.code ?? "").Trim().ToUpper() == code.ToUpper(), cancellationToken);
            if (codeExists)
                throw new InvalidOperationException($"Code : {code} already exists");

            var nameExists = await _context.OrgBuildingLegacy
                .AsNoTracking()
                .AnyAsync(b =>
                    b.Id != id
                    && b.clientid == clientId
                    && b.entityid == entityId
                    && !b.utblock
                    && b.stxt != null
                    && (b.stxt ?? "").Trim().ToUpper() == buildingName.ToUpper(), cancellationToken);
            if (nameExists)
                throw new InvalidOperationException($"Building : {buildingName} already exists");

            building.code = code;
            building.stxt = buildingName;

            _context.OrgBuildingLegacy.Update(building);
            await _context.SaveChangesAsync(cancellationToken);

            return new BuildingV3Dto
            {
                Id = building.Id,
                Code = building.code ?? string.Empty,
                BuildingName = building.stxt ?? string.Empty,
                ClientId = building.clientid,
                EntityId = building.entityid
            };
        }
    }

    public sealed class DeleteBuildingV3CommandHandler : IRequestHandler<DeleteBuildingV3Command, bool>
    {
        private readonly OrganizationDbContext _context;

        public DeleteBuildingV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteBuildingV3Command request, CancellationToken cancellationToken)
        {
            var id = RoomV3AccessHelpers.EnsurePositive(request.Id, "Id");
            var clientId = RoomV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = RoomV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");

            var building = await _context.OrgBuildingLegacy
                .FirstOrDefaultAsync(b => b.Id == id && b.clientid == clientId && b.entityid == entityId && !b.utblock, cancellationToken);
            if (building == null)
                throw new KeyNotFoundException($"Building with ID {id} not found");

            // Check for dependent floors or rooms
            var hasFloors = await _context.OrgFloorLegacy.AnyAsync(
                f => f.buildingid == id && f.clientid == clientId && f.entityid == entityId && !f.utblock,
                cancellationToken);
            if (hasFloors)
                throw new InvalidOperationException("Cannot delete building as it has associated floors");

            var hasRooms = await _context.OrgRoomLegacy.AnyAsync(
                r => r.buildingid == id && r.clientid == clientId && r.entityid == entityId && !r.utblock,
                cancellationToken);
            if (hasRooms)
                throw new InvalidOperationException("Cannot delete building as it has associated rooms");

            building.utblock = true;
            building.utrecordversion += 1;
            _context.OrgBuildingLegacy.Update(building);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
