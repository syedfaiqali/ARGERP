using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class LeaveAdjustmentPolicyQueryHandlers :
        IRequestHandler<GetLeaveAdjustmentPoliciesV3Query, PaginationResponse<LeaveAdjustmentPolicyV3Dto>>,
        IRequestHandler<GetLeaveAdjustmentPolicyByIdV3Query, LeaveAdjustmentPolicyV3Dto?>
    {
        private readonly HumanResourceDbContext _context;

        public LeaveAdjustmentPolicyQueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<LeaveAdjustmentPolicyV3Dto>> Handle(GetLeaveAdjustmentPoliciesV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.LeaveAdjustmentPolicies
                .Where(x => x.clientid == request.ClientId);

            if (request.EntityId.HasValue)
            {
                query = query.Where(x => x.entityid == request.EntityId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(x => (x.code != null && x.code.Contains(term)) || (x.stxt != null && x.stxt.Contains(term)));
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(x => x.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new LeaveAdjustmentPolicyV3Dto
                {
                    Id = x.Id,
                    Code = x.code,
                    Stxt = x.stxt
                })
                .ToListAsync(cancellationToken);

            return new PaginationResponse<LeaveAdjustmentPolicyV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<LeaveAdjustmentPolicyV3Dto?> Handle(GetLeaveAdjustmentPolicyByIdV3Query request, CancellationToken cancellationToken)
        {
            var header = await _context.LeaveAdjustmentPolicies
                .Include(x => x.Details)
                .FirstOrDefaultAsync(x => x.Id == request.Id
                    && x.clientid == request.ClientId
                    && (!request.EntityId.HasValue || x.entityid == request.EntityId.Value), cancellationToken);

            if (header == null) return null;

            var leaveTypes = await _context.LeaveTypes
                .Select(x => new { x.Id, x.stxt })
                .ToDictionaryAsync(x => x.Id, x => x.stxt, cancellationToken);
            var lovs = await _context.Lovs
                .Where(x => x.Vtype == "ADJP" || x.Vtype == "PRIT")
                .Select(x => new { x.Id, x.Stxt })
                .ToDictionaryAsync(x => x.Id, x => x.Stxt, cancellationToken);
            var atnPolicies = await _context.AttendancePolicies
                .Select(x => new { x.Id, x.Stxt })
                .ToDictionaryAsync(x => x.Id, x => x.Stxt, cancellationToken);

            return new LeaveAdjustmentPolicyV3Dto
            {
                Id = header.Id,
                Code = header.code,
                Stxt = header.stxt,
                Details = header.Details
                    .OrderBy(d => d.Id)
                    .Select(d => new LeaveAdjustmentPolicyDetailV3Dto
                    {
                        Id = d.Id,
                        LeaveTypeId = d.leavetypeid,
                        LeaveTypeName = d.leavetypeid.HasValue && leaveTypes.ContainsKey(d.leavetypeid.Value)
                            ? leaveTypes[d.leavetypeid.Value]
                            : null,
                        AdjTypeId = d.adjtypeid,
                        AdjTypeName = d.adjtypeid.HasValue && lovs.ContainsKey(d.adjtypeid.Value)
                            ? lovs[d.adjtypeid.Value]
                            : null,
                        BasedOnId = d.basedonid,
                        BasedOnName = d.basedonid.HasValue && atnPolicies.ContainsKey(d.basedonid.Value)
                            ? atnPolicies[d.basedonid.Value]
                            : null,
                        PriorityId = d.priorityid,
                        PriorityName = d.priorityid.HasValue && lovs.ContainsKey(d.priorityid.Value)
                            ? lovs[d.priorityid.Value]
                            : null,
                        Days = d.days
                    }).ToList()
            };
        }
    }
}
