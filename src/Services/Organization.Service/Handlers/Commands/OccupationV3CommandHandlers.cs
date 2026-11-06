using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Infrastructure.Data;

namespace Organization.Service.Handlers.Commands
{
    public sealed class OccupationV3CommandHandlers :
        IRequestHandler<CreateOccupationV3Command, OccupationV3Dto>,
        IRequestHandler<UpdateOccupationV3Command, bool>,
        IRequestHandler<DeleteOccupationV3Command, bool>
    {
        private readonly OrganizationDbContext _context;

        public OccupationV3CommandHandlers(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<OccupationV3Dto> Handle(CreateOccupationV3Command command, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(command.ClientId);
            var request = command.Request;
            OrganizationValidation.Text(request.Code, "Code", 10);
            OrganizationValidation.Text(request.OccupationName, "Occupation", 150);
            OrganizationValidation.Text(request.Description, "Description", 500);
            var code = request.Code?.Trim();
            var occupationName = request.OccupationName?.Trim();

            if (string.IsNullOrWhiteSpace(code))
                throw new Exception("Code is required.");

            if (string.IsNullOrWhiteSpace(occupationName))
                throw new Exception("Occupation is required.");

            if (await _context.OccupationLegacy.AnyAsync(o => o.clientid == command.ClientId && !o.utblock && (o.code ?? "").Trim().ToUpper() == code.ToUpper(), cancellationToken))
                throw new Exception($"Occupation Code: {code} already exists.");

            if (await _context.OccupationLegacy.AnyAsync(o => o.clientid == command.ClientId && !o.utblock && (o.stxt ?? "").Trim().ToUpper() == occupationName.ToUpper(), cancellationToken))
                throw new Exception($"Occupation: {occupationName} already exists.");

            var nextId = (await _context.OccupationLegacy.IgnoreQueryFilters()
                .MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            var entity = new Entities.OccupationLegacy
            {
                Id = nextId,
                clientid = command.ClientId,
                code = code,
                stxt = occupationName,
                ltxt = request.Description?.Trim(),
                utblock = false
            };

            _context.OccupationLegacy.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new OccupationV3Dto
            {
                Id = entity.Id,
                Code = entity.code,
                OccupationName = entity.stxt,
                Description = entity.ltxt
            };
        }

        public async Task<bool> Handle(UpdateOccupationV3Command command, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(command.ClientId);
            var request = command.Request;
            OrganizationValidation.Text(request.Code, "Code", 10);
            OrganizationValidation.Text(request.OccupationName, "Occupation", 150);
            OrganizationValidation.Text(request.Description, "Description", 500);
            var code = request.Code?.Trim();
            var occupationName = request.OccupationName?.Trim();

            if (string.IsNullOrWhiteSpace(code))
                throw new Exception("Code is required.");

            if (string.IsNullOrWhiteSpace(occupationName))
                throw new Exception("Occupation is required.");

            var entity = await _context.OccupationLegacy
                .FirstOrDefaultAsync(x => x.clientid == command.ClientId && x.Id == request.Id && !x.utblock, cancellationToken);

            if (entity == null) return false;

            if (await _context.OccupationLegacy.AnyAsync(
                    o => o.clientid == command.ClientId && !o.utblock && (o.code ?? "").Trim().ToUpper() == code.ToUpper() && o.Id != request.Id, cancellationToken))
            {
                throw new Exception($"Occupation Code: {code} already exists.");
            }

            if (await _context.OccupationLegacy.AnyAsync(
                    o => o.clientid == command.ClientId && !o.utblock && (o.stxt ?? "").Trim().ToUpper() == occupationName.ToUpper() && o.Id != request.Id, cancellationToken))
            {
                throw new Exception($"Occupation: {occupationName} already exists.");
            }

            entity.code = code;
            entity.stxt = occupationName;
            entity.ltxt = request.Description?.Trim();

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Handle(DeleteOccupationV3Command request, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(request.ClientId);
            var entity = await _context.OccupationLegacy
                .FirstOrDefaultAsync(x => x.clientid == request.ClientId && x.Id == request.Id && !x.utblock, cancellationToken);

            if (entity == null) return false;

            entity.utblock = true;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
