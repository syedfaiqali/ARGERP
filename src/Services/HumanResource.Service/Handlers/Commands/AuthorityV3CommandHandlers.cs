using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Commands
{
    public class AuthorityV3CommandHandlers : 
        IRequestHandler<CreateAuthorityV3Command, AuthorityV3Dto>,
        IRequestHandler<UpdateAuthorityV3Command, AuthorityV3Dto>,
        IRequestHandler<DeleteAuthorityV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;

        public AuthorityV3CommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<AuthorityV3Dto> Handle(CreateAuthorityV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            var codeExists = await _context.EmployeeAuthorities
                .AnyAsync(x => x.Code == request.Code && x.ClientId == command.ClientId, cancellationToken);
            if (codeExists)
                throw new InvalidOperationException("Authority code already exists.");

            var nameExists = await _context.EmployeeAuthorities
                .AnyAsync(x => x.Stxt == request.Authority && x.ClientId == command.ClientId, cancellationToken);
            if (nameExists)
                throw new InvalidOperationException("Authority name already exists.");

            if (request.AuthorityTypeId.HasValue)
            {
                var lovExists = await _context.Lovs.AnyAsync(x => x.Id == request.AuthorityTypeId.Value, cancellationToken);
                if (!lovExists)
                    throw new InvalidOperationException("Invalid Authority Type ID.");
            }

            var nextId = (await _context.EmployeeAuthorities.IgnoreQueryFilters()
                .MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            var entity = new EREmpAuthorityLegacy
            {
                Id = nextId,
                Code = request.Code?.Trim(),
                AuthTypeId = request.AuthorityTypeId,
                Stxt = request.Authority?.Trim(),
                Ltxt = request.Description?.Trim(),
                ContactNo = request.ContactNo?.Trim(),
                Phone = request.Phone?.Trim(),
                Web = request.Web?.Trim(),
                Fax = request.Fax?.Trim(),
                Email = request.EmailId?.Trim(),
                UtBlock = !request.IsActive,
                ClientId = command.ClientId,
                EntityId = command.EntityId
            };

            _context.EmployeeAuthorities.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            var authTypeName = string.Empty;
            if (entity.AuthTypeId.HasValue)
            {
                authTypeName = await _context.Lovs
                    .Where(x => x.Id == entity.AuthTypeId.Value)
                    .Select(x => x.Stxt)
                    .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;
            }

            return new AuthorityV3Dto(
                entity.Id,
                entity.Code ?? string.Empty,
                entity.AuthTypeId,
                authTypeName,
                entity.Stxt ?? string.Empty,
                entity.Ltxt ?? string.Empty,
                entity.ContactNo ?? string.Empty,
                entity.Phone ?? string.Empty,
                entity.Web ?? string.Empty,
                entity.Fax ?? string.Empty,
                entity.Email ?? string.Empty,
                entity.UtBlock != true
            );
        }

        public async Task<AuthorityV3Dto> Handle(UpdateAuthorityV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var entity = await _context.EmployeeAuthorities
                .Where(x => x.ClientId == command.ClientId && x.Id == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException("Authority not found.");

            var codeExists = await _context.EmployeeAuthorities
                .AnyAsync(x => x.Code == request.Code && x.Id != request.Id && x.ClientId == command.ClientId, cancellationToken);
            if (codeExists)
                throw new InvalidOperationException("Authority code already exists.");

            var nameExists = await _context.EmployeeAuthorities
                .AnyAsync(x => x.Stxt == request.Authority && x.Id != request.Id && x.ClientId == command.ClientId, cancellationToken);
            if (nameExists)
                throw new InvalidOperationException("Authority name already exists.");

            if (request.AuthorityTypeId.HasValue)
            {
                var lovExists = await _context.Lovs.AnyAsync(x => x.Id == request.AuthorityTypeId.Value, cancellationToken);
                if (!lovExists)
                    throw new InvalidOperationException("Invalid Authority Type ID.");
            }

            entity.Code = request.Code?.Trim();
            entity.AuthTypeId = request.AuthorityTypeId;
            entity.Stxt = request.Authority?.Trim();
            entity.Ltxt = request.Description?.Trim();
            entity.ContactNo = request.ContactNo?.Trim();
            entity.Phone = request.Phone?.Trim();
            entity.Web = request.Web?.Trim();
            entity.Fax = request.Fax?.Trim();
            entity.Email = request.EmailId?.Trim();
            entity.UtBlock = !request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);

            var authTypeName = string.Empty;
            if (entity.AuthTypeId.HasValue)
            {
                authTypeName = await _context.Lovs
                    .Where(x => x.Id == entity.AuthTypeId.Value)
                    .Select(x => x.Stxt)
                    .FirstOrDefaultAsync(cancellationToken) ?? string.Empty;
            }

            return new AuthorityV3Dto(
                entity.Id,
                entity.Code ?? string.Empty,
                entity.AuthTypeId,
                authTypeName,
                entity.Stxt ?? string.Empty,
                entity.Ltxt ?? string.Empty,
                entity.ContactNo ?? string.Empty,
                entity.Phone ?? string.Empty,
                entity.Web ?? string.Empty,
                entity.Fax ?? string.Empty,
                entity.Email ?? string.Empty,
                entity.UtBlock != true
            );
        }

        public async Task<bool> Handle(DeleteAuthorityV3Command command, CancellationToken cancellationToken)
        {
            var entity = await _context.EmployeeAuthorities
                .FirstOrDefaultAsync(x => x.Id == command.Id && x.ClientId == command.ClientId, cancellationToken);

            if (entity == null)
                throw new KeyNotFoundException("Authority not found.");

            _context.EmployeeAuthorities.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
