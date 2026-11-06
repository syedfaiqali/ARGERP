using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Handlers;
using Organization.Service.Handlers.Queries;
using Organization.Service.Infrastructure.Data;

namespace Organization.Service.Handlers.Commands
{
    public sealed class CreateRoomV3CommandHandler : IRequestHandler<CreateRoomV3Command, RoomV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public CreateRoomV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<RoomV3Dto> Handle(CreateRoomV3Command request, CancellationToken cancellationToken)
        {
            var clientId = RoomV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = RoomV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");
            var code = RoomV3AccessHelpers.EnsureRequiredText(request.Code, "Code").ToUpperInvariant();
            var roomName = RoomV3AccessHelpers.EnsureRequiredText(request.RoomName, "Room Name");
            var roomTypeId = RoomV3AccessHelpers.EnsurePositive(request.RoomTypeId, "Room Type");
            var buildingId = RoomV3AccessHelpers.EnsurePositive(request.BuildingId, "Building");
            var floorId = RoomV3AccessHelpers.EnsurePositive(request.FloorId, "Floor");

            if (!request.Strength.HasValue)
                throw new InvalidOperationException("Strength is required");
            if (request.Strength.Value < 0)
                throw new InvalidOperationException("Strength must be zero or greater");

            if (code.Length > 10)
                throw new InvalidOperationException("Code must be 10 characters or less");
            if (roomName.Length > 100)
                throw new InvalidOperationException("Room Name must be 100 characters or less");

            await RoomV3AccessHelpers.EnsureRoomTypeAsync(_context, roomTypeId, cancellationToken);
            await RoomV3AccessHelpers.EnsureBuildingAsync(_context, clientId, entityId, buildingId, cancellationToken);
            await RoomV3AccessHelpers.EnsureFloorAsync(_context, clientId, entityId, floorId, buildingId, cancellationToken);

            var codeExists = await _context.OrgRoomLegacy
                .AsNoTracking()
                .AnyAsync(r =>
                    r.clientid == clientId
                    && r.entityid == entityId
                    && !r.utblock
                    && r.code != null
                    && (r.code ?? "").Trim().ToUpper() == code.ToUpper(), cancellationToken);
            if (codeExists)
                throw new InvalidOperationException($"Code : {code} already exists");

            var duplicateRoom = await _context.OrgRoomLegacy
                .AsNoTracking()
                .AnyAsync(r =>
                    r.clientid == clientId
                    && r.entityid == entityId
                    && !r.utblock
                    && r.buildingid == buildingId
                    && r.lovfloorid == floorId
                    && (r.stxt ?? "").Trim().ToUpper() == roomName.ToUpper(), cancellationToken);
            if (duplicateRoom)
                throw new InvalidOperationException($"Room : {roomName} already exists against Building and Floor");

            var nextId = (await _context.OrgRoomLegacy.IgnoreQueryFilters()
                .MaxAsync(r => (int?)r.Id, cancellationToken) ?? 0) + 1;

            var room = new Entities.OrgRoomLegacy
            {
                Id = nextId,
                code = code,
                stxt = roomName,
                clientid = clientId,
                entityid = entityId,
                roomtypeid = roomTypeId,
                buildingid = buildingId,
                lovfloorid = floorId,
                strength = request.Strength,
                utsysgen = false,
                utblock = false,
                utispartial = false,
                utrecordversion = 0,
            };

            _context.OrgRoomLegacy.Add(room);
            await _context.SaveChangesAsync(cancellationToken);

