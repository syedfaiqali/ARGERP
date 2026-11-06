using MediatR;
using Microsoft.EntityFrameworkCore;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Commands;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class SkillTypeCommandHandlers :
        IRequestHandler<CreateSkillTypeV3Command, SkillTypeV3Dto>,
        IRequestHandler<UpdateSkillTypeV3Command, bool>,
        IRequestHandler<DeleteSkillTypeV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;

        public SkillTypeCommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<SkillTypeV3Dto> Handle(CreateSkillTypeV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            if (await _context.SkillTypes.AnyAsync(x => x.Code == request.Code && x.EntityId == command.EntityId && x.ClientId == command.ClientId, cancellationToken))
            {
                throw new InvalidOperationException($"Skill Type with code '{request.Code}' already exists.");
            }

            var nextId = (await _context.SkillTypes.IgnoreQueryFilters()
                .Select(x => (int?)x.Id)
                .MaxAsync(cancellationToken) ?? 0) + 1;

            var entity = new ERSkilltypeLegacy
            {
                Id = nextId,
                Code = request.Code.Trim(),
                Stxt = request.Name.Trim(),
                Ltxt = request.Description?.Trim() ?? string.Empty,
                EntityId = command.EntityId,
                ClientId = command.ClientId,
                UtBlock = !request.IsActive
            };

            _context.SkillTypes.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new SkillTypeV3Dto(entity.Id, entity.Code ?? string.Empty, entity.Stxt ?? string.Empty, entity.Ltxt, !(entity.UtBlock ?? false));
        }

        public async Task<bool> Handle(UpdateSkillTypeV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var entity = await _context.SkillTypes
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.ClientId == command.ClientId && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value), cancellationToken);

            if (entity == null) return false;

            if (request.Code != null && await _context.SkillTypes.AnyAsync(x => x.Code == request.Code && x.EntityId == command.EntityId && x.ClientId == command.ClientId && x.Id != request.Id, cancellationToken))
            {
                throw new InvalidOperationException($"Another skill type with code '{request.Code}' already exists.");
            }

            entity.Code = request.Code?.Trim();
            entity.Stxt = request.Name.Trim();
            entity.Ltxt = request.Description?.Trim() ?? string.Empty;
            entity.UtBlock = !request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Handle(DeleteSkillTypeV3Command command, CancellationToken cancellationToken)
        {
            var entity = await _context.SkillTypes
                .FirstOrDefaultAsync(x => x.Id == command.Id && x.ClientId == command.ClientId && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value), cancellationToken);

            if (entity == null) return false;

            _context.SkillTypes.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
