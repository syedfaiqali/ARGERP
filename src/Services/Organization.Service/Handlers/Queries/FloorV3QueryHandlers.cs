using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.DTOs;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Queries;

namespace Organization.Service.Handlers.Queries
{
    public sealed class GetFloorsV3QueryHandler : IRequestHandler<GetFloorsV3Query, PaginationResponse<FloorV3Dto>>
    {
        private readonly OrganizationDbContext _context;

        public GetFloorsV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<FloorV3Dto>> Handle(GetFloorsV3Query request, CancellationToken cancellationToken)
        {
            var clientId = RoomV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = RoomV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");

            var query = _context.OrgFloorLegacy
                .AsNoTracking()
                .Where(f => f.clientid == clientId && f.entityid == entityId && !f.utblock)
                .AsQueryable();

            if (request.BuildingId.HasValue && request.BuildingId.Value > 0)
                query = query.Where(f => f.buildingid == request.BuildingId.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.Trim().ToLower();
                query = query.Where(f =>
                    (f.code != null && f.code.ToLower().Contains(search))
                    || (f.stxt != null && f.stxt.ToLower().Contains(search)));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(f => f.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(f => new FloorV3Dto
                {
                    Id = f.Id,
                    Code = f.code ?? string.Empty,
                    FloorName = f.stxt ?? string.Empty,
                    BuildingId = f.buildingid ?? 0,
                    BuildingName = _context.OrgBuildingLegacy
                        .Where(b => b.Id == f.buildingid && b.clientid == clientId && b.entityid == entityId && !b.utblock)
                        .Select(b => b.stxt)
                        .FirstOrDefault() ?? string.Empty,
                    ClientId = f.clientid,
                    EntityId = f.entityid
                })
                .ToListAsync(cancellationToken);

            return new PaginationResponse<FloorV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            };
        }
    }

    public sealed class GetFloorByIdV3QueryHandler : IRequestHandler<GetFloorByIdV3Query, FloorV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public GetFloorByIdV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<FloorV3Dto> Handle(GetFloorByIdV3Query request, CancellationToken cancellationToken)
        {
            var clientId = RoomV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = RoomV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");
            var id = RoomV3AccessHelpers.EnsurePositive(request.Id, "Id");

            var f = await _context.OrgFloorLegacy
                .AsNoTracking()
                .Where(x => x.Id == id && x.clientid == clientId && x.entityid == entityId && !x.utblock)
                .Select(f => new FloorV3Dto
                {
                    Id = f.Id,
                    Code = f.code ?? string.Empty,
                    FloorName = f.stxt ?? string.Empty,
                    BuildingId = f.buildingid ?? 0,
                    BuildingName = _context.OrgBuildingLegacy
                        .Where(b => b.Id == f.buildingid && b.clientid == clientId && b.entityid == entityId && !b.utblock)
                        .Select(b => b.stxt)
                        .FirstOrDefault() ?? string.Empty,
                    ClientId = f.clientid,
                    EntityId = f.entityid
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (f == null)
                throw new KeyNotFoundException($"Floor with ID {id} not found");

            return f;
        }
    }
}
