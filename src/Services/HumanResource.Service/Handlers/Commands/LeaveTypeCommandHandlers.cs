using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Commands
{
    public class LeaveTypeCommandHandlers : 
        IRequestHandler<CreateLeaveTypeV3Command, LeaveTypeV3Dto>,
        IRequestHandler<UpdateLeaveTypeV3Command, bool>,
        IRequestHandler<DeleteLeaveTypeV3Command, bool>
    {
        private readonly HumanResourceDbContext _context;

        public LeaveTypeCommandHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<LeaveTypeV3Dto> Handle(CreateLeaveTypeV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var lastId = await _context.LeaveTypes.IgnoreQueryFilters().MaxAsync(x => (int?)x.Id, cancellationToken) ?? 0;

            var entity = new ETLeavetypeLegacy
            {
                Id = lastId + 1,
                code = request.Code,
                stxt = request.LeaveType,
                clientid = command.ClientId,
                entityid = command.EntityId,
                leaves = request.Leaves,
                renewonid = ParseInt(request.RenewOn),
                lvunitid = ParseInt(request.LeaveUnit),
                availunitid = ParseInt(request.MaxAvailUnit),
                maxavail = ParseInt(request.MaxAvail),
                maritalid = ParseInt(request.MaritalStatus),
                carrylv = ParseDouble(request.CarryForwardVal),
                carrylvid = ParseInt(request.CarryForwardLookup),
                encash = ParseDouble(request.EncashmentVal),
                encashid = ParseInt(request.EncashmentLookup),
                genderid = ParseInt(request.Gender),
                entitleid = ParseInt(request.EntitleOn),
                accrualunitid = ParseInt(request.AccrualUnit),
                accrualleaves = ParseDouble(request.EntitleLeaves),
                allowinprob = request.AllowInProb,
                quotavalidate = request.QuotaValidate,
                lateadjust = request.LateAdjustable,
                earlydepadjust = request.EarlyDepAdjustable,
                halfadjust = request.SpecialLeave,
                incholidays = request.IncludeHolidays,
                exclweeklyholidays = request.ExcludeWeeklyHolidays,
                ispaid = request.PaidLeave,
                requestbefdays = ParseInt(request.RequestBefore),
                requestunitid = ParseInt(request.RequestUnit),
                nom = ParseInt(request.NoOfMonth),
                utblock = !request.IsActive
            };

            await _context.LeaveTypes.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return MapToDto(entity);
        }

        public async Task<bool> Handle(UpdateLeaveTypeV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var entity = await _context.LeaveTypes.FirstOrDefaultAsync(
                x => x.Id == request.Id
                    && x.clientid == command.ClientId
                    && (!command.EntityId.HasValue || x.entityid == command.EntityId),
                cancellationToken);

            if (entity == null) return false;

            entity.code = request.Code;
            entity.stxt = request.LeaveType;
            entity.leaves = request.Leaves;
            entity.renewonid = ParseInt(request.RenewOn);
            entity.lvunitid = ParseInt(request.LeaveUnit);
            entity.availunitid = ParseInt(request.MaxAvailUnit);
            entity.maxavail = ParseInt(request.MaxAvail);
            entity.maritalid = ParseInt(request.MaritalStatus);
            entity.carrylv = ParseDouble(request.CarryForwardVal);
            entity.carrylvid = ParseInt(request.CarryForwardLookup);
            entity.encash = ParseDouble(request.EncashmentVal);
            entity.encashid = ParseInt(request.EncashmentLookup);
            entity.genderid = ParseInt(request.Gender);
            entity.entitleid = ParseInt(request.EntitleOn);
            entity.accrualunitid = ParseInt(request.AccrualUnit);
            entity.accrualleaves = ParseDouble(request.EntitleLeaves);
            entity.allowinprob = request.AllowInProb;
            entity.quotavalidate = request.QuotaValidate;
            entity.lateadjust = request.LateAdjustable;
            entity.earlydepadjust = request.EarlyDepAdjustable;
            entity.halfadjust = request.SpecialLeave;
            entity.incholidays = request.IncludeHolidays;
            entity.exclweeklyholidays = request.ExcludeWeeklyHolidays;
            entity.ispaid = request.PaidLeave;
            entity.requestbefdays = ParseInt(request.RequestBefore);
            entity.requestunitid = ParseInt(request.RequestUnit);
            entity.nom = ParseInt(request.NoOfMonth);
            entity.utblock = !request.IsActive;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> Handle(DeleteLeaveTypeV3Command command, CancellationToken cancellationToken)
        {
            var entity = await _context.LeaveTypes.FirstOrDefaultAsync(
                x => x.Id == command.Id
                    && x.clientid == command.ClientId
                    && (!command.EntityId.HasValue || x.entityid == command.EntityId),
                cancellationToken);
            if (entity == null) return false;

            // Leave type IDs are referenced by legacy records. Check these first so a
            // foreign-key violation never reaches the client as an unhelpful 500.
            var usedIn = new List<string>();

            if (await _context.LeaveRequests.AnyAsync(x => x.LeaveTypeId == entity.Id, cancellationToken))
                usedIn.Add("Leave Requests");

            if (await _context.LeaveTransactions.AnyAsync(x => x.LeaveTypeId == entity.Id, cancellationToken))
                usedIn.Add("Leave Transactions");

            if (await _context.LeaveGroupDetails.AnyAsync(x => x.LeaveTypeId == entity.Id, cancellationToken))
                usedIn.Add("Leave Group");

            if (await _context.LeaveAdjustmentPolicyDetails.AnyAsync(x => x.leavetypeid == entity.Id, cancellationToken))
                usedIn.Add("Leave Adjustment Policy");

            if (usedIn.Count > 0)
                throw new InvalidOperationException(
                    $"Cannot delete Leave Type because it is used in: {string.Join(", ", usedIn)}.");

            _context.LeaveTypes.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private LeaveTypeV3Dto MapToDto(ETLeavetypeLegacy entity)
        {
            return new LeaveTypeV3Dto
            {
                Id = entity.Id,
                Code = entity.code,
                LeaveType = entity.stxt,
                Leaves = entity.leaves ?? 0,
                RenewOn = entity.renewonid?.ToString(),
                LeaveUnit = entity.lvunitid?.ToString(),
                MaxAvailUnit = entity.availunitid?.ToString(),
                MaxAvail = entity.maxavail?.ToString(),
                MaritalStatus = entity.maritalid?.ToString(),
                CarryForwardVal = entity.carrylv?.ToString(),
                CarryForwardLookup = entity.carrylvid?.ToString(),
                EncashmentVal = entity.encash?.ToString(),
                EncashmentLookup = entity.encashid?.ToString(),
                Gender = entity.genderid?.ToString(),
                EntitleOn = entity.entitleid?.ToString(),
                AccrualUnit = entity.accrualunitid?.ToString(),
                EntitleLeaves = entity.accrualleaves?.ToString(),
                AllowInProb = entity.allowinprob == true,
                QuotaValidate = entity.quotavalidate == true,
                LateAdjustable = entity.lateadjust == true,
                EarlyDepAdjustable = entity.earlydepadjust == true,
                SpecialLeave = entity.halfadjust == true,
                IncludeHolidays = entity.incholidays == true,
                ExcludeWeeklyHolidays = entity.exclweeklyholidays == true,
                PaidLeave = entity.ispaid == true,
                RequestBefore = entity.requestbefdays?.ToString(),
                RequestUnit = entity.requestunitid?.ToString(),
                NoOfMonth = entity.nom?.ToString(),
                IsActive = entity.utblock != true
            };
        }

        private int? ParseInt(string? val) => int.TryParse(val, out var res) ? res : null;
        private double? ParseDouble(string? val) => double.TryParse(val, out var res) ? res : null;
    }
}
