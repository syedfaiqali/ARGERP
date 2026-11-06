using MediatR;
using Microsoft.EntityFrameworkCore;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Commands;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class AttendanceActivityCommandHandlers :
        IRequestHandler<CreateAttendanceActivityV3Command, AttendanceActivityV3Dto>,
        IRequestHandler<UpdateAttendanceActivityV3Command, bool>,
        IRequestHandler<DeleteAttendanceActivityV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;

        public AttendanceActivityCommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<AttendanceActivityV3Dto> Handle(CreateAttendanceActivityV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            if (string.IsNullOrWhiteSpace(request.Activity))
            {
                throw new InvalidOperationException("Activity is required.");
            }

            var activityExists = await _context.AttendanceActivities
                .AnyAsync(x => x.Stxt == request.Activity.Trim() && x.ClientId == command.ClientId && !x.UtBlock, cancellationToken);
            if (activityExists)
            {
                throw new InvalidOperationException("Attendance Activity already exists.");
            }

            var entity = new HRAtnactivityLegacy
            {
                Stxt = request.Activity.Trim(),
                Value = request.Value,
                OrderId = request.OrderId,
                ClientId = command.ClientId,
                UtBlock = false
            };

            _context.AttendanceActivities.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new AttendanceActivityV3Dto(
                entity.Id,
                entity.OrderId,
                entity.Stxt,
                entity.Value
            );
        }

        public async Task<bool> Handle(UpdateAttendanceActivityV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var entity = await _context.AttendanceActivities
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.ClientId == command.ClientId, cancellationToken);

            if (entity == null) return false;

            if (string.IsNullOrWhiteSpace(request.Activity))
            {
                throw new InvalidOperationException("Activity is required.");
            }

            var activityExists = await _context.AttendanceActivities
                .AnyAsync(x => x.Stxt == request.Activity.Trim() && x.ClientId == command.ClientId && x.Id != request.Id && !x.UtBlock, cancellationToken);
            if (activityExists)
            {
                throw new InvalidOperationException("Attendance Activity already exists.");
            }

            entity.Stxt = request.Activity.Trim();
            entity.Value = request.Value;
            entity.OrderId = request.OrderId;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Handle(DeleteAttendanceActivityV3Command command, CancellationToken cancellationToken)
        {
            var entity = await _context.AttendanceActivities
                .FirstOrDefaultAsync(x => x.Id == command.Id && x.ClientId == command.ClientId, cancellationToken);

            if (entity == null) return false;

            entity.UtBlock = true;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}