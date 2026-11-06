using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class DeductionGroupV3CommandHandlers :
        IRequestHandler<CreateDeductionGroupV3Command, DeductionGroupHeaderV3Dto>,
        IRequestHandler<UpdateDeductionGroupV3Command, bool>,
        IRequestHandler<DeleteDeductionGroupV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;

        public DeductionGroupV3CommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<DeductionGroupHeaderV3Dto> Handle(
            CreateDeductionGroupV3Command command,
            CancellationToken cancellationToken)
        {
            var request = command.Request;
            await Validate(request.DeductionGroup, request.Details, 0, command.ClientId, cancellationToken);

            var nextHeaderId = (await _context.DeductionGroups.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
            var nextDetailId = (await _context.DeductionGroupDetails.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0);
            var generatedCode = string.IsNullOrWhiteSpace(request.Code) ? nextHeaderId.ToString("000") : request.Code.Trim();

            var header = new EPDeductiongrphdrLegacy
            {
                Id = nextHeaderId,
                code = generatedCode,
                stxt = request.DeductionGroup.Trim(),
                clientid = command.ClientId,
                utsysgen = false,
                utispartial = false,
                utrecordversion = 1,
                utblock = !request.IsActive,
            };

            foreach (var detail in request.Details.OrderBy(x => x.LineIndex))
            {
                nextDetailId += 1;
                header.details.Add(MapDetail(detail, nextDetailId, header.Id, 1));
            }

            _context.DeductionGroups.Add(header);
            await _context.SaveChangesAsync(cancellationToken);

            return await BuildHeaderDto(header.Id, command.ClientId, cancellationToken)
                ?? throw new InvalidOperationException("Failed to load saved Deduction Group.");
        }

        public async Task<bool> Handle(
            UpdateDeductionGroupV3Command command,
            CancellationToken cancellationToken)
        {
            var request = command.Request;

            var header = await _context.DeductionGroups
                .Include(x => x.details)
                .FirstOrDefaultAsync(x =>
                    x.Id == request.Id &&
                    x.clientid == command.ClientId,
                    cancellationToken);

            if (header == null) return false;

            await Validate(request.DeductionGroup, request.Details, request.Id, command.ClientId, cancellationToken);

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                header.code = request.Code.Trim();
            }
            header.stxt = request.DeductionGroup.Trim();
            header.utblock = !request.IsActive;
            header.utrecordversion += 1;

            var requestIds = request.Details
                .Where(x => x.Id.HasValue && x.Id.Value > 0)
                .Select(x => x.Id!.Value)
                .ToHashSet();

            var toRemove = header.details.Where(x => !requestIds.Contains(x.Id)).ToList();
            _context.DeductionGroupDetails.RemoveRange(toRemove);

            var nextDetailId = (await _context.DeductionGroupDetails.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0);
            foreach (var detail in request.Details.OrderBy(x => x.LineIndex))
            {
                if (detail.Id.HasValue && detail.Id.Value > 0)
                {
                    var existing = header.details.FirstOrDefault(x => x.Id == detail.Id.Value);
                    if (existing != null)
                    {
                        ApplyDetail(existing, detail);
                        existing.utrecordversion += 1;
                    }
                }
                else
                {
                    nextDetailId += 1;
                    header.details.Add(MapDetail(detail, nextDetailId, header.Id, 1));
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Handle(
            DeleteDeductionGroupV3Command command,
            CancellationToken cancellationToken)
        {
            var header = await _context.DeductionGroups
                .Include(x => x.details)
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.clientid == command.ClientId,
                    cancellationToken);

            if (header == null) return false;

            _context.DeductionGroupDetails.RemoveRange(header.details);
            _context.DeductionGroups.Remove(header);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private async Task Validate(
            string? deductionGroup,
            List<CreateDeductionGroupDetailV3Request>? details,
            int currentId,
            int clientId,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(deductionGroup))
                throw new InvalidOperationException("Deduction Group cannot be empty.");

            var duplicateName = await _context.DeductionGroups.AnyAsync(x =>
                x.stxt == deductionGroup.Trim() &&
                x.Id != currentId &&
                x.clientid == clientId,
                cancellationToken);
            if (duplicateName)
                throw new InvalidOperationException($"Deduction Group [{deductionGroup.Trim()}] already exists.");

            if (details == null || details.Count == 0)
                throw new InvalidOperationException("Detail cannot be empty.");

            var duplicateDeductionType = new HashSet<int>();
            var duplicatePaySlipOrder = new HashSet<int>();

            foreach (var d in details.OrderBy(x => x.LineIndex))
            {
                var line = $"Line {d.LineIndex}: ";
                if (!d.DeductionTypeId.HasValue)
                    throw new InvalidOperationException(line + "Deduction Type cannot be empty.");
                if (!d.RateBasisId.HasValue)
                    throw new InvalidOperationException(line + "Rate Basis cannot be empty.");
                if (!d.StatusId.HasValue)
                    throw new InvalidOperationException(line + "Status cannot be empty.");
                if (!d.Value.HasValue || d.Value.Value <= 0)
                    throw new InvalidOperationException(line + "Value must be greater than 0.");
                if (!d.PaySlipOrder.HasValue || d.PaySlipOrder.Value <= 0)
                    throw new InvalidOperationException(line + "Pay Slip Order must be greater than 0.");

                if (!duplicateDeductionType.Add(d.DeductionTypeId.Value))
                    throw new InvalidOperationException(line + "Deduction Type already exists.");
                if (!duplicatePaySlipOrder.Add(d.PaySlipOrder.Value))
                    throw new InvalidOperationException(line + "Pay Slip Order already exists.");
            }
        }

        private static EPDeductiongrpdtlLegacy MapDetail(
            CreateDeductionGroupDetailV3Request source,
            int id,
            int headerId,
            int recordVersion)
        {
            return new EPDeductiongrpdtlLegacy
            {
                Id = id,
                dedgrpid = headerId,
                lineindex = source.LineIndex,
                dedtypeid = source.DeductionTypeId,
                ratebasisid = source.RateBasisId,
                basisearntypeid = source.BasisEarningTypeId,
                value = source.Value,
                payslipindex = source.PaySlipOrder,
                paysliptxt = source.PaySlipDescription?.Trim(),
                statusid = source.StatusId,
                utsysgen = false,
                utispartial = false,
                utrecordversion = recordVersion,
                utblock = false,
            };
        }

        private static void ApplyDetail(EPDeductiongrpdtlLegacy target, CreateDeductionGroupDetailV3Request source)
        {
            target.lineindex = source.LineIndex;
            target.dedtypeid = source.DeductionTypeId;
            target.ratebasisid = source.RateBasisId;
            target.basisearntypeid = source.BasisEarningTypeId;
            target.value = source.Value;
            target.payslipindex = source.PaySlipOrder;
            target.paysliptxt = source.PaySlipDescription?.Trim();
            target.statusid = source.StatusId;
            target.utblock = false;
        }

        private async Task<DeductionGroupHeaderV3Dto?> BuildHeaderDto(
            int id,
            int clientId,
            CancellationToken cancellationToken)
        {
            var header = await _context.DeductionGroups
                .AsNoTracking()
                .Include(x => x.details)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.clientid == clientId,
                    cancellationToken);
            if (header == null) return null;

            var deductionTypeMap = await _context.DeductionTypes
                .AsNoTracking()
                .Where(x => x.ClientId == clientId && !x.UtBlock)
                .Select(x => new { x.Id, x.Stxt })
                .ToDictionaryAsync(x => x.Id, x => x.Stxt ?? string.Empty, cancellationToken);

            var earningTypeMap = await _context.EarningTypes
                .AsNoTracking()
                .Where(x => x.ClientId == clientId && !x.UtBlock)
                .Select(x => new { x.Id, x.Stxt })
                .ToDictionaryAsync(x => x.Id, x => x.Stxt ?? string.Empty, cancellationToken);

            var lovMap = await _context.Lovs
                .AsNoTracking()
                .Where(x => !x.UtBlock)
                .Select(x => new { x.Id, x.Stxt })
                .ToDictionaryAsync(x => x.Id, x => x.Stxt ?? string.Empty, cancellationToken);

            string? DeductionName(int? deductionTypeId) =>
                deductionTypeId.HasValue && deductionTypeMap.TryGetValue(deductionTypeId.Value, out var nm) ? nm : null;
            string? EarningName(int? earningTypeId) =>
                earningTypeId.HasValue && earningTypeMap.TryGetValue(earningTypeId.Value, out var nm) ? nm : null;
            string? LovName(int? lovId) =>
                lovId.HasValue && lovMap.TryGetValue(lovId.Value, out var nm) ? nm : null;

            var details = header.details
                .Where(x => !x.utblock)
                .OrderBy(x => x.lineindex)
                .Select(x => new DeductionGroupDetailV3Dto(
                    x.Id,
                    x.lineindex ?? 0,
                    x.dedtypeid,
                    DeductionName(x.dedtypeid),
                    x.ratebasisid,
                    LovName(x.ratebasisid),
                    x.basisearntypeid,
                    EarningName(x.basisearntypeid),
                    x.value,
                    x.payslipindex,
                    x.paysliptxt,
                    x.statusid,
                    LovName(x.statusid)
                ))
                .ToList();

            return new DeductionGroupHeaderV3Dto(
                header.Id,
                header.code ?? string.Empty,
                header.stxt ?? string.Empty,
                !header.utblock,
                details);
        }
    }
}
