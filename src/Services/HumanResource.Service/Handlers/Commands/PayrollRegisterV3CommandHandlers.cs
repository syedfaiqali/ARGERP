using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class PayrollRegisterV3CommandHandlers :
        IRequestHandler<CreatePayrollRegisterV3Command, PayrollRegisterV3Dto>,
        IRequestHandler<UpdatePayrollRegisterV3Command, bool>,
        IRequestHandler<DeletePayrollRegisterV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;

        public PayrollRegisterV3CommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<PayrollRegisterV3Dto> Handle(CreatePayrollRegisterV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            if (string.IsNullOrWhiteSpace(request.Stxt))
            {
                throw new InvalidOperationException("Payroll Register cannot be empty.");
            }

            var stxtExists = await _context.PayrollRegisterLegacy
                .AnyAsync(x => x.Stxt == request.Stxt.Trim() && x.ClientId == command.ClientId, cancellationToken);
            if (stxtExists)
            {
                throw new InvalidOperationException("Payroll Register already exists.");
            }

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                var codeExists = await _context.PayrollRegisterLegacy
                    .AnyAsync(x => x.Code == request.Code.Trim() && x.ClientId == command.ClientId, cancellationToken);
                if (codeExists)
                {
                    throw new InvalidOperationException("Payroll Register code already exists.");
                }
            }

            var nextId = (await _context.PayrollRegisterLegacy.IgnoreQueryFilters()
                .MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            var code = string.IsNullOrWhiteSpace(request.Code)
                ? nextId.ToString("0000")
                : request.Code.Trim();

            var entity = new ETPayrollregisterLegacy
            {
                Id = nextId,
                Code = code,
                Stxt = request.Stxt.Trim(),
                Ltxt = request.Ltxt?.Trim(),
                ClientId = command.ClientId,
                // Utblock is the legacy soft-delete flag. Payroll Register has no
                // active/inactive state, so saving must never set this flag.
                Utblock = false
            };

            _context.PayrollRegisterLegacy.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new PayrollRegisterV3Dto(
                entity.Id,
                entity.Code ?? string.Empty,
                entity.Stxt ?? string.Empty,
                entity.Ltxt);
        }

        public async Task<bool> Handle(UpdatePayrollRegisterV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var entity = await _context.PayrollRegisterLegacy
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.ClientId == command.ClientId, cancellationToken);

            if (entity == null) return false;

            if (string.IsNullOrWhiteSpace(request.Stxt))
            {
                throw new InvalidOperationException("Payroll Register cannot be empty.");
            }

            var stxtExists = await _context.PayrollRegisterLegacy
                .AnyAsync(x => x.Stxt == request.Stxt.Trim() && x.Id != request.Id && x.ClientId == command.ClientId, cancellationToken);
            if (stxtExists)
            {
                throw new InvalidOperationException("Payroll Register already exists.");
            }

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                var codeExists = await _context.PayrollRegisterLegacy
                    .AnyAsync(x => x.Code == request.Code.Trim() && x.Id != request.Id && x.ClientId == command.ClientId, cancellationToken);
                if (codeExists)
                {
                    throw new InvalidOperationException("Payroll Register code already exists.");
                }
            }

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                entity.Code = request.Code.Trim();
            }

            entity.Stxt = request.Stxt.Trim();
            entity.Ltxt = request.Ltxt?.Trim();
            // Keep a saved register visible; deletion alone sets Utblock.
            entity.Utblock = false;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Handle(DeletePayrollRegisterV3Command command, CancellationToken cancellationToken)
        {
            var entity = await _context.PayrollRegisterLegacy
                .FirstOrDefaultAsync(x => x.Id == command.Id && x.ClientId == command.ClientId, cancellationToken);

            if (entity == null) return false;

            entity.Utblock = true;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
