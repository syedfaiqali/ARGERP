using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.DTOs;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Queries;

namespace Organization.Service.Handlers.Queries
{
    public sealed class GetBuildingsV3QueryHandler : IRequestHandler<GetBuildingsV3Query, PaginationResponse<BuildingV3Dto>>
    {
        private readonly OrganizationDbContext _context;

        public GetBuildingsV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<BuildingV3Dto>> Handle(GetBuildingsV3Query request, CancellationToken cancellationToken)
        {
            var clientId = RoomV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = RoomV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");

            var query = _context.OrgBuildingLegacy
                .AsNoTracking()
                .Where(b => b.clientid == clientId && b.entityid == entityId && !b.utblock)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var search = request.SearchTerm.Trim().ToLower();
                query = query.Where(b =>
                    (b.code != null && b.code.ToLower().Contains(search))
                    || (b.stxt != null && b.stxt.ToLower().Contains(search)));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(b => b.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(b => new BuildingV3Dto
                {
                    Id = b.Id,
                    Code = b.code ?? string.Empty,
                    BuildingName = b.stxt ?? string.Empty,
                    ClientId = b.clientid,
                    EntityId = b.entityid
                })
                .ToListAsync(cancellationToken);

            return new PaginationResponse<BuildingV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
            };
        }
    }

    public sealed class GetBuildingByIdV3QueryHandler : IRequestHandler<GetBuildingByIdV3Query, BuildingV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public GetBuildingByIdV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<BuildingV3Dto> Handle(GetBuildingByIdV3Query request, CancellationToken cancellationToken)
        {
            var clientId = RoomV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = RoomV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");
            var id = RoomV3AccessHelpers.EnsurePositive(request.Id, "Id");

            var b = await _context.OrgBuildingLegacy
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.clientid == clientId && x.entityid == entityId && !x.utblock, cancellationToken);

            if (b == null)
                throw new KeyNotFoundException($"Building with ID {id} not found");

            return new BuildingV3Dto
            {
                Id = b.Id,
                Code = b.code ?? string.Empty,
                BuildingName = b.stxt ?? string.Empty,
                ClientId = b.clientid,
                EntityId = b.entityid
            };
        }
    }
}
