using MediatR;
using Microsoft.EntityFrameworkCore;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Commands;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class HolidayTypeCommandHandlers :
        IRequestHandler<CreateHolidayTypeV3Command, HolidayTypeV3Dto>,
        IRequestHandler<UpdateHolidayTypeV3Command, bool>,
        IRequestHandler<DeleteHolidayTypeV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;

        public HolidayTypeCommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<HolidayTypeV3Dto> Handle(CreateHolidayTypeV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            if (await _context.HolidayTypes.AnyAsync(x => x.Stxt == request.HolidayType && x.EntityId != null && (x.EntityId == command.EntityId || !command.EntityId.HasValue) && x.ClientId == command.ClientId, cancellationToken))
            {
                throw new InvalidOperationException($"Holiday Type with name '{request.HolidayType}' already exists.");
            }

            var nextId = (await _context.HolidayTypes.IgnoreQueryFilters()
                .MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            var code = request.Code?.Trim();
            if (string.IsNullOrWhiteSpace(code))
            {
                // Imported/manual codes may already occupy the next generated number.
                var nextCode = nextId;
                do { code = (nextCode++).ToString("0000"); }
                while (await CodeExistsAsync(code, command.ClientId, command.EntityId, null, cancellationToken));
            }
            else if (await CodeExistsAsync(code, command.ClientId, command.EntityId, null, cancellationToken))
            {
                throw new InvalidOperationException($"Holiday Type with code '{code}' already exists.");
            }

            var entity = new ETHolidaytypeLegacy
            {
                Id = nextId,
                Code = code,
                Stxt = request.HolidayType.Trim(),
                Ltxt = request.Description?.Trim(),
                Colorcode = request.ColorCode?.Trim(),
                EntityId = command.EntityId,
                ClientId = command.ClientId,
                UtBlock = !request.IsActive
            };

            _context.HolidayTypes.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new HolidayTypeV3Dto(entity.Id, entity.Code ?? string.Empty, entity.Stxt ?? string.Empty, entity.Ltxt, entity.Colorcode, !(entity.UtBlock ?? false));
        }

        public async Task<bool> Handle(UpdateHolidayTypeV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var entity = await _context.HolidayTypes
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.ClientId == command.ClientId && x.EntityId != null && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value), cancellationToken);

            if (entity == null) return false;

            if (await _context.HolidayTypes.AnyAsync(x => x.Stxt == request.HolidayType && x.EntityId != null && (x.EntityId == command.EntityId || !command.EntityId.HasValue) && x.ClientId == command.ClientId && x.Id != request.Id, cancellationToken))
            {
                throw new InvalidOperationException($"Another holiday type with name '{request.HolidayType}' already exists.");
            }

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                if (await CodeExistsAsync(request.Code.Trim(), command.ClientId, command.EntityId, request.Id, cancellationToken))
                {
                    throw new InvalidOperationException($"Holiday Type with code '{request.Code.Trim()}' already exists.");
                }
                entity.Code = request.Code.Trim();
            }
            entity.Stxt = request.HolidayType.Trim();
            entity.Ltxt = request.Description?.Trim();
            entity.Colorcode = request.ColorCode?.Trim();
            entity.UtBlock = !request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private Task<bool> CodeExistsAsync(string code, int clientId, int? entityId, int? excludingId, CancellationToken cancellationToken)
        {
            return _context.HolidayTypes.AnyAsync(x => x.Code != null && x.Code.Trim().ToUpper() == code.ToUpper()
                && x.ClientId == clientId && (!entityId.HasValue || x.EntityId == entityId)
                && (!excludingId.HasValue || x.Id != excludingId), cancellationToken);
        }

        public async Task<bool> Handle(DeleteHolidayTypeV3Command command, CancellationToken cancellationToken)
        {
            var entity = await _context.HolidayTypes
                .FirstOrDefaultAsync(x => x.Id == command.Id
                    && x.ClientId == command.ClientId
                    && x.EntityId != null
                    && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value), cancellationToken);

            if (entity == null) return false;

            var isUsedInCalendarHoliday = await _context.CalendarHolidayDetails
                .AnyAsync(x => x.Holidaytypeid == entity.Id
                               && !x.Utblock
                               && x.Header != null
                               && !x.Header.Utblock
                               && x.Header.ClientId == command.ClientId
                               && (!command.EntityId.HasValue || x.Header.EntityId == command.EntityId.Value),
                    cancellationToken);

            if (isUsedInCalendarHoliday)
            {
                throw new InvalidOperationException("This holiday type is used in Calendar Holiday and cannot be deleted.");
            }

            _context.HolidayTypes.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
