using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Entities;

namespace Organization.Service.Handlers.Commands
{
    public sealed class CreateAcademicRegisterV3CommandHandler : IRequestHandler<CreateAcademicRegisterV3Command, AcademicRegisterV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public CreateAcademicRegisterV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<AcademicRegisterV3Dto> Handle(CreateAcademicRegisterV3Command request, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(request.ClientId);
            var code = request.Code?.Trim();
            var registerName = request.RegisterName?.Trim();

            if (string.IsNullOrWhiteSpace(code))
                throw new InvalidOperationException("Code is required");
            if (string.IsNullOrWhiteSpace(registerName))
                throw new InvalidOperationException("Academic Register is required");

            if (await _context.AcademicRegisterLegacy.AnyAsync(x => !x.utblock && x.clientid == request.ClientId && (x.code ?? "").Trim().ToUpper() == code.ToUpper(), cancellationToken))
                throw new InvalidOperationException($"Code : {code} already exists");

            if (await _context.AcademicRegisterLegacy.AnyAsync(x => !x.utblock && x.clientid == request.ClientId && (x.stxt ?? "").Trim().ToUpper() == registerName.ToUpper(), cancellationToken))
                throw new InvalidOperationException($"Academic Register : {registerName} already exists");

            var nextId = (await _context.AcademicRegisterLegacy.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            var entity = new AcademicRegisterLegacy
            {
                Id = nextId,
                code = code,
                stxt = registerName,
                ltxt = request.Description,
                clientid = request.ClientId,
                utsysgen = false,
                utblock = false,
                utispartial = false,
                utrecordversion = 0
            };

            _context.AcademicRegisterLegacy.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new AcademicRegisterV3Dto
            {
                Id = entity.Id,
                Code = entity.code,
                RegisterName = entity.stxt,
                Description = entity.ltxt,
                ClientId = entity.clientid
            };
        }
    }

    public sealed class UpdateAcademicRegisterV3CommandHandler : IRequestHandler<UpdateAcademicRegisterV3Command, AcademicRegisterV3Dto>
    {
        private readonly OrganizationDbContext _context;

        public UpdateAcademicRegisterV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<AcademicRegisterV3Dto> Handle(UpdateAcademicRegisterV3Command request, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(request.ClientId);
            var entity = await _context.AcademicRegisterLegacy
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.clientid == request.ClientId && !x.utblock, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"Academic Register with ID {request.Id} not found");

            var code = request.Code?.Trim();
            var registerName = request.RegisterName?.Trim();

            if (string.IsNullOrWhiteSpace(code))
                throw new InvalidOperationException("Code is required");
            if (string.IsNullOrWhiteSpace(registerName))
                throw new InvalidOperationException("Academic Register is required");

            if (await _context.AcademicRegisterLegacy.AnyAsync(x => !x.utblock && x.clientid == request.ClientId && (x.code ?? "").Trim().ToUpper() == code.ToUpper() && x.Id != request.Id, cancellationToken))
                throw new InvalidOperationException($"Code : {code} already exists");

            if (await _context.AcademicRegisterLegacy.AnyAsync(x => !x.utblock && x.clientid == request.ClientId && (x.stxt ?? "").Trim().ToUpper() == registerName.ToUpper() && x.Id != request.Id, cancellationToken))
                throw new InvalidOperationException($"Academic Register : {registerName} already exists");

            entity.code = code;
            entity.stxt = registerName;
            entity.ltxt = request.Description;

            _context.AcademicRegisterLegacy.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new AcademicRegisterV3Dto
            {
                Id = entity.Id,
                Code = entity.code ?? string.Empty,
                RegisterName = entity.stxt ?? string.Empty,
                Description = entity.ltxt,
                ClientId = entity.clientid
            };
        }
    }

    public sealed class DeleteAcademicRegisterV3CommandHandler : IRequestHandler<DeleteAcademicRegisterV3Command, bool>
    {
        private readonly OrganizationDbContext _context;

        public DeleteAcademicRegisterV3CommandHandler(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteAcademicRegisterV3Command request, CancellationToken cancellationToken)
        {
            OrganizationValidation.Client(request.ClientId);
            var entity = await _context.AcademicRegisterLegacy
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.clientid == request.ClientId && !x.utblock, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException($"Academic Register with ID {request.Id} not found");

            entity.utblock = true;
            _context.AcademicRegisterLegacy.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
