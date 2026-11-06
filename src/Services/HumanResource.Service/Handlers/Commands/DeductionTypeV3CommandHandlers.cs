using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class DeductionTypeV3CommandHandlers :
        IRequestHandler<CreateDeductionTypeV3Command, DeductionTypeV3Dto>,
        IRequestHandler<UpdateDeductionTypeV3Command, bool>,
        IRequestHandler<DeleteDeductionTypeV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;

        public DeductionTypeV3CommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<DeductionTypeV3Dto> Handle(CreateDeductionTypeV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            await Validate(request.Code, request.DeductionType, request.PaymentRunTypeId, 0, command.ClientId, cancellationToken);

            var nextId = (await _context.DeductionTypes.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
            var code = string.IsNullOrWhiteSpace(request.Code) ? nextId.ToString("000") : request.Code.Trim();

            var entity = new EPDeductiontypeLegacy
            {
                Id = nextId,
                Code = code,
                Stxt = request.DeductionType.Trim(),
                Ltxt = request.Description?.Trim(),
                PayRunTypeId = request.PaymentRunTypeId,
                Reimbursed = request.Reimbursed,
                ClientId = command.ClientId,
                UtSysGen = false,
                UtIsPartial = false,
                UtRecordVersion = 0,
                UtBlock = !request.IsActive
            };

            _context.DeductionTypes.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return await BuildDto(entity, cancellationToken);
        }

        public async Task<bool> Handle(UpdateDeductionTypeV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var entity = await _context.DeductionTypes.FirstOrDefaultAsync(x =>
                x.Id == request.Id
                && x.ClientId == command.ClientId, cancellationToken);

            if (entity == null) return false;

            await Validate(request.Code, request.DeductionType, request.PaymentRunTypeId, request.Id, command.ClientId, cancellationToken);

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                entity.Code = request.Code.Trim();
            }

            entity.Stxt = request.DeductionType.Trim();
            entity.Ltxt = request.Description?.Trim();
            entity.PayRunTypeId = request.PaymentRunTypeId;
            entity.Reimbursed = request.Reimbursed;
            entity.UtSysGen = false;
            entity.UtIsPartial = false;
            entity.UtRecordVersion += 1;
            entity.UtBlock = !request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Handle(DeleteDeductionTypeV3Command command, CancellationToken cancellationToken)
        {
            var entity = await _context.DeductionTypes.FirstOrDefaultAsync(x =>
                x.Id == command.Id
                && x.ClientId == command.ClientId
                && !x.UtBlock, cancellationToken);

            if (entity == null) return false;

            entity.UtSysGen = false;
            entity.UtIsPartial = false;
            entity.UtRecordVersion += 1;
            entity.UtBlock = true;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private async Task Validate(
            string? code,
            string deductionType,
            int? paymentRunTypeId,
            int currentId,
            int clientId,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(deductionType))
                throw new InvalidOperationException("Deduction Type cannot be empty.");
            if (!paymentRunTypeId.HasValue)
                throw new InvalidOperationException("Payment Run Type cannot be empty.");

            var duplicateName = await _context.DeductionTypes.AnyAsync(x =>
                x.Stxt == deductionType.Trim()
                && x.Id != currentId
                && x.ClientId == clientId, cancellationToken);
            if (duplicateName) throw new InvalidOperationException("Deduction Type already exists.");

            if (!string.IsNullOrWhiteSpace(code))
            {
                var duplicateCode = await _context.DeductionTypes.AnyAsync(x =>
                    x.Code == code.Trim()
                    && x.Id != currentId
                    && x.ClientId == clientId, cancellationToken);
                if (duplicateCode) throw new InvalidOperationException("Code already exists.");
            }
        }

        private async Task<DeductionTypeV3Dto> BuildDto(EPDeductiontypeLegacy entity, CancellationToken cancellationToken)
        {
            var paymentRunType = entity.PayRunTypeId.HasValue
                ? await _context.Lovs
                    .AsNoTracking()
                    .Where(x => x.Id == entity.PayRunTypeId.Value && !x.UtBlock)
                    .Select(x => x.Stxt)
                    .FirstOrDefaultAsync(cancellationToken)
                : null;

            return new DeductionTypeV3Dto(
                entity.Id,
                entity.Code ?? string.Empty,
                entity.Stxt ?? string.Empty,
                entity.Ltxt,
                entity.PayRunTypeId,
                paymentRunType,
                entity.Reimbursed,
                !entity.UtBlock);
        }
    }
}
