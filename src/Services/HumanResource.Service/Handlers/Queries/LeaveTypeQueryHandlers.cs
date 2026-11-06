using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Queries
{
    public class LeaveTypeQueryHandlers :
        IRequestHandler<GetLeaveTypesV3Query, PaginationResponse<LeaveTypeV3Dto>>,
        IRequestHandler<GetLeaveTypeByIdV3Query, LeaveTypeV3Dto>
    {
        private readonly HumanResourceDbContext _context;

        public LeaveTypeQueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<LeaveTypeV3Dto>> Handle(GetLeaveTypesV3Query query, CancellationToken cancellationToken)
        {
            var q = _context.LeaveTypes
                .Where(x => x.clientid == query.ClientId && (!query.EntityId.HasValue || x.entityid == query.EntityId))
                .AsQueryable();

            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                q = q.Where(x => x.stxt.Contains(query.SearchTerm) || x.code.Contains(query.SearchTerm));
            }

            var totalCount = await q.CountAsync(cancellationToken);
            var items = await q
                .OrderByDescending(x => x.Id)
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(x => new LeaveTypeV3Dto
                {
                    Id = x.Id,
                    Code = x.code,
                    LeaveType = x.stxt,
                    Leaves = x.leaves ?? 0,
                    RenewOn = x.renewonid == null ? null : x.renewonid.ToString(),
                    LeaveUnit = x.lvunitid == null ? null : x.lvunitid.ToString(),
                    Gender = x.genderid == null ? null : x.genderid.ToString(),
                    EntitleOn = x.entitleid == null ? null : x.entitleid.ToString(),
                    SpecialLeave = x.halfadjust == true,
                    NoOfMonth = x.nom == null ? null : x.nom.ToString(),
                    IsActive = x.utblock != true
                })
                .ToListAsync(cancellationToken);

            return new PaginationResponse<LeaveTypeV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            };
        }

        public async Task<LeaveTypeV3Dto> Handle(GetLeaveTypeByIdV3Query query, CancellationToken cancellationToken)
        {
            var x = await _context.LeaveTypes
                .FirstOrDefaultAsync(
                    x => x.Id == query.Id
                        && x.clientid == query.ClientId
                        && (!query.EntityId.HasValue || x.entityid == query.EntityId),
                    cancellationToken);

            if (x == null) return null;

            return new LeaveTypeV3Dto
            {
                Id = x.Id,
                Code = x.code,
                LeaveType = x.stxt,
                Leaves = x.leaves ?? 0,
                RenewOn = x.renewonid?.ToString(),
                LeaveUnit = x.lvunitid?.ToString(),
                MaxAvailUnit = x.availunitid?.ToString(),
                MaxAvail = x.maxavail?.ToString(),
                MaritalStatus = x.maritalid?.ToString(),
                CarryForwardVal = x.carrylv?.ToString(),
                CarryForwardLookup = x.carrylvid?.ToString(),
                EncashmentVal = x.encash?.ToString(),
                EncashmentLookup = x.encashid?.ToString(),
                Gender = x.genderid?.ToString(),
                EntitleOn = x.entitleid?.ToString(),
                AccrualUnit = x.accrualunitid?.ToString(),
                EntitleLeaves = x.accrualleaves?.ToString(),
                AllowInProb = x.allowinprob == true,
                QuotaValidate = x.quotavalidate == true,
                LateAdjustable = x.lateadjust == true,
                EarlyDepAdjustable = x.earlydepadjust == true,
                SpecialLeave = x.halfadjust == true,
                IncludeHolidays = x.incholidays == true,
                ExcludeWeeklyHolidays = x.exclweeklyholidays == true,
                PaidLeave = x.ispaid == true,
                RequestBefore = x.requestbefdays?.ToString(),
                RequestUnit = x.requestunitid?.ToString(),
                NoOfMonth = x.nom?.ToString(),
                IsActive = x.utblock != true
            };
        }

    }
}
