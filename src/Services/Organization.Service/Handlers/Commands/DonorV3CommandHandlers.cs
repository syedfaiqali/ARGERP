using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Entities;
using Organization.Service.Infrastructure.Data;

namespace Organization.Service.Handlers.Commands
{
    public sealed class DonorV3CommandHandlers :
        IRequestHandler<CreateDonorV3Command, DonorV3Dto>,
        IRequestHandler<UpdateDonorV3Command, bool>,
        IRequestHandler<DeleteDonorV3Command, bool>
    {
        private readonly OrganizationDbContext _context;

        public DonorV3CommandHandlers(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<DonorV3Dto> Handle(CreateDonorV3Command command, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(command.ClientId);
            var request = command.Request;
            OrganizationValidation.Text(request.Code, "Code", 10);
            OrganizationValidation.Text(request.DonorName, "Donor Name", 50);
            OrganizationValidation.Text(request.Description, "Description", 50);
            OrganizationValidation.Text(request.ContactNo, "Contact No", 50);
            var code = request.Code?.Trim();
            var donorName = request.DonorName?.Trim();

            if (string.IsNullOrWhiteSpace(code))
                throw new Exception("Code is required.");

            if (string.IsNullOrWhiteSpace(donorName))
                throw new Exception("Donor name is required.");

            if (await _context.OrgDonorLegacy.AnyAsync(
                    x => x.clientid == command.ClientId &&
                         x.utblock != true &&
                         (x.code ?? "").Trim().ToUpper() == code.ToUpper(), cancellationToken))
            {
                throw new Exception($"Code: {code} already exists.");
            }

            if (await _context.OrgDonorLegacy.AnyAsync(
                    x => x.clientid == command.ClientId &&
                         x.utblock != true &&
                         (x.stxt ?? "").Trim().ToUpper() == donorName.ToUpper(), cancellationToken))
            {
                throw new Exception($"Donor Name: {donorName} already exists.");
            }

            var nextId = (await _context.OrgDonorLegacy.IgnoreQueryFilters()
                .MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            var entity = new OrgDonorLegacy
            {
                Id = nextId,
                code = code,
                stxt = donorName,
                ltxt = request.Description?.Trim(),
                contactno = request.ContactNo?.Trim(),
                clientid = command.ClientId,
                entityid = command.EntityId,
                utblock = false
            };

            _context.OrgDonorLegacy.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new DonorV3Dto
            {
                Id = entity.Id,
                Code = entity.code,
                DonorName = entity.stxt,
                Description = entity.ltxt,
                ContactNo = entity.contactno
            };
        }

        public async Task<bool> Handle(UpdateDonorV3Command command, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(command.ClientId);
            var request = command.Request;
            OrganizationValidation.Text(request.Code, "Code", 10);
            OrganizationValidation.Text(request.DonorName, "Donor Name", 50);
            OrganizationValidation.Text(request.Description, "Description", 50);
            OrganizationValidation.Text(request.ContactNo, "Contact No", 50);
            var code = request.Code?.Trim();
            var donorName = request.DonorName?.Trim();

            if (string.IsNullOrWhiteSpace(code))
                throw new Exception("Code is required.");

            if (string.IsNullOrWhiteSpace(donorName))
                throw new Exception("Donor name is required.");

            var entity = await _context.OrgDonorLegacy
                .FirstOrDefaultAsync(x =>
                    x.clientid == command.ClientId &&
                    x.Id == request.Id &&
                    x.utblock != true, cancellationToken);

            if (entity == null) return false;

            if (await _context.OrgDonorLegacy.AnyAsync(
                    x => x.clientid == command.ClientId &&
                         x.utblock != true &&
                         (x.code ?? "").Trim().ToUpper() == code.ToUpper() &&
                         x.Id != request.Id, cancellationToken))
            {
                throw new Exception($"Code: {code} already exists.");
            }

            if (await _context.OrgDonorLegacy.AnyAsync(
                    x => x.clientid == command.ClientId &&
                         x.utblock != true &&
                         (x.stxt ?? "").Trim().ToUpper() == donorName.ToUpper() &&
                         x.Id != request.Id, cancellationToken))
            {
                throw new Exception($"Donor Name: {donorName} already exists.");
            }

            entity.code = code;
            entity.stxt = donorName;
            entity.ltxt = request.Description?.Trim();
            entity.contactno = request.ContactNo?.Trim();

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Handle(DeleteDonorV3Command request, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(request.ClientId);
            var entity = await _context.OrgDonorLegacy
                .FirstOrDefaultAsync(x =>
                    x.clientid == request.ClientId &&
                    x.Id == request.Id &&
                    x.utblock != true, cancellationToken);

            if (entity == null) return false;

            entity.utblock = true;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
