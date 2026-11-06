using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class AttendancePolicyV3CommandHandlers :
        IRequestHandler<CreateAttendancePolicyV3Command, AttendancePolicyV3Dto>,
        IRequestHandler<UpdateAttendancePolicyV3Command, bool>,
        IRequestHandler<DeleteAttendancePolicyV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;

        public AttendancePolicyV3CommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<AttendancePolicyV3Dto> Handle(CreateAttendancePolicyV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            await ValidateRequestAsync(request.PolicyTypeId, request.Stxt, request.PolicyDetails, request.AbsentDetails, cancellationToken);

            var nameExists = await _context.AttendancePolicies
                .AnyAsync(x => x.Stxt == request.Stxt.Trim()
                    && x.ClientId == command.ClientId
                    && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value), cancellationToken);
            if (nameExists)
            {
                throw new InvalidOperationException("Policy Name already exists.");
            }

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                var codeExists = await _context.AttendancePolicies
                    .AnyAsync(x => x.Code == request.Code.Trim()
                        && x.ClientId == command.ClientId
                        && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value), cancellationToken);
                if (codeExists)
                {
                    throw new InvalidOperationException("Policy code already exists.");
                }
            }

            var nextId = (await _context.AttendancePolicies.IgnoreQueryFilters()
                .MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;

            var code = string.IsNullOrWhiteSpace(request.Code)
                ? nextId.ToString("0000000000")
                : request.Code.Trim();

            var header = new ETPolicyhdrLegacy
            {
                Id = nextId,
                Code = code,
                Stxt = request.Stxt.Trim(),
                Policytypeid = request.PolicyTypeId,
                Applyoffdays = request.ApplyOffDays,
                Noofexcuse = request.NoOfExcuse,
                EntityId = command.EntityId,
                ClientId = command.ClientId,
                UtSysGen = false,
                UtRecordVersion = 0,
                UtIsPartial = false,
                Utblock = !request.IsActive
            };

            var nextDetailId = (await _context.AttendancePolicyDetails.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
            foreach (var detail in request.PolicyDetails)
            {
                header.PolicyDetails.Add(new ETPolicydtlLegacy
                {
                    Id = nextDetailId++,
                    Policyid = header.Id,
                    Lineindex = detail.LineIndex,
                    Starttime = detail.StartTime,
                    Endtime = detail.EndTime,
                    Periodtypeid = detail.PeriodTypeId,
                    Valtypeid = detail.ValTypeId,
                    Value = detail.Value,
                    UtSysGen = false,
                    UtRecordVersion = 0,
                    UtIsPartial = false,
                    Utblock = false
                });
            }

            var nextAbsentId = (await _context.AttendancePolicyAbsentDetails.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
            foreach (var detail in request.AbsentDetails)
            {
                header.AbsentDetails.Add(new ETAbsentpolicydtlLegacy
                {
                    Id = nextAbsentId++,
                    Policyid = header.Id,
                    Lineindex = detail.LineIndex,
                    Periodtypeid = detail.PeriodTypeId,
                    Policycount = detail.PolicyCount,
                    Absentcount = detail.AbsentCount,
                    UtSysGen = false,
                    UtRecordVersion = 0,
                    UtIsPartial = false,
                    Utblock = false
                });
            }

            _context.AttendancePolicies.Add(header);
            await _context.SaveChangesAsync(cancellationToken);

            var policyTypeName = await _context.Lovs
                .Where(x => x.Id == header.Policytypeid)
                .Select(x => x.Stxt)
                .FirstOrDefaultAsync(cancellationToken);

            return new AttendancePolicyV3Dto(
                header.Id,
                header.Code ?? string.Empty,
                header.Stxt ?? string.Empty,
                header.Policytypeid,
                policyTypeName,
                header.Applyoffdays ?? false,
                header.Noofexcuse ?? 0,
                !header.Utblock);
        }

        public async Task<bool> Handle(UpdateAttendancePolicyV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            await ValidateRequestAsync(request.PolicyTypeId, request.Stxt, request.PolicyDetails, request.AbsentDetails, cancellationToken);

            var header = await _context.AttendancePolicies
                .Include(x => x.PolicyDetails)
                .Include(x => x.AbsentDetails)
                .FirstOrDefaultAsync(x => x.Id == request.Id
                    && x.ClientId == command.ClientId
                    && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value), cancellationToken);

            if (header == null) return false;

            var nameExists = await _context.AttendancePolicies
                .AnyAsync(x => x.Stxt == request.Stxt.Trim()
                    && x.Id != request.Id
                    && x.ClientId == command.ClientId
                    && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value), cancellationToken);
            if (nameExists)
            {
                throw new InvalidOperationException("Policy Name already exists.");
            }

            if (!string.IsNullOrWhiteSpace(request.Code))
            {
                var codeExists = await _context.AttendancePolicies
                    .AnyAsync(x => x.Code == request.Code.Trim()
                        && x.Id != request.Id
                        && x.ClientId == command.ClientId
                        && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value), cancellationToken);
                if (codeExists)
                {
                    throw new InvalidOperationException("Policy code already exists.");
                }
                header.Code = request.Code.Trim();
            }

            header.Stxt = request.Stxt.Trim();
            header.Policytypeid = request.PolicyTypeId;
            header.Applyoffdays = request.ApplyOffDays;
            header.Noofexcuse = request.NoOfExcuse;
            header.UtRecordVersion += 1;
            header.UtIsPartial = false;
            header.Utblock = !request.IsActive;

            _context.AttendancePolicyDetails.RemoveRange(header.PolicyDetails);
            _context.AttendancePolicyAbsentDetails.RemoveRange(header.AbsentDetails);

            var nextDetailId = (await _context.AttendancePolicyDetails.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
            foreach (var detail in request.PolicyDetails)
            {
                header.PolicyDetails.Add(new ETPolicydtlLegacy
                {
                    Id = detail.Id ?? nextDetailId++,
                    Policyid = header.Id,
                    Lineindex = detail.LineIndex,
                    Starttime = detail.StartTime,
                    Endtime = detail.EndTime,
                    Periodtypeid = detail.PeriodTypeId,
                    Valtypeid = detail.ValTypeId,
                    Value = detail.Value,
                    UtSysGen = false,
                    UtRecordVersion = 0,
                    UtIsPartial = false,
                    Utblock = false
                });
            }

            var nextAbsentId = (await _context.AttendancePolicyAbsentDetails.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0) + 1;
            foreach (var detail in request.AbsentDetails)
            {
                header.AbsentDetails.Add(new ETAbsentpolicydtlLegacy
                {
                    Id = detail.Id ?? nextAbsentId++,
                    Policyid = header.Id,
                    Lineindex = detail.LineIndex,
                    Periodtypeid = detail.PeriodTypeId,
                    Policycount = detail.PolicyCount,
                    Absentcount = detail.AbsentCount,
                    UtSysGen = false,
                    UtRecordVersion = 0,
                    UtIsPartial = false,
                    Utblock = false
                });
            }

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Handle(DeleteAttendancePolicyV3Command command, CancellationToken cancellationToken)
        {
            var header = await _context.AttendancePolicies
                .FirstOrDefaultAsync(x => x.Id == command.Id
                    && x.ClientId == command.ClientId
                    && (!command.EntityId.HasValue || x.EntityId == command.EntityId.Value), cancellationToken);

            if (header == null) return false;

            header.Utblock = true;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private async Task ValidateRequestAsync(
            int? policyTypeId,
            string policyName,
            List<AttendancePolicyDetailV3Dto> policyDetails,
            List<AttendancePolicyAbsentDetailV3Dto> absentDetails,
            CancellationToken cancellationToken)
        {
            if (policyTypeId == null)
                throw new InvalidOperationException("Policy Type is required.");

            if (string.IsNullOrWhiteSpace(policyName))
                throw new InvalidOperationException("Policy Name is required.");

            var policyType = await _context.Lovs
                .Where(x => x.Id == policyTypeId.Value && x.Vtype == "ATPOL" && !x.UtBlock)
                .Select(x => new { x.Code, x.Stxt })
                .FirstOrDefaultAsync(cancellationToken);
            var policyKind = GetAttendancePolicyKind(policyType?.Code, policyType?.Stxt);

            var hasPolicyDetails = policyDetails != null && policyDetails.Count > 0;
            var hasAbsentDetails = absentDetails != null && absentDetails.Count > 0;

            if (policyKind == "late" || policyKind == "early")
            {
                if (!hasPolicyDetails)
                    throw new InvalidOperationException("Policy Detail is required.");
                if (!hasAbsentDetails)
                    throw new InvalidOperationException("Absent Policy Detail is required.");
            }

            if (policyKind == "overtime")
            {
                if (!hasPolicyDetails)
                    throw new InvalidOperationException("Policy Detail is required.");
            }

            if (policyKind == "absent")
            {
                if (!hasAbsentDetails)
                    throw new InvalidOperationException("Absent Policy Detail is required.");
            }

            if (hasPolicyDetails)
            {
                foreach (var detail in policyDetails!)
                {
                    if (detail.PeriodTypeId == null)
                        throw new InvalidOperationException("Period Type is required in Policy Detail.");
                    if (detail.ValTypeId == null)
                        throw new InvalidOperationException("Type is required in Policy Detail.");
                    if (detail.Value == null || detail.Value <= 0)
                        throw new InvalidOperationException("Value must be greater than zero in Policy Detail.");
                    if (detail.StartTime == null || detail.EndTime == null)
                        throw new InvalidOperationException("Start Time and End Time are required in Policy Detail.");
                    if (detail.EndTime < detail.StartTime)
                        throw new InvalidOperationException("End Time cannot be less than Start Time in Policy Detail.");
                }
            }

            if (hasAbsentDetails)
            {
                foreach (var detail in absentDetails!)
                {
                    if (detail.PeriodTypeId == null)
                        throw new InvalidOperationException("Period Type is required in Absent Policy Detail.");
                    if (detail.PolicyCount == null || detail.PolicyCount <= 0)
                        throw new InvalidOperationException("Policy Count must be greater than zero in Absent Policy Detail.");
                    if (detail.AbsentCount == null || detail.AbsentCount <= 0)
                        throw new InvalidOperationException("Absent Count must be greater than zero in Absent Policy Detail.");
                }
            }
        }

        private static string GetAttendancePolicyKind(string? code, string? name)
        {
            var normalizedCode = (code ?? string.Empty).Trim().ToUpperInvariant();
            var text = $"{normalizedCode} {(name ?? string.Empty).Trim().ToUpperInvariant()}";

            if (text.Contains("LATE")) return "late";
            if (text.Contains("EARLY")) return "early";
            if (text.Contains("OVERTIME") || normalizedCode == "OT") return "overtime";
            if (text.Contains("ABSENT")) return "absent";
            return string.Empty;
        }
    }
}
