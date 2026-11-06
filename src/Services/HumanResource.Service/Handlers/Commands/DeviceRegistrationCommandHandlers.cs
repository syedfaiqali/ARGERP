using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class DeviceRegistrationCommandHandlers :
        IRequestHandler<CreateDeviceRegistrationV3Command, DeviceRegistrationV3Dto>,
        IRequestHandler<UpdateDeviceRegistrationV3Command, DeviceRegistrationV3Dto?>,
        IRequestHandler<DeleteDeviceRegistrationV3Command, bool>
    {
        private const string ConnectivityTypeVType = "CNVTP";
        private readonly HumanResourceDbContext _context;

        public DeviceRegistrationCommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<DeviceRegistrationV3Dto> Handle(
            CreateDeviceRegistrationV3Command request,
            CancellationToken cancellationToken
        )
        {
            var normalized = await NormalizeAndValidate(
                request.Request,
                request.ClientId,
                request.EntityId,
                null,
                cancellationToken
            );

            var nextId = (await _context.MachineRegistrations.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            var entity = new SMMachineRegistrationLegacy
            {
                Id = nextId,
                code = normalized.Code,
                stxt = normalized.DeviceName,
                machineip = normalized.DeviceIp,
                port = normalized.MachinePort,
                lovmachinetypeid = normalized.lovmachinetypeid,
                devicetypeid = normalized.DeviceTypeId,
                siteid = normalized.SiteId,
                convtytypeid = normalized.ConnectivityTypeId,
                ltxt = normalized.SerialNumber,
                deviceid = normalized.DeviceId,
                inactive = !normalized.IsActive,
                entityid = request.EntityId.HasValue && request.EntityId.Value > 0 ? request.EntityId : null,
                clientid = request.ClientId,
                utsysgen = false,
                utblock = false,
                utispartial = false,
                utrecordversion = 1
            };

            _context.MachineRegistrations.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return await BuildQuery(request.ClientId, request.EntityId)
                .FirstAsync(x => x.Id == entity.Id, cancellationToken);
        }

        public async Task<DeviceRegistrationV3Dto?> Handle(
            UpdateDeviceRegistrationV3Command request,
            CancellationToken cancellationToken
        )
        {
            var entity = await _context.MachineRegistrations.FirstOrDefaultAsync(x =>
                x.Id == request.Id
                && x.utblock != true
                && (x.clientid == null || x.clientid == 0 || x.clientid == request.ClientId)
                && (x.entityid == null || x.entityid == 0 || x.entityid == request.EntityId),
                cancellationToken);
            if (entity == null)
            {
                return null;
            }

            var normalized = await NormalizeAndValidate(
                request.Request,
                request.ClientId,
                request.EntityId,
                request.Id,
                cancellationToken
            );

            entity.code = normalized.Code;
            entity.stxt = normalized.DeviceName;
            entity.machineip = normalized.DeviceIp;
            entity.port = normalized.MachinePort;
            entity.lovmachinetypeid = normalized.lovmachinetypeid;
            entity.devicetypeid = normalized.DeviceTypeId;
            entity.siteid = normalized.SiteId;
            entity.convtytypeid = normalized.ConnectivityTypeId;
            entity.ltxt = normalized.SerialNumber;
            entity.deviceid = normalized.DeviceId;
            entity.inactive = !normalized.IsActive;
            entity.clientid = request.ClientId;
            entity.entityid = request.EntityId.HasValue && request.EntityId.Value > 0 ? request.EntityId : null;
            entity.utblock = false;
            entity.utrecordversion = (entity.utrecordversion <= 0 ? 0 : entity.utrecordversion) + 1;

            await _context.SaveChangesAsync(cancellationToken);

            return await BuildQuery(request.ClientId, request.EntityId)
                .FirstOrDefaultAsync(x => x.Id == entity.Id, cancellationToken);
        }

        public async Task<bool> Handle(
            DeleteDeviceRegistrationV3Command request,
            CancellationToken cancellationToken
        )
        {
            var entity = await _context.MachineRegistrations.FirstOrDefaultAsync(x =>
                x.Id == request.Id
                && x.utblock != true
                && (x.clientid == null || x.clientid == 0 || x.clientid == request.ClientId)
                && (x.entityid == null || x.entityid == 0 || x.entityid == request.EntityId),
                cancellationToken);
            if (entity == null)
            {
                return false;
            }

            entity.utblock = true;
            entity.inactive = true;
            entity.utrecordversion = (entity.utrecordversion <= 0 ? 0 : entity.utrecordversion) + 1;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private async Task<NormalizedMachineRegistration> NormalizeAndValidate(
            DeviceRegistrationUpsertV3Request request,
            int clientId,
            int? entityId,
            int? currentId,
            CancellationToken cancellationToken
        )
        {
            var code = (request.Code ?? string.Empty).Trim();
            var deviceName = (request.DeviceName ?? string.Empty).Trim();
            var deviceIp = (request.DeviceIp ?? string.Empty).Trim();
            var normalizedEntityId = entityId.HasValue && entityId.Value > 0 ? entityId : null;

            if (string.IsNullOrWhiteSpace(code))
                throw new InvalidOperationException("Code is required.");
            if (request.DeviceTypeId <= 0)
                throw new InvalidOperationException("Device Type is required.");
            if (string.IsNullOrWhiteSpace(deviceName))
                throw new InvalidOperationException("Device Name is required.");
            if (request.SiteId <= 0)
                throw new InvalidOperationException("Site is required.");
            if (string.IsNullOrWhiteSpace(deviceIp))
                throw new InvalidOperationException("Device Ip is required.");
            if (request.MachinePort <= 0)
                throw new InvalidOperationException("Machine Port must be greater than Zero(0).");
            if (request.ConnectivityTypeId <= 0)
                throw new InvalidOperationException("Connectivity Type is required.");

            if (normalizedEntityId.HasValue)
            {
                var legalEntityExists = await _context.OrgLegalentities
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.Id == normalizedEntityId.Value
                        && x.ClientId == clientId
                        && !x.UtBlock,
                        cancellationToken);
                if (!legalEntityExists)
                    throw new InvalidOperationException("EntityId is invalid.");
            }

            var deviceTypeExists = await _context.DeviceTypes
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id == request.DeviceTypeId
                    && x.UtBlock != true
                    && (x.ClientId == null || x.ClientId == 0 || x.ClientId == clientId),
                    cancellationToken);
            if (!deviceTypeExists)
                throw new InvalidOperationException("Device Type is invalid.");

            var siteExists = await _context.Sites
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id == request.SiteId
                    && x.utblock != true
                    && (x.clientid == null || x.clientid == 0 || x.clientid == clientId)
                    && (normalizedEntityId == null || x.entityid == null || x.entityid == 0 || x.entityid == normalizedEntityId),
                    cancellationToken);
            if (!siteExists)
                throw new InvalidOperationException("Site is invalid.");

            var connectivityTypeExists = await _context.Lovs
                .AsNoTracking()
                .AnyAsync(x =>
                    x.Id == request.ConnectivityTypeId
                    && x.UtBlock != true
                    && x.Vtype == ConnectivityTypeVType,
                    cancellationToken);
            if (!connectivityTypeExists)
                throw new InvalidOperationException("Connectivity Type is invalid.");

            if (!string.IsNullOrWhiteSpace(deviceIp))
            {
                var duplicateIpExists = await _context.MachineRegistrations
                    .AsNoTracking()
                    .AnyAsync(x =>
                        x.utblock != true
                        && (x.clientid == null || x.clientid == 0 || x.clientid == clientId)
                        && (normalizedEntityId == null || x.entityid == null || x.entityid == 0 || x.entityid == normalizedEntityId)
                        && x.machineip == deviceIp
                        && (!currentId.HasValue || x.Id != currentId.Value),
                        cancellationToken);
                if (duplicateIpExists)
                    throw new InvalidOperationException("Machine IP already exists.");
            }

            var duplicateNameExists = await _context.MachineRegistrations
                .AsNoTracking()
                .AnyAsync(x =>
                    x.utblock != true
                    && (x.clientid == null || x.clientid == 0 || x.clientid == clientId)
                    && (normalizedEntityId == null || x.entityid == null || x.entityid == 0 || x.entityid == normalizedEntityId)
                    && x.devicetypeid == request.DeviceTypeId
                    && x.siteid == request.SiteId
                    && x.stxt == deviceName
                    && (!currentId.HasValue || x.Id != currentId.Value),
                    cancellationToken);
            if (duplicateNameExists)
                throw new InvalidOperationException("Device Name already exists against Device Type and Site.");

            return new NormalizedMachineRegistration
            {
                Code = code,
                DeviceTypeId = request.DeviceTypeId,
                DeviceName = deviceName,
                SiteId = request.SiteId,
                DeviceIp = deviceIp,
                MachinePort = request.MachinePort,
                ConnectivityTypeId = request.ConnectivityTypeId,
                SerialNumber = (request.SerialNumber ?? string.Empty).Trim(),
                DeviceId = request.DeviceId,
                IsActive = request.IsActive,
                lovmachinetypeid = null // or some default if needed
            };
        }

        private sealed class NormalizedMachineRegistration
        {
            public string Code { get; set; } = string.Empty;
            public int DeviceTypeId { get; set; }
            public string DeviceName { get; set; } = string.Empty;
            public int SiteId { get; set; }
            public string DeviceIp { get; set; } = string.Empty;
            public int MachinePort { get; set; }
            public int ConnectivityTypeId { get; set; }
            public string SerialNumber { get; set; } = string.Empty;
            public int? DeviceId { get; set; }
            public bool IsActive { get; set; }
            public int? lovmachinetypeid { get; set; }
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
