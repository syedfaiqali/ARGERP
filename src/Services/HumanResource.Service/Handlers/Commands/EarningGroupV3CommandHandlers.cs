using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class EarningGroupV3CommandHandlers :
        IRequestHandler<CreateEarningGroupV3Command, EarningGroupHeaderV3Dto>,
        IRequestHandler<UpdateEarningGroupV3Command, bool>,
        IRequestHandler<DeleteEarningGroupV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;

        public EarningGroupV3CommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<EarningGroupHeaderV3Dto> Handle(
            CreateEarningGroupV3Command command,
            CancellationToken cancellationToken)
        {
            var request = command.Request;
            await Validate(request.EarningGroup, request.Details, 0, command.ClientId, command.EntityId, cancellationToken);

            var nextHeaderId = (await _context.EarningGroups.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
            var nextDetailId = (await _context.EarningGroupDetails.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0);
            var generatedCode = string.IsNullOrWhiteSpace(request.Code) ? nextHeaderId.ToString("000") : request.Code.Trim();

            var header = new EPEarninggrphdrLegacy
            {
                Id = nextHeaderId,
                code = generatedCode,
                stxt = request.EarningGroup.Trim(),
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

            _context.EarningGroups.Add(header);
            await _context.SaveChangesAsync(cancellationToken);

            return await BuildHeaderDto(header.Id, command.ClientId, command.EntityId, cancellationToken)
                ?? throw new InvalidOperationException("Failed to load saved Earning Group.");
        }

        public async Task<bool> Handle(
            UpdateEarningGroupV3Command command,
            CancellationToken cancellationToken)
        {
            var request = command.Request;

            var header = await _context.EarningGroups
                .Include(x => x.details)
                .FirstOrDefaultAsync(x =>
                    x.Id == request.Id &&
                    x.clientid == command.ClientId,
                    cancellationToken);

            if (header == null) return false;

            await Validate(request.EarningGroup, request.Details, request.Id, command.ClientId, command.EntityId, cancellationToken);

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                header.code = request.Code.Trim();
            }
            header.stxt = request.EarningGroup.Trim();
            header.utblock = !request.IsActive;
            header.utrecordversion += 1;

            var requestIds = request.Details
                .Where(x => x.Id.HasValue && x.Id.Value > 0)
                .Select(x => x.Id!.Value)
                .ToHashSet();

            var toRemove = header.details.Where(x => !requestIds.Contains(x.Id)).ToList();
            _context.EarningGroupDetails.RemoveRange(toRemove);

            var nextDetailId = (await _context.EarningGroupDetails.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0);
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
            DeleteEarningGroupV3Command command,
            CancellationToken cancellationToken)
        {
            var header = await _context.EarningGroups
                .Include(x => x.details)
                .FirstOrDefaultAsync(x =>
                    x.Id == command.Id &&
                    x.clientid == command.ClientId,
                    cancellationToken);

            if (header == null) return false;

            _context.EarningGroupDetails.RemoveRange(header.details);
            _context.EarningGroups.Remove(header);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private async Task Validate(
            string? earningGroup,
            List<CreateEarningGroupDetailV3Request>? details,
            int currentId,
            int clientId,
            int? entityId,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(earningGroup))
                throw new InvalidOperationException("Earning Group cannot be empty.");

            var duplicateName = await _context.EarningGroups.AnyAsync(x =>
                x.stxt == earningGroup.Trim() &&
                x.Id != currentId &&
                x.clientid == clientId,
                cancellationToken);
            if (duplicateName)
                throw new InvalidOperationException($"Earning Group [{earningGroup.Trim()}] already exists.");

            if (details == null || details.Count == 0)
                throw new InvalidOperationException("Detail cannot be empty.");

            var duplicateEarnType = new HashSet<int>();
            var duplicatePaySlipOrder = new HashSet<int>();

            foreach (var d in details.OrderBy(x => x.LineIndex))
            {
                var line = $"Line {d.LineIndex}: ";
                if (!d.EarnTypeId.HasValue)
                    throw new InvalidOperationException(line + "Earning Type cannot be empty.");
                if (!d.RateBasisId.HasValue)
                    throw new InvalidOperationException(line + "Rate Basis cannot be empty.");
                if (!d.StatusId.HasValue)
                    throw new InvalidOperationException(line + "Status cannot be empty.");
                if (!d.Value.HasValue || d.Value.Value <= 0)
                    throw new InvalidOperationException(line + "Value must be greater than 0.");
                if (!d.PaySlipIndex.HasValue || d.PaySlipIndex.Value <= 0)
                    throw new InvalidOperationException(line + "Pay Slip Order must be greater than 0.");

                if (!duplicateEarnType.Add(d.EarnTypeId.Value))
                    throw new InvalidOperationException(line + "Earning Type already exists.");
                if (!duplicatePaySlipOrder.Add(d.PaySlipIndex.Value))
                    throw new InvalidOperationException(line + "Pay Slip Order already exists.");
            }
        }

        private static EPEarninggrpdtlLegacy MapDetail(
            CreateEarningGroupDetailV3Request source,
            int id,
            int headerId,
            int recordVersion)
        {
            return new EPEarninggrpdtlLegacy
            {
                Id = id,
                earngrpid = headerId,
                lineindex = source.LineIndex,
                earntypeid = source.EarnTypeId,
                ratebasisid = source.RateBasisId,
                basisearntypeid = source.BasisEarnTypeId,
                value = source.Value,
                payslipindex = source.PaySlipIndex,
                paysliptxt = source.PaySlipText?.Trim(),
                statusid = source.StatusId,
                absnoteffective = source.AbsNotEffective,
                levnoteffective = source.LevNotEffective,
                isotpart = source.IsOtPart ? 1 : 0,
                utsysgen = false,
                utispartial = false,
                utrecordversion = recordVersion,
                utblock = false,
            };
        }

        private static void ApplyDetail(EPEarninggrpdtlLegacy target, CreateEarningGroupDetailV3Request source)
        {
            target.lineindex = source.LineIndex;
            target.earntypeid = source.EarnTypeId;
            target.ratebasisid = source.RateBasisId;
            target.basisearntypeid = source.BasisEarnTypeId;
            target.value = source.Value;
            target.payslipindex = source.PaySlipIndex;
            target.paysliptxt = source.PaySlipText?.Trim();
            target.statusid = source.StatusId;
            target.absnoteffective = source.AbsNotEffective;
            target.levnoteffective = source.LevNotEffective;
            target.isotpart = source.IsOtPart ? 1 : 0;
            target.utblock = false;
        }

        private async Task<EarningGroupHeaderV3Dto?> BuildHeaderDto(
            int id,
            int clientId,
            int? entityId,
            CancellationToken cancellationToken)
        {
            var header = await _context.EarningGroups
                .AsNoTracking()
                .Include(x => x.details)
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.clientid == clientId,
                    cancellationToken);
            if (header == null) return null;

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

            string? EtName(int? etId) =>
                etId.HasValue && earningTypeMap.TryGetValue(etId.Value, out var nm) ? nm : null;
            string? LovName(int? lovId) =>
                lovId.HasValue && lovMap.TryGetValue(lovId.Value, out var nm) ? nm : null;

            var details = header.details
                .Where(x => !x.utblock)
                .OrderBy(x => x.lineindex)
                .Select(x => new EarningGroupDetailV3Dto(
                    x.Id,
                    x.lineindex ?? 0,
                    x.earntypeid,
                    EtName(x.earntypeid),
                    x.ratebasisid,
                    LovName(x.ratebasisid),
                    x.basisearntypeid,
                    EtName(x.basisearntypeid),
                    x.value,
                    x.payslipindex,
                    x.paysliptxt,
                    x.statusid,
                    LovName(x.statusid),
                    x.absnoteffective,
                    x.levnoteffective ?? false,
                    (x.isotpart ?? 0) > 0
                ))
                .ToList();

            return new EarningGroupHeaderV3Dto(
                header.Id,
                header.code ?? string.Empty,
                header.stxt ?? string.Empty,
                !header.utblock,
                details);
        }
    }
}