            return await RoomV3Mapper.MapByIdAsync(_context, room.Id, clientId, entityId, cancellationToken);
        }
    }

    public sealed class UpdateRoomV3CommandHandler : IRequestHandler<UpdateRoomV3Command, RoomV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public UpdateRoomV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<RoomV3Dto> Handle(UpdateRoomV3Command request, CancellationToken cancellationToken)
        {
            var id = RoomV3AccessHelpers.EnsurePositive(request.Id, "Id");
            var clientId = RoomV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = RoomV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");
            var code = RoomV3AccessHelpers.EnsureRequiredText(request.Code, "Code").ToUpperInvariant();
            var roomName = RoomV3AccessHelpers.EnsureRequiredText(request.RoomName, "Room Name");
            var roomTypeId = RoomV3AccessHelpers.EnsurePositive(request.RoomTypeId, "Room Type");
            var buildingId = RoomV3AccessHelpers.EnsurePositive(request.BuildingId, "Building");
            var floorId = RoomV3AccessHelpers.EnsurePositive(request.FloorId, "Floor");

            if (!request.Strength.HasValue)
                throw new InvalidOperationException("Strength is required");
            if (request.Strength.Value < 0)
                throw new InvalidOperationException("Strength must be zero or greater");

            if (code.Length > 10)
                throw new InvalidOperationException("Code must be 10 characters or less");
            if (roomName.Length > 100)
                throw new InvalidOperationException("Room Name must be 100 characters or less");

            await RoomV3AccessHelpers.EnsureRoomTypeAsync(_context, roomTypeId, cancellationToken);
            await RoomV3AccessHelpers.EnsureBuildingAsync(_context, clientId, entityId, buildingId, cancellationToken);
            await RoomV3AccessHelpers.EnsureFloorAsync(_context, clientId, entityId, floorId, buildingId, cancellationToken);

            var room = await _context.OrgRoomLegacy
                .FirstOrDefaultAsync(r => r.Id == id && r.clientid == clientId && r.entityid == entityId && !r.utblock, cancellationToken);
            if (room == null)
                throw new KeyNotFoundException($"Room with ID {id} not found");

            var codeExists = await _context.OrgRoomLegacy
                .AsNoTracking()
                .AnyAsync(r =>
                    r.Id != id
                    && r.clientid == clientId
                    && r.entityid == entityId
                    && !r.utblock
                    && r.code != null
                    && (r.code ?? "").Trim().ToUpper() == code.ToUpper(), cancellationToken);
            if (codeExists)
                throw new InvalidOperationException($"Code : {code} already exists");

            var duplicateRoom = await _context.OrgRoomLegacy
                .AsNoTracking()
                .AnyAsync(r =>
                    r.Id != id
                    && r.clientid == clientId
                    && r.entityid == entityId
                    && !r.utblock
                    && r.buildingid == buildingId
                    && r.lovfloorid == floorId
                    && (r.stxt ?? "").Trim().ToUpper() == roomName.ToUpper(), cancellationToken);
            if (duplicateRoom)
                throw new InvalidOperationException($"Room : {roomName} already exists against Building and Floor");

            room.code = code;
            room.stxt = roomName;
            room.roomtypeid = roomTypeId;
            room.buildingid = buildingId;
            room.lovfloorid = floorId;
            room.strength = request.Strength;

            _context.OrgRoomLegacy.Update(room);
            await _context.SaveChangesAsync(cancellationToken);

            return await RoomV3Mapper.MapByIdAsync(_context, id, clientId, entityId, cancellationToken);
        }
    }

    public sealed class DeleteRoomV3CommandHandler : IRequestHandler<DeleteRoomV3Command, bool>
    {
        private readonly OrganizationDbContext _context;

        public DeleteRoomV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteRoomV3Command request, CancellationToken cancellationToken)
        {
            var id = RoomV3AccessHelpers.EnsurePositive(request.Id, "Id");
            var clientId = RoomV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = RoomV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");

            var room = await _context.OrgRoomLegacy
                .FirstOrDefaultAsync(r => r.Id == id && r.clientid == clientId && r.entityid == entityId && !r.utblock, cancellationToken);
            if (room == null)
                throw new KeyNotFoundException($"Room with ID {id} not found");

            room.utblock = true;
            room.utrecordversion += 1;
            _context.OrgRoomLegacy.Update(room);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
