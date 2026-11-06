using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class AttendancePolicyV3QueryHandlers :
        IRequestHandler<GetAttendancePoliciesV3Query, List<AttendancePolicyV3Dto>>,
        IRequestHandler<GetAttendancePolicyByIdV3Query, AttendancePolicyHeaderV3Dto?>
    {
        private readonly HumanResourceDbContext _context;

        public AttendancePolicyV3QueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<List<AttendancePolicyV3Dto>> Handle(GetAttendancePoliciesV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.AttendancePolicies
                .Where(x => x.ClientId == request.ClientId && !x.Utblock);

            if (request.EntityId.HasValue)
            {
                query = query.Where(x => x.EntityId == request.EntityId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim();
                query = query.Where(x => (x.Code != null && x.Code.Contains(term)) || (x.Stxt != null && x.Stxt.Contains(term)));
            }

            var result = await (from hdr in query
                                join lov in _context.Lovs on hdr.Policytypeid equals lov.Id into lovs
                                from lov in lovs.DefaultIfEmpty()
                                orderby hdr.Id descending
                                select new AttendancePolicyV3Dto(
                                    hdr.Id,
                                    hdr.Code ?? string.Empty,
                                    hdr.Stxt ?? string.Empty,
                                    hdr.Policytypeid,
                                    lov != null ? lov.Stxt : null,
                                    hdr.Applyoffdays ?? false,
                                    hdr.Noofexcuse ?? 0,
                                    !hdr.Utblock
                                ))
                .ToListAsync(cancellationToken);

            return result;
        }

        public async Task<AttendancePolicyHeaderV3Dto?> Handle(GetAttendancePolicyByIdV3Query request, CancellationToken cancellationToken)
        {
            var header = await _context.AttendancePolicies
                .Include(x => x.PolicyDetails)
                .Include(x => x.AbsentDetails)
                .FirstOrDefaultAsync(x => x.Id == request.Id
                    && x.ClientId == request.ClientId
                    && !x.Utblock
                    && (!request.EntityId.HasValue || x.EntityId == request.EntityId.Value), cancellationToken);

            if (header == null)
            {
                return null;
            }

            var policyTypeName = await _context.Lovs
                .Where(x => x.Id == header.Policytypeid)
                .Select(x => x.Stxt)
                .FirstOrDefaultAsync(cancellationToken);

            var periodTypeIds = header.PolicyDetails.Select(x => x.Periodtypeid)
                .Concat(header.AbsentDetails.Select(x => x.Periodtypeid))
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .Distinct()
                .ToList();

            var valTypeIds = header.PolicyDetails
                .Where(x => x.Valtypeid.HasValue)
                .Select(x => x.Valtypeid!.Value)
                .Distinct()
                .ToList();

            var lovs = await _context.Lovs
                .Where(x => periodTypeIds.Contains(x.Id) || valTypeIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

            var policyDetails = header.PolicyDetails
                .OrderBy(x => x.Lineindex ?? 0)
                .Select(x => new AttendancePolicyDetailV3Dto(
                    x.Id,
                    x.Lineindex ?? 0,
                    x.Starttime,
                    x.Endtime,
                    x.Periodtypeid,
                    lovs.FirstOrDefault(l => l.Id == x.Periodtypeid)?.Stxt,
                    x.Valtypeid,
                    lovs.FirstOrDefault(l => l.Id == x.Valtypeid)?.Stxt,
                    x.Value
                ))
                .ToList();

            var absentDetails = header.AbsentDetails
                .OrderBy(x => x.Lineindex ?? 0)
                .Select(x => new AttendancePolicyAbsentDetailV3Dto(
                    x.Id,
                    x.Lineindex ?? 0,
                    x.Periodtypeid,
                    lovs.FirstOrDefault(l => l.Id == x.Periodtypeid)?.Stxt,
                    x.Policycount,
                    x.Absentcount
                ))
                .ToList();

            return new AttendancePolicyHeaderV3Dto(
                header.Id,
                header.Code ?? string.Empty,
                header.Stxt ?? string.Empty,
                header.Policytypeid,
                policyTypeName,
                header.Applyoffdays ?? false,
                header.Noofexcuse ?? 0,
                !header.Utblock,
                policyDetails,
                absentDetails
            );
        }
    }
}
