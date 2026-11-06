using MediatR;
using Microsoft.EntityFrameworkCore;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Commands;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class QualificationCommandHandlers :
        IRequestHandler<CreateQualificationV3Command, QualificationV3Dto>,
        IRequestHandler<UpdateQualificationV3Command, bool>,
        IRequestHandler<DeleteQualificationV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;
        private const int HumanResourceLovTypeId = 368;

        public QualificationCommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<QualificationV3Dto> Handle(CreateQualificationV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            // Validation logic from legacy
            if (await _context.Qualifications.AnyAsync(x => x.code == request.Code && x.entityid == command.EntityId && x.clientid == command.ClientId, cancellationToken))
            {
                throw new InvalidOperationException($"Qualification with code '{request.Code}' already exists.");
            }

            var nextId = (await _context.Qualifications.IgnoreQueryFilters()
                .MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            var entity = new ERQualificationLegacy
            {
                Id = nextId,
                code = request.Code.Trim(),
                quatypeid = request.QualificationTypeId,
                stxt = request.Qualification.Trim(),
                ltxt = request.Description?.Trim() ?? string.Empty,
                entityid = command.EntityId,
                clientid = command.ClientId,
                lovtypeid = HumanResourceLovTypeId,
                utblock = !request.IsActive
            };

            _context.Qualifications.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            var typeName = await _context.QualificationTypes
                .Where(x => x.Id == entity.quatypeid)
                .Select(x => x.Stxt)
                .FirstOrDefaultAsync(cancellationToken);

            return new QualificationV3Dto(entity.Id, entity.code ?? string.Empty, entity.quatypeid, typeName ?? string.Empty, entity.stxt ?? string.Empty, entity.ltxt, entity.lovtypeid, !(entity.utblock ?? false));
        }

        public async Task<bool> Handle(UpdateQualificationV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var entity = await _context.Qualifications
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.clientid == command.ClientId && (!command.EntityId.HasValue || x.entityid == command.EntityId.Value), cancellationToken);

            if (entity == null) return false;

            // Validation logic from legacy
            if (await _context.Qualifications.AnyAsync(x => x.code == request.Code && x.entityid == command.EntityId && x.clientid == command.ClientId && x.Id != request.Id, cancellationToken))
            {
                throw new InvalidOperationException($"Another qualification with code '{request.Code}' already exists.");
            }

            entity.code = request.Code.Trim();
            entity.quatypeid = request.QualificationTypeId;
            entity.stxt = request.Qualification.Trim();
            entity.ltxt = request.Description?.Trim() ?? string.Empty;
            entity.lovtypeid = HumanResourceLovTypeId;
            entity.utblock = !request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Handle(DeleteQualificationV3Command command, CancellationToken cancellationToken)
        {
            var entity = await _context.Qualifications
                .FirstOrDefaultAsync(x => x.Id == command.Id && x.clientid == command.ClientId && (!command.EntityId.HasValue || x.entityid == command.EntityId.Value), cancellationToken);

            if (entity == null) return false;

            _context.Qualifications.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}

