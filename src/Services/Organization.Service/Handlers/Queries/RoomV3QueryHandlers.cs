using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.DTOs;
using Organization.Service.Handlers;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Queries;

namespace Organization.Service.Handlers.Queries
{
    public sealed class GetRoomsV3QueryHandler : IRequestHandler<GetRoomsV3Query, PaginationResponse<RoomV3Dto>>
    {
        private readonly OrganizationDbContext _context;

        public GetRoomsV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<RoomV3Dto>> Handle(GetRoomsV3Query request, CancellationToken cancellationToken)
        {
            var clientId = RoomV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = RoomV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");

            var query = _context.OrgRoomLegacy
                .AsNoTracking()
                .Where(r => r.clientid == clientId && r.entityid == entityId && !r.utblock)
                .AsQueryable();

            if (request.BuildingId.HasValue && request.BuildingId.Value > 0)
                query = query.Where(r => r.buildingid == request.BuildingId.Value);

            if (request.FloorId.HasValue && request.FloorId.Value > 0)
                query = query.Where(r => r.lovfloorid == request.FloorId.Value);

            if (request.RoomTypeId.HasValue && request.RoomTypeId.Value > 0)
                query = query.Where(r => r.roomtypeid == request.RoomTypeId.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.Trim().ToLower();
                query = query.Where(r =>
                    (r.code != null && r.code.ToLower().Contains(search))
                    || (r.stxt != null && r.stxt.ToLower().Contains(search)));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var ids = await query
                .OrderByDescending(r => r.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(r => r.Id)
                .ToListAsync(cancellationToken);

            var items = new List<RoomV3Dto>(ids.Count);
            foreach (var id in ids)
            {
                items.Add(await RoomV3Mapper.MapByIdAsync(_context, id, clientId, entityId, cancellationToken));
            }

            return new PaginationResponse<RoomV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            };
        }
    }

    public sealed class GetRoomByIdV3QueryHandler : IRequestHandler<GetRoomByIdV3Query, RoomV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public GetRoomByIdV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public Task<RoomV3Dto> Handle(GetRoomByIdV3Query request, CancellationToken cancellationToken)
        {
            var clientId = RoomV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = RoomV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");
            var id = RoomV3AccessHelpers.EnsurePositive(request.Id, "Id");

            return RoomV3Mapper.MapByIdAsync(_context, id, clientId, entityId, cancellationToken);
        }
    }

    public sealed class GetRoomTypesV3QueryHandler : IRequestHandler<GetRoomTypesV3Query, List<RoomLookupDto>>
    {
        private readonly OrganizationDbContext _context;

        public GetRoomTypesV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RoomLookupDto>> Handle(GetRoomTypesV3Query request, CancellationToken cancellationToken)
        {
            return await _context.Lovs
                .AsNoTracking()
                .Where(l => l.vtype != null && l.vtype.ToUpper() == "RMTP")
                .OrderBy(l => l.stxt)
                .Select(l => new RoomLookupDto
                {
                    Id = l.Id,
                    Code = l.code ?? string.Empty,
                    Name = l.stxt ?? l.ltxt ?? string.Empty,
                })
                .ToListAsync(cancellationToken);
        }
    }

    public sealed class GetRoomBuildingsV3QueryHandler : IRequestHandler<GetRoomBuildingsV3Query, List<RoomLookupDto>>
    {
        private readonly OrganizationDbContext _context;

        public GetRoomBuildingsV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RoomLookupDto>> Handle(GetRoomBuildingsV3Query request, CancellationToken cancellationToken)
        {
            var clientId = RoomV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = RoomV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");

            return await _context.OrgBuildingLegacy
                .AsNoTracking()
                .Where(b => b.clientid == clientId && b.entityid == entityId && !b.utblock)
                .OrderBy(b => b.stxt)
                .Select(b => new RoomLookupDto
                {
                    Id = b.Id,
                    Code = b.code ?? string.Empty,
                    Name = b.stxt ?? string.Empty,
                })
                .ToListAsync(cancellationToken);
        }
    }

    public sealed class GetRoomFloorsV3QueryHandler : IRequestHandler<GetRoomFloorsV3Query, List<RoomLookupDto>>
    {
        private readonly OrganizationDbContext _context;

        public GetRoomFloorsV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<List<RoomLookupDto>> Handle(GetRoomFloorsV3Query request, CancellationToken cancellationToken)
        {
            var clientId = RoomV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = RoomV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");

            var query = _context.OrgFloorLegacy
                .AsNoTracking()
                .Where(f => f.clientid == clientId && f.entityid == entityId && !f.utblock);

            if (request.BuildingId.HasValue && request.BuildingId.Value > 0)
                query = query.Where(f => f.buildingid == request.BuildingId.Value);

            return await query
                .OrderBy(f => f.stxt)
                .Select(f => new RoomLookupDto
                {
                    Id = f.Id,
                    Code = f.code ?? string.Empty,
                    Name = f.stxt ?? string.Empty,
                })
                .ToListAsync(cancellationToken);
        }
    }

    internal static class RoomV3Mapper
    {
        internal static async Task<RoomV3Dto> MapByIdAsync(
            OrganizationDbContext context,
            int id,
            int clientId,
            int entityId,
            CancellationToken cancellationToken)
        {
            var model = await context.OrgRoomLegacy
                .AsNoTracking()
                .Where(r => r.Id == id && r.clientid == clientId && r.entityid == entityId && !r.utblock)
                .Select(r => new
                {
                    room = r,
                    roomTypeName = context.Lovs
                        .Where(l => l.Id == r.roomtypeid)
                        .Select(l => l.stxt)
                        .FirstOrDefault(),
                    buildingName = context.OrgBuildingLegacy
                        .Where(b => b.Id == r.buildingid && b.clientid == clientId && b.entityid == entityId && !b.utblock)
                        .Select(b => b.stxt)
                        .FirstOrDefault(),
                    floorName = context.OrgFloorLegacy
                        .Where(f => f.Id == r.lovfloorid && f.clientid == clientId && f.entityid == entityId && !f.utblock)
                        .Select(f => f.stxt)
                        .FirstOrDefault(),
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (model == null)
                throw new KeyNotFoundException($"Room with ID {id} not found");

            return new RoomV3Dto
            {
                Id = model.room.Id,
                Code = model.room.code ?? string.Empty,
                RoomName = model.room.stxt ?? string.Empty,
                RoomTypeId = model.room.roomtypeid ?? 0,
                RoomTypeName = model.roomTypeName ?? string.Empty,
                BuildingId = model.room.buildingid ?? 0,
                BuildingName = model.buildingName ?? string.Empty,
                FloorId = model.room.lovfloorid ?? 0,
                FloorName = model.floorName ?? string.Empty,
                Strength = model.room.strength,
            };
        }
    }
}
