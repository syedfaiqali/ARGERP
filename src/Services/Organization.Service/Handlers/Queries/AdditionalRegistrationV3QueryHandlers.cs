using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.DTOs;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Queries;

namespace Organization.Service.Handlers.Queries
{
    public sealed class GetAdditionalRegistrationsV3QueryHandler
        : IRequestHandler<GetAdditionalRegistrationsV3Query, List<AdditionalRegistrationV3Dto>>
    {
        private readonly OrganizationDbContext _context;

        public GetAdditionalRegistrationsV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdditionalRegistrationV3Dto>> Handle(GetAdditionalRegistrationsV3Query request, CancellationToken cancellationToken)
        {
            var clientId = RoomV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = request.EntityId;
            var infoForId = RoomV3AccessHelpers.EnsurePositive(request.InfoForId, "InfoForId");

            var rows = await (
                from cfg in _context.SMAdditionalinfoconfigLegacy.AsNoTracking()
                where !cfg.utblock && cfg.infoforid == infoForId
                join reg in _context.SMAdditionalregistrationLegacy.AsNoTracking()
                    .Where(r =>
                        entityId > 0 &&
                        !r.utblock &&
                        r.clientid == clientId &&
                        r.entityid == entityId &&
                        r.infoforid == infoForId)
                    on cfg.Id equals reg.configid into regs
                from reg in regs.DefaultIfEmpty()
                select new AdditionalRegistrationV3Dto
                {
                    Id = reg != null ? reg.Id : 0,
                    ConfigId = cfg.Id,
                    Code = cfg.code ?? string.Empty,
                    SectionName = cfg.stxt ?? string.Empty,
                    Caption = reg != null && !string.IsNullOrWhiteSpace(reg.caption)
                        ? reg.caption!
                        : (cfg.stxt ?? string.Empty),
                    InfoForId = cfg.infoforid ?? infoForId,
                    Sequance = reg != null ? reg.sequance : null,
                    Visible = reg != null ? reg.hide : false,
                    Route = cfg.route ?? string.Empty,
                    Icon = cfg.webicon ?? string.Empty,
                    CssClass = cfg._class ?? string.Empty,
                })
                .OrderBy(x => x.Code)
                .ThenBy(x => x.ConfigId)
                .ToListAsync(cancellationToken);

            return rows;
        }
    }

    public sealed class GetAdditionalRegistrationByIdV3QueryHandler
        : IRequestHandler<GetAdditionalRegistrationByIdV3Query, AdditionalRegistrationV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public GetAdditionalRegistrationByIdV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<AdditionalRegistrationV3Dto> Handle(GetAdditionalRegistrationByIdV3Query request, CancellationToken cancellationToken)
        {
            var clientId = RoomV3AccessHelpers.EnsurePositive(request.ClientId, "ClientId");
            var entityId = RoomV3AccessHelpers.EnsurePositive(request.EntityId, "EntityId");
            var id = RoomV3AccessHelpers.EnsurePositive(request.Id, "Id");

            var item = await (
                from reg in _context.SMAdditionalregistrationLegacy.AsNoTracking()
                where reg.Id == id && !reg.utblock && reg.clientid == clientId && reg.entityid == entityId
                join cfg in _context.SMAdditionalinfoconfigLegacy.AsNoTracking()
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

            if (item == null)
                throw new KeyNotFoundException($"Additional registration with ID {id} not found");

            return item;
        }
    }

    public sealed class GetAdditionalRegistrationInfoForOptionsV3QueryHandler
        : IRequestHandler<GetAdditionalRegistrationInfoForOptionsV3Query, List<AdditionalRegistrationInfoForOptionV3Dto>>
    {
        private readonly OrganizationDbContext _context;

        public GetAdditionalRegistrationInfoForOptionsV3QueryHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AdditionalRegistrationInfoForOptionV3Dto>> Handle(
            GetAdditionalRegistrationInfoForOptionsV3Query request,
            CancellationToken cancellationToken)
        {
            return await _context.Lovs
                .AsNoTracking()
                .Where(x => x.vtype == "GSTYP" && !x.utblock)
                .OrderBy(x => x.indx ?? int.MaxValue)
                .ThenBy(x => x.Id)
                .Select(x => new
                {
                    x.Id,
                    x.code,
                    x.stxt
                })
                .Select(x => new AdditionalRegistrationInfoForOptionV3Dto
                {
                    Id = x.Id,
                    Code = x.code ?? string.Empty,
                    Name = x.stxt ?? x.Id.ToString()
                })
                .ToListAsync(cancellationToken);
        }
    }
}
