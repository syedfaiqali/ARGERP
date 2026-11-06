using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Entities;
using Organization.Service.Infrastructure;
using Organization.Service.Infrastructure.Data;

namespace Organization.Service.Handlers.Commands
{
    public sealed class FOCalllogV3CommandHandlers :
        IRequestHandler<CreateFOCalllogV3Command, FOCalllogV3Dto>,
        IRequestHandler<UpdateFOCalllogV3Command, FOCalllogV3Dto>,
        IRequestHandler<DeleteFOCalllogV3Command, bool>
    {
        private readonly OrganizationDbContext _context;

        public FOCalllogV3CommandHandlers(OrganizationDbContext context)
        {
            _context = context;
        }

        private string GenerateCallId(int nextId)
        {
            return $"CALL-{(DateTime.Now.Year.ToString().Substring(2))}{nextId.ToString("D5")}";
        }

        public async Task<FOCalllogV3Dto> Handle(CreateFOCalllogV3Command command, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(command.ClientId);
            var req = command.Request;

            var nextId = 1;
            if (await _context.FOCalllogLegacy.IgnoreQueryFilters().AnyAsync(cancellationToken))
            {
                nextId = await _context.FOCalllogLegacy.IgnoreQueryFilters().MaxAsync(x => x.Id, cancellationToken) + 1;
            }

            var callIdStr = GenerateCallId(nextId);

            var entity = new FOCalllogLegacy
            {
                Id = nextId,
                LogId = callIdStr,
                CallDate = req.CallDate,
                CallTime = req.CallTime,
                PurposeTypeId = req.PurposeTypeId,
                Name = req.Name,
                PhoneNo = req.PhoneNo,
                CallTypeId = req.CallTypeId,
                CallMediumId = req.CallMediumId,
                Duration = req.Duration,
                Description = req.Description,
                EntityId = command.EntityId ?? (req.EntityId > 0 ? req.EntityId : 1),
                ClientId = command.ClientId,
                UtBlock = false,
                UtIsPartial = false,
                UtSysGen = false,
                UtRecordVersion = 1
            };

            _context.FOCalllogLegacy.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new FOCalllogV3Dto
            {
                Id = entity.Id,
                LogId = entity.LogId,
                CallDate = entity.CallDate?.ToString("yyyy-MM-dd"),
                Name = entity.Name,
                Description = entity.Description
            };
        }

        public async Task<FOCalllogV3Dto> Handle(UpdateFOCalllogV3Command command, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(command.ClientId);
            var req = command.Request;

            var entity = await _context.FOCalllogLegacy
                .FirstOrDefaultAsync(
                    x => x.Id == command.Id
                      && x.ClientId == command.ClientId
                      && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value)
                      && !x.UtBlock,
                    cancellationToken);

            if (entity == null)
            {
                throw new KeyNotFoundException($"Call Log with ID {command.Id} not found");
            }

            entity.CallDate = req.CallDate;
            entity.CallTime = req.CallTime;
            entity.PurposeTypeId = req.PurposeTypeId;
            entity.Name = req.Name;
            entity.PhoneNo = req.PhoneNo;
            entity.CallTypeId = req.CallTypeId;
            entity.CallMediumId = req.CallMediumId;
            entity.Duration = req.Duration;
            entity.Description = req.Description;
            entity.EntityId = command.EntityId ?? (req.EntityId > 0 ? req.EntityId : entity.EntityId);

            entity.UtRecordVersion += 1;

            _context.FOCalllogLegacy.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new FOCalllogV3Dto
            {
                Id = entity.Id,
                LogId = entity.LogId,
                CallDate = entity.CallDate?.ToString("yyyy-MM-dd"),
                Name = entity.Name,
                Description = entity.Description
            };
        }

        public async Task<bool> Handle(DeleteFOCalllogV3Command command, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(command.ClientId);
            var entity = await _context.FOCalllogLegacy
                .FirstOrDefaultAsync(
                    x => x.Id == command.Id
                      && x.ClientId == command.ClientId
                      && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value),
                    cancellationToken);

            if (entity == null || entity.UtBlock)
            {
                return false;
            }

            entity.UtBlock = true;
            entity.UtRecordVersion += 1;

            _context.FOCalllogLegacy.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
