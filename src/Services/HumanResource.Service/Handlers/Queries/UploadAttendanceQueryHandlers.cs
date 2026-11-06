using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class UploadAttendanceQueryHandlers :
        IRequestHandler<GetUploadAttendanceLookupsV3Query, UploadAttendanceLookupsV3Dto>,
        IRequestHandler<GetUploadAttendanceEmployeesV3Query, IReadOnlyList<UploadAttendanceEmployeeV3Dto>>
    {
        private readonly HumanResourceDbContext _context;

        public UploadAttendanceQueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<UploadAttendanceLookupsV3Dto> Handle(GetUploadAttendanceLookupsV3Query request, CancellationToken cancellationToken)
        {
            async Task<List<UploadAttendanceLookupOptionV3Dto>> GetLovByType(string vtype)
            {
                return await _context.Lovs
                    .AsNoTracking()
                    .Where(x => x.Vtype == vtype && !x.UtBlock)
                    .OrderBy(x => x.Stxt)
                    .Select(x => new UploadAttendanceLookupOptionV3Dto(
                        x.Id,
                        x.Code ?? string.Empty,
                        x.Stxt ?? string.Empty,
                        x.Ltxt
                    ))
                    .ToListAsync(cancellationToken);
            }

            var memberTypes = await GetLovByType("MEMTP");
            var deviceTypes = await GetLovByType("mtype");
            var fileTypes = await GetLovByType("FTYPE");

            return new UploadAttendanceLookupsV3Dto(memberTypes.Where(x => x.Id != 190).ToList(), deviceTypes, fileTypes);
        }

        public async Task<IReadOnlyList<UploadAttendanceEmployeeV3Dto>> Handle(GetUploadAttendanceEmployeesV3Query request, CancellationToken cancellationToken)
        {
            var query =
                from emp in _context.Employees.AsNoTracking()
                join dep in _context.Departments.AsNoTracking() on emp.DepartmentId equals dep.Id into depJoin
                from dep in depJoin.DefaultIfEmpty()
                join des in _context.Designations.AsNoTracking() on emp.DesignationId equals des.Id into desJoin
                from des in desJoin.DefaultIfEmpty()
                where emp.ClientId == request.ClientId
                      && (!request.EntityId.HasValue || emp.EntityId == request.EntityId.Value)
                      && (emp.UtBlock == null || emp.UtBlock == false)
                orderby emp.EmployeeCode
                select new UploadAttendanceEmployeeV3Dto(
                    emp.Id,
                    emp.EmployeeCode ?? string.Empty,
                    $"{emp.FirstName ?? string.Empty} {emp.LastName ?? string.Empty}".Trim().ToUpperInvariant(),
                    dep != null ? dep.DepStxt ?? string.Empty : string.Empty,
                    des != null ? des.Stxt ?? string.Empty : string.Empty,
                    emp.MachineCode ?? string.Empty
                );

            return await query.ToListAsync(cancellationToken);
        }
    }
}
