using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class LeaveGroupV3QueryHandlers : 
        IRequestHandler<GetLeaveGroupsV3Query, PaginationResponse<LeaveGroupV3Dto>>,
        IRequestHandler<GetLeaveGroupByIdV3Query, LeaveGroupHeaderV3Dto?>
    {
        private readonly HumanResourceDbContext _context;

        public LeaveGroupV3QueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<LeaveGroupV3Dto>> Handle(GetLeaveGroupsV3Query request, CancellationToken cancellationToken)
        {
            // The list DTO projects header columns only, so the Details collection is
            // deliberately not Included here: loading it multiplied every header row by
            // its detail count on the wire and in memory for data the grid never shows.
            var query = _context.LeaveGroups
                .Where(x => x.ClientId == request.ClientId);

            if (request.EntityId.HasValue)
            {
                query = query.Where(x => x.EntityId == request.EntityId);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(x => 
                    (x.Code != null && x.Code.Contains(request.SearchTerm)) ||
                    (x.Stxt != null && x.Stxt.Contains(request.SearchTerm))
                );
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderBy(x => x.Code)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new LeaveGroupV3Dto
                {
                    Id = x.Id,
                    Code = x.Code,
                    Stxt = x.Stxt,
                    AdjustmentPolicyId = x.AdjustmentPolicyId
                })
                .ToListAsync(cancellationToken);

            // The policy name used to be a correlated subquery inside the projection,
            // which SQL Server ran once per returned row. One batched lookup over just
            // the ids on this page replaces it.
            var policyIds = items
                .Where(x => x.AdjustmentPolicyId.HasValue)
                .Select(x => x.AdjustmentPolicyId!.Value)
                .Distinct()
                .ToList();

            if (policyIds.Count > 0)
            {
                var policyNames = await _context.LeaveAdjustmentPolicies
                    .Where(p => policyIds.Contains(p.Id))
                    .Select(p => new { p.Id, p.stxt })
                    .ToDictionaryAsync(p => p.Id, p => p.stxt, cancellationToken);

                foreach (var item in items)
                {
                    if (item.AdjustmentPolicyId.HasValue &&
                        policyNames.TryGetValue(item.AdjustmentPolicyId.Value, out var name))
                    {
                        item.AdjustmentPolicyName = name;
                    }
                }
            }

            return new PaginationResponse<LeaveGroupV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<LeaveGroupHeaderV3Dto?> Handle(GetLeaveGroupByIdV3Query request, CancellationToken cancellationToken)
        {
            var entity = await _context.LeaveGroups
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.ClientId == request.ClientId && (!request.EntityId.HasValue || x.EntityId == request.EntityId), cancellationToken);

            if (entity == null) return null;

            var result = new LeaveGroupHeaderV3Dto
            {
                Id = entity.Id,
                Code = entity.Code,
                Stxt = entity.Stxt,
                AdjustmentPolicyId = entity.AdjustmentPolicyId,
                AdjustmentPolicyName = await _context.LeaveAdjustmentPolicies
                    .Where(p => p.Id == entity.AdjustmentPolicyId)
                    .Select(p => p.stxt)
                    .FirstOrDefaultAsync(cancellationToken),
                Details = new List<LeaveGroupDetailV3Dto>()
            };

            foreach (var d in entity.Details)
            {
                var detailDto = new LeaveGroupDetailV3Dto
                {
                    Id = d.Id,
                    LeaveTypeId = d.LeaveTypeId,
                    LeaveDays = d.LeaveDays,
                    LeaveTypeName = await _context.LeaveTypes
                        .Where(lt => lt.Id == d.LeaveTypeId)
                        .Select(lt => lt.stxt)
                        .FirstOrDefaultAsync(cancellationToken),
                    LeaveTypeCode = await _context.LeaveTypes
                        .Where(lt => lt.Id == d.LeaveTypeId)
                        .Select(lt => lt.code)
                        .FirstOrDefaultAsync(cancellationToken)
                };
                result.Details.Add(detailDto);
            }

            return result;
        }
    }
}
