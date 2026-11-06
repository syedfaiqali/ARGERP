using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class EarningTypeV3CommandHandlers :
        IRequestHandler<CreateEarningTypeV3Command, EarningTypeV3Dto>,
        IRequestHandler<UpdateEarningTypeV3Command, bool>,
        IRequestHandler<DeleteEarningTypeV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;

        public EarningTypeV3CommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<EarningTypeV3Dto> Handle(CreateEarningTypeV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            await Validate(request.Code, request.EarningType, request.PaymentRunTypeId, request.TaxMethodId, request.RateBasisId, request.BasisEarningTypeId, 0, command.ClientId, command.EntityId, cancellationToken);

            var nextId = (await _context.EarningTypes.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
            var code = string.IsNullOrWhiteSpace(request.Code) ? nextId.ToString("00") : request.Code.Trim();

            var entity = new EPEarningtypeLegacy
            {
                Id = nextId,
                Code = code,
                Stxt = request.EarningType.Trim(),
                Ltxt = request.Description?.Trim(),
                PayRunTypeId = request.PaymentRunTypeId,
                TaxMethodId = request.TaxMethodId,
                UomId = request.UomId,
                RateBasisId = request.RateBasisId,
                BsSalary = request.BaseSalary,
                EarningTypeId = request.BasisEarningTypeId,
                PaySlipIndex = request.PayslipIndex?.Trim(),
                PaySlipDescription = request.PayslipDescription?.Trim(),
                ClientId = command.ClientId,
                UtSysGen = false,
                UtIsPartial = false,
                UtRecordVersion = 0,
                UtBlock = !request.IsActive
            };

            _context.EarningTypes.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return await BuildDto(entity, cancellationToken);
        }

        public async Task<bool> Handle(UpdateEarningTypeV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var entity = await _context.EarningTypes.FirstOrDefaultAsync(x =>
                x.Id == request.Id
                && x.ClientId == command.ClientId, cancellationToken);

            if (entity == null) return false;

            await Validate(request.Code, request.EarningType, request.PaymentRunTypeId, request.TaxMethodId, request.RateBasisId, request.BasisEarningTypeId, request.Id, command.ClientId, command.EntityId, cancellationToken);

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                entity.Code = request.Code.Trim();
            }
            entity.Stxt = request.EarningType.Trim();
            entity.Ltxt = request.Description?.Trim();
            entity.PayRunTypeId = request.PaymentRunTypeId;
            entity.TaxMethodId = request.TaxMethodId;
            entity.UomId = request.UomId;
            entity.RateBasisId = request.RateBasisId;
            entity.BsSalary = request.BaseSalary;
            entity.EarningTypeId = request.BasisEarningTypeId;
            entity.PaySlipIndex = request.PayslipIndex?.Trim();
            entity.PaySlipDescription = request.PayslipDescription?.Trim();
            entity.UtSysGen = false;
            entity.UtIsPartial = false;
            entity.UtRecordVersion += 1;
            entity.UtBlock = !request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Handle(DeleteEarningTypeV3Command command, CancellationToken cancellationToken)
        {
            var entity = await _context.EarningTypes.FirstOrDefaultAsync(x =>
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
            string earningType,
            int? paymentRunTypeId,
            int? taxMethodId,
            int? rateBasisId,
            int? basisEarningTypeId,
            int currentId,
            int clientId,
            int? entityId,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(earningType))
                throw new InvalidOperationException("Earning Type cannot be empty.");
            if (!paymentRunTypeId.HasValue)
                throw new InvalidOperationException("Payment Run Type cannot be empty.");
            if (!taxMethodId.HasValue)
                throw new InvalidOperationException("Tax Method cannot be empty.");
            if (!rateBasisId.HasValue)
                throw new InvalidOperationException("Rate Basis cannot be empty.");

            var duplicateName = await _context.EarningTypes.AnyAsync(x =>
                x.Stxt == earningType.Trim()
                && x.Id != currentId
                && x.ClientId == clientId, cancellationToken);
            if (duplicateName) throw new InvalidOperationException("Earning Type already exists.");

            if (!string.IsNullOrWhiteSpace(code))
            {
                var duplicateCode = await _context.EarningTypes.AnyAsync(x =>
                    x.Code == code.Trim()
                    && x.Id != currentId
                    && x.ClientId == clientId, cancellationToken);
                if (duplicateCode) throw new InvalidOperationException("Code already exists.");
            }

            if (basisEarningTypeId.HasValue && basisEarningTypeId.Value == currentId && currentId > 0)
                throw new InvalidOperationException("Basis Earning Type cannot be same as current record.");
        }

        private async Task<EarningTypeV3Dto> BuildDto(EPEarningtypeLegacy entity, CancellationToken cancellationToken)
        {
            var lovMap = await _context.Lovs
                .AsNoTracking()
                .Where(x => !x.UtBlock)
                .Select(x => new { x.Id, x.Stxt })
                .ToDictionaryAsync(x => x.Id, x => x.Stxt ?? string.Empty, cancellationToken);

            string? basisName = null;
            if (entity.EarningTypeId.HasValue)
            {
                basisName = await _context.EarningTypes
                    .AsNoTracking()
                    .Where(x => x.Id == entity.EarningTypeId.Value)
                    .Select(x => x.Stxt)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            string? LovName(int? id) => id.HasValue && lovMap.TryGetValue(id.Value, out var name) ? name : null;
            return new EarningTypeV3Dto(
                entity.Id,
                entity.Code ?? string.Empty,
                entity.Stxt ?? string.Empty,
                entity.Ltxt,
                entity.BsSalary ?? false,
                entity.PaySlipIndex,
                entity.PaySlipDescription,
                entity.UomId,
                LovName(entity.UomId),
                entity.PayRunTypeId,
                LovName(entity.PayRunTypeId),
                entity.TaxMethodId,
                LovName(entity.TaxMethodId),
                entity.RateBasisId,
                LovName(entity.RateBasisId),
                entity.EarningTypeId,
                basisName,
                !entity.UtBlock);
        }
    }
}
