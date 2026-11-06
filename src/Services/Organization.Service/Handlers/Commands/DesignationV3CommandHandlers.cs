using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Entities;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Services;

namespace Organization.Service.Handlers.Commands
{
    public sealed class DesignationV3CommandHandlers :
        IRequestHandler<CreateDesignationV3Command, DesignationV3Dto>,
        IRequestHandler<UpdateDesignationV3Command, bool>,
        IRequestHandler<DeleteDesignationV3Command, MasterDeleteResult>
    {
        private readonly OrganizationDbContext _context;

        public DesignationV3CommandHandlers(OrganizationDbContext context)
        {
            _context = context;
        }

        public Task<DesignationV3Dto> Handle(CreateDesignationV3Command command, CancellationToken cancellationToken) =>
            OrganizationMasterData.SaveAsync(_context, () => CreateAsync(command, cancellationToken), cancellationToken);

        private async Task<DesignationV3Dto> CreateAsync(CreateDesignationV3Command command, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(command.ClientId);
            var request = command.Request;
            OrganizationValidation.Text(request.DesignationName, "Designation", 50);
            OrganizationValidation.Text(request.Description, "Description", 500);
            var designationName = request.DesignationName?.Trim();

            if (string.IsNullOrWhiteSpace(designationName))
                throw new InvalidOperationException("Designation is required.");

            if (await _context.OrgDesignationLegacy.AnyAsync(
                    x => x.clientid == command.ClientId && (x.stxt ?? "").Trim().ToUpper() == designationName.ToUpper(), cancellationToken))
            {
                throw new InvalidOperationException($"Designation: {designationName} already exists.");
            }

            var nextId = (await _context.OrgDesignationLegacy.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            var code = OrganizationMasterData.NextCode(await _context.OrgDesignationLegacy.IgnoreQueryFilters()
                .Where(x => x.clientid == command.ClientId).Select(x => x.desgcode).ToListAsync(cancellationToken), 5);

            var entity = new OrgDesignationLegacy
            {
                Id = nextId,
                desgcode = code,
                stxt = designationName,
                ltxt = request.Description?.Trim(),
                blank = request.Inactive,
                clientid = command.ClientId,
                utblock = false
            };

            _context.OrgDesignationLegacy.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new DesignationV3Dto
            {
                Id = entity.Id,
                Code = entity.desgcode,
                DesignationName = entity.stxt,
                Description = entity.ltxt,
                Inactive = entity.blank == true
            };
        }

        public async Task<bool> Handle(UpdateDesignationV3Command command, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(command.ClientId);
            var request = command.Request;
            OrganizationValidation.Text(request.DesignationName, "Designation", 50);
            OrganizationValidation.Text(request.Description, "Description", 500);
            var designationName = request.DesignationName?.Trim();

            if (string.IsNullOrWhiteSpace(designationName))
                throw new InvalidOperationException("Designation is required.");

            var entity = await _context.OrgDesignationLegacy
                .FirstOrDefaultAsync(x => x.clientid == command.ClientId && x.Id == request.Id, cancellationToken);

            if (entity == null) return false;

            if (await _context.OrgDesignationLegacy.AnyAsync(
                    x => x.clientid == command.ClientId && (x.stxt ?? "").Trim().ToUpper() == designationName.ToUpper() && x.Id != request.Id, cancellationToken))
            {
                throw new InvalidOperationException($"Designation: {designationName} already exists.");
            }

            // Preserve the server-generated code on edit.
            entity.stxt = designationName;
            entity.ltxt = request.Description?.Trim();
            entity.blank = request.Inactive;
            entity.utblock = false;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<MasterDeleteResult> Handle(DeleteDesignationV3Command request, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(request.ClientId);
            var entity = await _context.OrgDesignationLegacy
                .FirstOrDefaultAsync(x => x.clientid == request.ClientId && x.Id == request.Id, cancellationToken);

            if (entity == null) return new(false, []);

            var usedIn = await OrganizationMasterData.FindUsageAsync(_context, "Designation", request.Id, cancellationToken);
            if (usedIn.Count > 0) return new(true, usedIn);
            entity.utblock = true;
            await _context.SaveChangesAsync(cancellationToken);
            return new(true, []);
        }
    }
}
