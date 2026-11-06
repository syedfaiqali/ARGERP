using MediatR;
using Microsoft.EntityFrameworkCore;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Commands;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class SkillCommandHandlers :
        IRequestHandler<CreateSkillV3Command, SkillV3Dto>,
        IRequestHandler<UpdateSkillV3Command, bool>,
        IRequestHandler<DeleteSkillV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;

        public SkillCommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<SkillV3Dto> Handle(CreateSkillV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            if (!request.RatingModelId.HasValue)
            {
                throw new ArgumentException("Rating Model is required.");
            }

            if (await _context.Skills.AnyAsync(x => x.Code == request.Code && x.EntityId == command.EntityId && x.ClientId == command.ClientId, cancellationToken))
            {
                throw new InvalidOperationException($"Skill with code '{request.Code}' already exists.");
            }

            var nextId = (await _context.Skills.IgnoreQueryFilters()
                .MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            var entity = new ERSkillLegacy
            {
                Id = nextId,
                Code = request.Code.Trim(),
                SkillTypeId = request.SkillTypeId,
                SkillRatingId = request.RatingModelId,
                Stxt = request.Name.Trim(),
                Ltxt = request.Description?.Trim() ?? string.Empty,
                EntityId = command.EntityId,
                ClientId = command.ClientId,
                UtBlock = !request.IsActive
            };

            _context.Skills.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            // Fetch names for DTO
            var stName = await _context.SkillTypes.Where(x => x.Id == entity.SkillTypeId).Select(x => x.Stxt).FirstOrDefaultAsync(cancellationToken);
            var rmName = await _context.SkillRatings.Where(x => x.Id == entity.SkillRatingId).Select(x => x.Stxt).FirstOrDefaultAsync(cancellationToken);

            return new SkillV3Dto(entity.Id, entity.Code ?? string.Empty, entity.SkillTypeId, stName, entity.Stxt ?? string.Empty, entity.Ltxt, entity.SkillRatingId, rmName, !(entity.UtBlock ?? false));
        }

        public async Task<bool> Handle(UpdateSkillV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            if (!request.RatingModelId.HasValue)
            {
                throw new ArgumentException("Rating Model is required.");
            }

            var entity = await _context.Skills
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.ClientId == command.ClientId && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value), cancellationToken);

            if (entity == null) return false;

            if (await _context.Skills.AnyAsync(x => x.Code == request.Code && x.EntityId == command.EntityId && x.ClientId == command.ClientId && x.Id != request.Id, cancellationToken))
            {
                throw new InvalidOperationException($"Another skill with code '{request.Code}' already exists.");
            }

            entity.Code = request.Code.Trim();
            entity.SkillTypeId = request.SkillTypeId;
            entity.SkillRatingId = request.RatingModelId;
            entity.Stxt = request.Name.Trim();
            entity.Ltxt = request.Description?.Trim() ?? string.Empty;
            entity.UtBlock = !request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Handle(DeleteSkillV3Command command, CancellationToken cancellationToken)
        {
            var entity = await _context.Skills
                .FirstOrDefaultAsync(x => x.Id == command.Id && x.ClientId == command.ClientId && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value), cancellationToken);

            if (entity == null) return false;

            _context.Skills.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
