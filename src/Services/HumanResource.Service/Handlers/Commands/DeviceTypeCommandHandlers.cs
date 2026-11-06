using MediatR;
using Microsoft.EntityFrameworkCore;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Commands;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class DeviceTypeCommandHandlers :
        IRequestHandler<CreateDeviceTypeV3Command, DeviceTypeV3Dto>,
        IRequestHandler<UpdateDeviceTypeV3Command, bool>,
        IRequestHandler<DeleteDeviceTypeV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;

        public DeviceTypeCommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<DeviceTypeV3Dto> Handle(CreateDeviceTypeV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new InvalidOperationException("Device Type is required.");
            }

            var nameExists = await _context.DeviceTypes
                .AnyAsync(x => x.Stxt == request.Name.Trim() && x.ClientId == command.ClientId && !x.UtBlock, cancellationToken);
            if (nameExists)
            {
                throw new InvalidOperationException("Device Type already exists.");
            }

            var nextId = (await _context.DeviceTypes.IgnoreQueryFilters()
                .Select(x => (int?)x.Id)
                .MaxAsync(cancellationToken) ?? 0) + 1;

            var entity = new SMDevicetypeLegacy
            {
                Id = nextId,
                Code = string.IsNullOrWhiteSpace(request.Code) ? null : request.Code.Trim(),
                Stxt = request.Name.Trim(),
                Ltxt = request.Description?.Trim() ?? string.Empty,
                ClientId = command.ClientId,
                UtSysGen = false,
                UtIsPartial = false,
                UtRecordVersion = 0,
                UtBlock = !request.IsActive
            };

            _context.DeviceTypes.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new DeviceTypeV3Dto(
                entity.Id,
                entity.Code ?? string.Empty,
                entity.Stxt ?? string.Empty,
                entity.Ltxt,
                !entity.UtBlock
            );
        }

        public async Task<bool> Handle(UpdateDeviceTypeV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var entity = await _context.DeviceTypes
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.ClientId == command.ClientId, cancellationToken);

            if (entity == null) return false;

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                throw new InvalidOperationException("Device Type is required.");
            }

            var nameExists = await _context.DeviceTypes
                .AnyAsync(x => x.Stxt == request.Name.Trim() && x.ClientId == command.ClientId && x.Id != request.Id && !x.UtBlock, cancellationToken);
            if (nameExists)
            {
                throw new InvalidOperationException("Device Type already exists.");
            }

            entity.Code = string.IsNullOrWhiteSpace(request.Code) ? null : request.Code.Trim();
            entity.Stxt = request.Name.Trim();
            entity.Ltxt = request.Description?.Trim() ?? string.Empty;
            entity.UtRecordVersion += 1;
            entity.UtIsPartial = false;
            entity.UtSysGen = false;
            entity.UtBlock = !request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Handle(DeleteDeviceTypeV3Command command, CancellationToken cancellationToken)
        {
            var entity = await _context.DeviceTypes
                .FirstOrDefaultAsync(x => x.Id == command.Id && x.ClientId == command.ClientId && !x.UtBlock, cancellationToken);

            if (entity == null) return false;

            entity.UtBlock = true;
            entity.UtRecordVersion += 1;
            entity.UtIsPartial = false;
            entity.UtSysGen = false;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
