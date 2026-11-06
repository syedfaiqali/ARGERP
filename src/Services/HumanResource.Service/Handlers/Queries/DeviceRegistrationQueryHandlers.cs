using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class DeviceRegistrationQueryHandlers :
        IRequestHandler<GetDeviceRegistrationLookupsV3Query, DeviceRegistrationLookupsV3Dto>,
        IRequestHandler<GetDeviceRegistrationsV3Query, PaginationResponse<DeviceRegistrationV3Dto>>,
        IRequestHandler<GetDeviceRegistrationByIdV3Query, DeviceRegistrationV3Dto?>
    {
        private const string ConnectivityTypeVType = "CNVTP";
        private readonly HumanResourceDbContext _context;

        public DeviceRegistrationQueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<DeviceRegistrationLookupsV3Dto> Handle(
            GetDeviceRegistrationLookupsV3Query request,
            CancellationToken cancellationToken
        )
        {
            var deviceTypes = await _context.DeviceTypes
                .AsNoTracking()
                .Where(x => x.UtBlock != true && (x.ClientId == null || x.ClientId == 0 || x.ClientId == request.ClientId))
                .OrderBy(x => x.Stxt)
                .Select(x => new DeviceRegistrationLookupItemV3Dto
                {
                    Id = x.Id,
                    Code = x.Code ?? string.Empty,
                    Name = x.Stxt ?? string.Empty,
                    Description = x.Ltxt ?? string.Empty
                })
                .ToListAsync(cancellationToken);

            var sites = await _context.Sites
                .AsNoTracking()
                .Where(x =>
                    x.utblock != true
                    && (x.clientid == null || x.clientid == 0 || x.clientid == request.ClientId)
                    && (request.EntityId == null || request.EntityId == 0 || x.entityid == null || x.entityid == 0 || x.entityid == request.EntityId))
                .OrderBy(x => x.stxt)
                .Select(x => new DeviceRegistrationLookupItemV3Dto
                {
                    Id = x.Id,
                    Code = x.code ?? string.Empty,
                    Name = x.stxt ?? string.Empty,
                    Description = string.Empty
                })
                .ToListAsync(cancellationToken);

            var connectivityTypes = await _context.Lovs
                .AsNoTracking()
                .Where(x => x.UtBlock != true && x.Vtype == ConnectivityTypeVType)
                .OrderBy(x => x.Stxt)
                .Select(x => new DeviceRegistrationLookupItemV3Dto
                {
                    Id = x.Id,
                    Code = x.Code ?? string.Empty,
                    Name = x.Stxt ?? string.Empty,
                    Description = x.Ltxt ?? string.Empty
                })
                .ToListAsync(cancellationToken);

            return new DeviceRegistrationLookupsV3Dto
            {
                DeviceTypes = deviceTypes,
                Sites = sites,
                ConnectivityTypes = connectivityTypes
            };
        }

        public async Task<PaginationResponse<DeviceRegistrationV3Dto>> Handle(
            GetDeviceRegistrationsV3Query request,
            CancellationToken cancellationToken
        )
        {
            var query = BuildQuery(request.ClientId, request.EntityId);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                query = query.Where(x =>
                    x.Code.ToLower().Contains(term) ||
                    x.DeviceName.ToLower().Contains(term) ||
                    x.SerialNumber.ToLower().Contains(term) ||
                    x.DeviceTypeName.ToLower().Contains(term) ||
                    x.SiteName.ToLower().Contains(term) ||
                    x.DeviceIp.ToLower().Contains(term)
                );
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(x => x.Code)
                .ThenBy(x => x.DeviceName)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            return new PaginationResponse<DeviceRegistrationV3Dto>(items, totalCount, request.PageNumber, request.PageSize);
        }

        public async Task<DeviceRegistrationV3Dto?> Handle(
            GetDeviceRegistrationByIdV3Query request,
            CancellationToken cancellationToken
        )
        {
            return await BuildQuery(request.ClientId, request.EntityId)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        }

        private IQueryable<DeviceRegistrationV3Dto> BuildQuery(int clientId, int? entityId)
        {
            return
                from m in _context.MachineRegistrations.AsNoTracking()
                join d in _context.DeviceTypes.AsNoTracking() on m.devicetypeid equals d.Id into dLft
                from d in dLft.DefaultIfEmpty()
                join s in _context.Sites.AsNoTracking() on m.siteid equals s.Id into sLft
                from s in sLft.DefaultIfEmpty()
                join c in _context.Lovs.AsNoTracking() on m.convtytypeid equals c.Id into cLft
                from c in cLft.DefaultIfEmpty()
                where
                    m.utblock != true
                    && m.inactive != true
                    && (m.clientid == null || m.clientid == 0 || m.clientid == clientId)
                    && (entityId == null || entityId == 0 || m.entityid == null || m.entityid == 0 || m.entityid == entityId)
                select new DeviceRegistrationV3Dto
                {
                    Id = m.Id,
                    Code = m.code ?? string.Empty,
                    DeviceTypeId = m.devicetypeid ?? 0,
                    DeviceTypeName = d != null ? d.Stxt ?? string.Empty : string.Empty,
                    DeviceName = m.stxt ?? string.Empty,
                    SerialNumber = m.ltxt ?? string.Empty,
                    SiteId = m.siteid ?? 0,
                    SiteName = s != null ? s.stxt ?? string.Empty : string.Empty,
                    DeviceIp = m.machineip ?? string.Empty,
                    MachinePort = m.port ?? 0,
                    ConnectivityTypeId = m.convtytypeid ?? 0,
                    ConnectivityTypeName = c != null ? c.Stxt ?? string.Empty : string.Empty,
                    DeviceId = m.deviceid,
                    IsActive = !m.inactive
                };
        }
    }
}
