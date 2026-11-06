using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Entities;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Services;

namespace Organization.Service.Handlers.Commands
{
    public sealed class DepartmentV3CommandHandlers :
        IRequestHandler<CreateDepartmentV3Command, DepartmentV3Dto>,
        IRequestHandler<UpdateDepartmentV3Command, bool>,
        IRequestHandler<DeleteDepartmentV3Command, MasterDeleteResult>
    {
        private readonly OrganizationDbContext _context;

        public DepartmentV3CommandHandlers(OrganizationDbContext context)
        {
            _context = context;
        }

        public Task<DepartmentV3Dto> Handle(CreateDepartmentV3Command command, CancellationToken cancellationToken) =>
            OrganizationMasterData.SaveAsync(_context, () => CreateAsync(command, cancellationToken), cancellationToken);

        private async Task<DepartmentV3Dto> CreateAsync(CreateDepartmentV3Command command, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(command.ClientId);
            var request = command.Request;
            NormalizeAndValidate(request);

            if (await _context.OrgDepartmentLegacy.AnyAsync(o => o.clientid == command.ClientId && (o.depstxt ?? "").Trim().ToUpper() == request.DepartmentName.ToUpper(), cancellationToken))
                throw new InvalidOperationException($"Department Name: {request.DepartmentName} already exists.");

            var nextId = (await _context.OrgDepartmentLegacy.IgnoreQueryFilters()
                .MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            request.Code = OrganizationMasterData.NextCode(await _context.OrgDepartmentLegacy.IgnoreQueryFilters()
                .Where(x => x.clientid == command.ClientId).Select(x => x.depcode).ToListAsync(cancellationToken), 10);

            var entity = new OrgDepartmentLegacy
            {
                Id = nextId,
                depcode = request.Code,
                depstxt = request.DepartmentName,
                depltxt = request.Description,
                ficoaid = request.ChartOfAccountId,
                ficoaid_rec = request.ReceivingChartOfAccountId,
                clientid = command.ClientId,
                utblock = false
            };

            _context.OrgDepartmentLegacy.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            // Fetch created entity to return with lookup names
            return (await _context.OrgDepartmentLegacy
                .Where(x => x.Id == entity.Id)
                .Select(x => new DepartmentV3Dto
                {
                    Id = x.Id,
                    Code = x.depcode,
                    DepartmentName = x.depstxt,
                    Description = x.depltxt,
                    ChartOfAccountId = x.ficoaid,
                    ChartOfAccount = x.Ficoa != null ? x.Ficoa.Title : null,
                    ReceivingChartOfAccountId = x.ficoaid_rec,
                    ReceivingChartOfAccount = x.FicoaRec != null ? x.FicoaRec.Title : null
                })
                .FirstAsync(cancellationToken));
        }

        public async Task<bool> Handle(UpdateDepartmentV3Command command, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(command.ClientId);
            var request = command.Request;
            NormalizeAndValidate(request);
            var entity = await _context.OrgDepartmentLegacy
                .FirstOrDefaultAsync(x => x.clientid == command.ClientId && x.Id == request.Id, cancellationToken);

            if (entity == null) return false;

            if (await _context.OrgDepartmentLegacy.AnyAsync(o => o.clientid == command.ClientId && (o.depstxt ?? "").Trim().ToUpper() == request.DepartmentName.ToUpper() && o.Id != request.Id, cancellationToken))
                throw new InvalidOperationException($"Department Name: {request.DepartmentName} already exists.");

            // Codes are generated on creation and remain immutable on edit.
            entity.depstxt = request.DepartmentName;
            entity.depltxt = request.Description;
            entity.ficoaid = request.ChartOfAccountId;
            entity.ficoaid_rec = request.ReceivingChartOfAccountId;
            entity.utblock = false;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task ValidateProposalAsync(CreateDepartmentV3Request request, int clientId, int? excludeId, CancellationToken ct)
        {
            NormalizeAndValidate(request);
            if (await _context.OrgDepartmentLegacy.AnyAsync(x => x.clientid == clientId && x.Id != excludeId && (x.depstxt ?? "").Trim().ToUpper() == request.DepartmentName.ToUpper(), ct))
                throw new InvalidOperationException("Department name already exists.");
        }

        private static void NormalizeAndValidate(CreateDepartmentV3Request request)
        {
            request.Code = request.Code?.Trim();
            request.DepartmentName = request.DepartmentName?.Trim();
            request.Description = request.Description?.Trim();

            ValidateText(request.DepartmentName, "Department", 50, required: true);
            ValidateText(request.Description, "Description", 500, required: false);
        }

        private static void NormalizeAndValidate(UpdateDepartmentV3Request request)
        {
            request.Code = request.Code?.Trim();
            request.DepartmentName = request.DepartmentName?.Trim();
            request.Description = request.Description?.Trim();

            ValidateText(request.DepartmentName, "Department", 50, required: true);
            ValidateText(request.Description, "Description", 500, required: false);
        }

        private static void ValidateText(string? value, string fieldName, int maxLength, bool required)
        {
            if (required && string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException($"{fieldName} is required.");
            }

            if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
            {
                throw new InvalidOperationException($"{fieldName} cannot exceed {maxLength} characters. Current length is {value.Length}.");
            }
        }

        public async Task<MasterDeleteResult> Handle(DeleteDepartmentV3Command request, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(request.ClientId);
            var entity = await _context.OrgDepartmentLegacy
                .FirstOrDefaultAsync(x => x.clientid == request.ClientId && x.Id == request.Id, cancellationToken);

            if (entity == null) return new(false, []);

            var usedIn = await OrganizationMasterData.FindUsageAsync(_context, "Department", request.Id, cancellationToken);
            if (usedIn.Count > 0) return new(true, usedIn);
            entity.utblock = true;
            await _context.SaveChangesAsync(cancellationToken);
            return new(true, []);
        }
    }
}
