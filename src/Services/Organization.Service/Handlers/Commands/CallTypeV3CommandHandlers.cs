using MediatR;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Entities;
using Organization.Service.Infrastructure.Data;

namespace Organization.Service.Handlers.Commands
{
    public sealed class CallTypeV3CommandHandlers :
        IRequestHandler<CreateCallTypeV3Command, CallTypeV3Dto>,
        IRequestHandler<UpdateCallTypeV3Command, bool>,
        IRequestHandler<DeleteCallTypeV3Command, bool>
    {
        private readonly OrganizationDbContext _context;

        public CallTypeV3CommandHandlers(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<CallTypeV3Dto> Handle(CreateCallTypeV3Command request, CancellationToken cancellationToken)
        {
            await EnsureUnique(request.Request.Code, request.Request.ShortName, 0, cancellationToken);
            var entity = new Lov
            {
                code = request.Request.Code,
                stxt = request.Request.ShortName,
                ltxt = request.Request.Description,
                vtype = "CALLT"
            };

            _context.Lovs.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new CallTypeV3Dto
            {
                Id = entity.Id,
                Code = entity.code,
                ShortName = entity.stxt,
                Description = entity.ltxt
            };
        }

        public async Task<bool> Handle(UpdateCallTypeV3Command request, CancellationToken cancellationToken)
        {
            var entity = await _context.Lovs
                .FirstOrDefaultAsync(x => x.Id == request.Request.Id && x.vtype == "CALLT" && !x.utblock, cancellationToken);

            if (entity == null) return false;

            await EnsureUnique(request.Request.Code, request.Request.ShortName, entity.Id, cancellationToken);
            entity.code = request.Request.Code;
            entity.stxt = request.Request.ShortName;
            entity.ltxt = request.Request.Description;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Handle(DeleteCallTypeV3Command request, CancellationToken cancellationToken)
        {
            var entity = await _context.Lovs
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.vtype == "CALLT" && !x.utblock, cancellationToken);

            if (entity == null) return false;

            entity.utblock = true;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        private async Task EnsureUnique(string? code, string? name, int id, CancellationToken token)
        {
            if (await _context.Lovs.AnyAsync(x => x.vtype == "CALLT" && !x.utblock && x.Id != id && x.code == code, token))
                throw new InvalidOperationException("Code already exists.");
            if (await _context.Lovs.AnyAsync(x => x.vtype == "CALLT" && !x.utblock && x.Id != id && x.stxt == name, token))
                throw new InvalidOperationException("Name already exists.");
        }
    }
}
