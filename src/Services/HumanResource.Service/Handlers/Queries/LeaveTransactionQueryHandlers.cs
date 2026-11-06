using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class LeaveTransactionV3QueryHandlers :
        IRequestHandler<GetLeaveTransactionListV3Query, List<LeaveTransactionV3Dto>>,
        IRequestHandler<GetScopedLeaveTransactionListV3Query, LeaveTransactionScopeV3Dto>,
        IRequestHandler<GetLeaveTransactionByIdV3Query, LeaveTransactionV3Dto?>
    {
        private const int ApprovedStatusId = 582;
        private const int ManagementEmployeeCategoryId = 267;
        private const int AdminEmployeeCategoryId = 268;
        private const int MaxAttachmentLookupIdsPerBatch = 2000;
        private const double CeoApprovalThresholdDays = 3;
        private readonly HumanResourceDbContext _context;
        private readonly IConfiguration _configuration;

        public LeaveTransactionV3QueryHandlers(HumanResourceDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<List<LeaveTransactionV3Dto>> Handle(GetLeaveTransactionListV3Query request, CancellationToken cancellationToken)
        {
            var items = await MapListAsync(BuildBaseQuery(request.EntityId), cancellationToken);
            return await ApplyAttachmentsAsync(items, request.ClientId, request.EntityId, cancellationToken);
        }

        public async Task<LeaveTransactionScopeV3Dto> Handle(GetScopedLeaveTransactionListV3Query request, CancellationToken cancellationToken)
        {
            var referenceEmployeeId = request.ReferenceEmployeeId
                ?? await GetReferenceEmployeeIdAsync(request.UserId, request.ClientId, cancellationToken);

            var scopeEmployee = referenceEmployeeId.HasValue
                ? await _context.Employees
                    .AsNoTracking()
                    .Include(x => x.Department)
                    .Include(x => x.Designation)
                    .FirstOrDefaultAsync(x => x.Id == referenceEmployeeId.Value && x.UtBlock != true, cancellationToken)
                : null;
            var scopeEmployeeCategory = await GetEmployeeCategoryAsync(scopeEmployee?.EmployeeCategoryId, cancellationToken);

            var query = BuildBaseQuery(request.EntityId);
            var scope = "Self";
            var isAdmin = IsAdminCategory(scopeEmployee?.EmployeeCategoryId, scopeEmployeeCategory);
            var isManagement = IsManagementCategory(scopeEmployee?.EmployeeCategoryId, scopeEmployeeCategory);

            if (isAdmin)
            {
                scope = "Admin";
            }
            else if (isManagement)
            {
                scope = "Management";
                if (IsHrDesignation(scopeEmployee?.Designation?.Stxt))
                {
                    query = ApplyHrDesignationScope(query);
                }
                else if (scopeEmployee?.DepartmentId.HasValue == true)
                {
                    query = query.Where(x => x.Employee != null && x.Employee.DepartmentId == scopeEmployee.DepartmentId.Value);
                }
                else
                {
                    query = query.Where(x => false);
                }
            }
            else if (referenceEmployeeId.HasValue)
            {
                query = query.Where(x => x.EmployeeId == referenceEmployeeId.Value);
            }
            else
            {
                query = query.Where(x => false);
            }

            var items = await MapListAsync(query, cancellationToken);
            if (request.IncludeAttachments)
            {
                items = await ApplyAttachmentsAsync(items, request.ClientId, request.EntityId, cancellationToken);
            }
            var showRequestNo = isAdmin || isManagement;

            return new LeaveTransactionScopeV3Dto(
                showRequestNo,
                scope,
                referenceEmployeeId,
                scopeEmployee?.EmployeeCategoryId,
                GetEmployeeCategoryName(scopeEmployee?.EmployeeCategoryId, scopeEmployeeCategory),
                scopeEmployee?.DepartmentId,
                scopeEmployee?.Department?.DepStxt,
                scopeEmployee?.DesignationId,
                scopeEmployee?.Designation?.Stxt,
                isAdmin,
                isManagement,
                items);
        }

        public async Task<LeaveTransactionV3Dto?> Handle(GetLeaveTransactionByIdV3Query request, CancellationToken cancellationToken)
        {
            var entity = await _context.LeaveTransactions
                .Include(x => x.Employee)
                    .ThenInclude(x => x!.Department)
                .Include(x => x.Employee)
                    .ThenInclude(x => x!.Designation)
                .Include(x => x.Request)
                .Include(x => x.LeaveType)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null) return null;

            var item = MapToDto(
                entity,
                entity.Request?.RequestNo,
                entity.Employee?.EmployeeCode,
                $"{entity.Employee?.FirstName} {entity.Employee?.LastName}".Trim(),
                entity.Employee?.Department?.DepStxt,
                entity.Employee?.Designation?.Stxt,
                GetLeaveUnit(entity.LeaveUnitId),
                entity.LeaveType?.stxt,
                entity.LeaveDays ?? entity.Request?.LeaveDays,
                entity.FromDate ?? entity.Request?.FromDate,
                entity.TillDate ?? entity.Request?.TillDate
            );

            return (await ApplyAttachmentsAsync(
                new List<LeaveTransactionV3Dto> { item },
                entity.ClientId,
                entity.EntityId,
                cancellationToken)).FirstOrDefault();
        }

        private static LeaveTransactionV3Dto MapToDto(
            Entities.ETLeavetranLegacy entity,
            string? requestNo,
            string? employeeCode,
            string? employeeName,
            string? department,
            string? designation,
            string? leaveUnit,
            string? leaveType,
            double? leaveDays,
            System.DateTime? fromDate,
            System.DateTime? tillDate) => new(
            entity.Id,
            entity.TransNo,
            entity.TransDate,
            entity.RequestId,
            requestNo,
            entity.EmployeeId,
            employeeCode,
            employeeName,
            department,
            designation,
            null,
            entity.LeaveUnitId,
            leaveUnit,
            entity.LeaveTypeId,
            leaveType,
            15,
            5,
            10,
            fromDate,
            tillDate,
            leaveDays,
            entity.IsPaid,
            entity.StatusId,
            GetStatusTxt(entity.StatusId),
            entity.AdmApproval,
            RequiresCeoApproval(leaveDays),
            IsLineManagerApproved(entity.StatusId),
            IsCeoApproved(entity.StatusId, entity.AdmApproval),
            IsFullyApproved(leaveDays, entity.StatusId, entity.AdmApproval),
            null,
            null,
            null,
            null
        );

        private static string GetLeaveUnit(int? id) => id switch
        {
            255 => "Days",
            1 => "Days",
            2 => "Hours",
            _ => "Days"
        };

        private static string GetStatusTxt(int? statusId) => statusId switch
        {
            ApprovedStatusId => "Approved",
            583 => "Rejected",
            1217 => "Special Aproval",
            _ => "Pending"
        };

        private static bool RequiresCeoApproval(double? leaveDays)
        {
            return (leaveDays ?? 0) >= CeoApprovalThresholdDays;
        }

        private static bool IsLineManagerApproved(int? statusId)
        {
            return statusId == ApprovedStatusId;
        }

        private static bool HasApprovalValue(int? admApproval) => admApproval.HasValue && admApproval.Value != 0;

        // ETLeavetrans.admApproval is an employee foreign key, not a status ID.
        private static bool IsCeoApproved(int? statusId, int? admApproval) => HasApprovalValue(admApproval);

        private static bool IsFullyApproved(double? leaveDays, int? statusId, int? admApproval) =>
            RequiresCeoApproval(leaveDays)
                ? IsLineManagerApproved(statusId) && IsCeoApproved(statusId, admApproval)
                : IsLineManagerApproved(statusId);

        private IQueryable<Entities.ETLeavetranLegacy> BuildBaseQuery(int? entityId)
        {
            IQueryable<Entities.ETLeavetranLegacy> query = _context.LeaveTransactions
                .AsNoTracking()
                .Include(x => x.Employee)
                    .ThenInclude(x => x!.Department)
                .Include(x => x.Employee)
                    .ThenInclude(x => x!.Designation)
                .Include(x => x.Request)
                .Include(x => x.LeaveType);

            if (entityId.HasValue)
            {
                query = query.Where(x => x.EntityId == entityId.Value);
            }

            return query;
        }

        private async Task<List<LeaveTransactionV3Dto>> MapListAsync(
            IQueryable<Entities.ETLeavetranLegacy> query,
            CancellationToken cancellationToken)
        {
            var items = await query.ToListAsync(cancellationToken);

            return items.Select(x => MapToDto(
                x,
                x.Request?.RequestNo,
                x.Employee?.EmployeeCode,
                $"{x.Employee?.FirstName} {x.Employee?.LastName}".Trim(),
                x.Employee?.Department?.DepStxt,
                x.Employee?.Designation?.Stxt,
                GetLeaveUnit(x.LeaveUnitId),
                x.LeaveType?.stxt,
                x.LeaveDays ?? x.Request?.LeaveDays,
                x.FromDate ?? x.Request?.FromDate,
                x.TillDate ?? x.Request?.TillDate
            )).OrderByDescending(x => x.Id).ToList();
        }

        private async Task<List<LeaveTransactionV3Dto>> ApplyAttachmentsAsync(
            List<LeaveTransactionV3Dto> items,
            int? clientId,
            int? entityId,
            CancellationToken cancellationToken)
        {
            if (!clientId.HasValue || items.Count == 0)
            {
                return items;
            }

            var requestIds = items
                .Select(x => x.RequestId)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .Distinct()
                .ToArray();

            if (requestIds.Length == 0)
            {
                return items;
            }

            var attachments = await GetAttachmentMapAsync(requestIds, clientId.Value, entityId, cancellationToken);

            return items
                .Select(item => item.RequestId.HasValue && attachments.TryGetValue(item.RequestId.Value, out var attachment)
                    ? item with
                    {
                        AttachmentDocumentName = attachment.FirstOrDefault()?.DocumentName,
                        AttachmentUploadPath = attachment.FirstOrDefault()?.UploadPath,
                        AttachmentExternalLink = attachment.FirstOrDefault()?.ExternalLink,
                        Attachments = attachment.Select(x => new LeaveRequestAttachmentV3Dto(
                            x.Id,
                            x.AttachmentNo,
                            x.DocumentTypeId,
                            x.DocumentType,
                            x.DocumentName,
                            x.UploadPath,
                            x.ExternalLink,
                            $"/api/HumanResource/LeaveRequest/v3/attachments/{x.Id}/download"
                        )).ToList()
                    }
                    : item)
                .ToList();
        }

        private async Task<Dictionary<int, List<AttachmentInfo>>> GetAttachmentMapAsync(
            int[] leaveRequestIds,
            int clientId,
            int? entityId,
            CancellationToken cancellationToken)
        {
            if (leaveRequestIds.Length > MaxAttachmentLookupIdsPerBatch)
            {
                var combined = new Dictionary<int, List<AttachmentInfo>>();
                foreach (var batch in leaveRequestIds.Chunk(MaxAttachmentLookupIdsPerBatch))
                {
                    var batchResult = await GetAttachmentMapAsync(batch, clientId, entityId, cancellationToken);
                    foreach (var item in batchResult)
                    {
                        combined[item.Key] = item.Value;
                    }
                }

                return combined;
            }

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync(cancellationToken);
            }

            var idParameters = leaveRequestIds
                .Select((id, index) => new SqlParameter($"@id{index}", id))
                .ToArray();
            var idParameterNames = string.Join(", ", idParameters.Select(x => x.ParameterName));

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = $"""
                SELECT
                    a.id,
                    a.refdocid,
                    ISNULL(a.attchmentno, '') AS attchmentno,
                    COALESCE(a.lovreftypeid, a.doctypeid) AS lovreftypeid,
                    COALESCE(lv.stxt, lv.ltxt, '') AS documenttype,
                    ISNULL(a.docname, '') AS docname,
                    ISNULL(a.uploadpath, '') AS uploadpath,
                    ISNULL(a.externallink, '') AS externallink
                FROM SMAttachments a
                LEFT JOIN lov lv ON lv.id = COALESCE(a.lovreftypeid, a.doctypeid)
                WHERE a.refdocid IN ({idParameterNames})
                  AND a.clientid = @clientId
                  AND (@entityId IS NULL OR a.entityid = @entityId OR a.entityid IS NULL OR a.entityid = 0)
                  AND ISNULL(a.utblock, 0) = 0
                ORDER BY a.refdocid, a.id
            """;

            foreach (var parameter in idParameters)
            {
                cmd.Parameters.Add(parameter);
            }

            cmd.Parameters.Add(new SqlParameter("@clientId", clientId));
            cmd.Parameters.Add(new SqlParameter("@entityId", (object?)entityId ?? DBNull.Value));

            var result = new Dictionary<int, List<AttachmentInfo>>();
            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                if (reader["refdocid"] is DBNull)
                {
                    continue;
                }

                var refDocId = Convert.ToInt32(reader["refdocid"]);
                if (!result.TryGetValue(refDocId, out var list))
                {
                    list = [];
                    result[refDocId] = list;
                }

                list.Add(new AttachmentInfo(
                    Convert.ToInt32(reader["id"]),
                    reader["attchmentno"]?.ToString() ?? string.Empty,
                    reader["lovreftypeid"] is DBNull ? null : Convert.ToInt32(reader["lovreftypeid"]),
                    reader["documenttype"]?.ToString() ?? string.Empty,
                    reader["docname"]?.ToString() ?? string.Empty,
                    reader["uploadpath"]?.ToString() ?? string.Empty,
                    reader["externallink"]?.ToString() ?? string.Empty
                ));
            }

            return result;
        }

        private sealed record AttachmentInfo(
            int Id,
            string AttachmentNo,
            int? DocumentTypeId,
            string DocumentType,
            string DocumentName,
            string UploadPath,
            string ExternalLink
        );

        private async Task<int?> GetReferenceEmployeeIdAsync(int? userId, int? clientId, CancellationToken cancellationToken)
        {
            if (!userId.HasValue || userId.Value <= 0)
                return null;

            var connectionString = _configuration.GetConnectionString("UserManagementDB");
            if (string.IsNullOrWhiteSpace(connectionString))
                return null;

            try
            {
                await using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync(cancellationToken);

                const string sql = @"
SELECT TOP 1 empid
FROM utusers
WHERE userid = @userId
  AND utblock = 0
  AND (@clientId IS NULL OR userclientid = @clientId)";

                await using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@userId", userId.Value);
                command.Parameters.AddWithValue("@clientId", clientId.HasValue ? clientId.Value : System.DBNull.Value);

                var value = await command.ExecuteScalarAsync(cancellationToken);
                return value == null || value == System.DBNull.Value ? null : System.Convert.ToInt32(value);
            }
            catch
            {
                return null;
            }
        }

        private async Task<string?> GetEmployeeCategoryAsync(int? employeeCategoryId, CancellationToken cancellationToken)
        {
            if (!employeeCategoryId.HasValue)
                return null;

            var category = await _context.Lovs
                .AsNoTracking()
                .Where(x => x.Id == employeeCategoryId.Value)
                .Select(x => new { x.Code, x.Stxt, x.Ltxt })
                .FirstOrDefaultAsync(cancellationToken);

            return category == null
                ? null
                : $"{category.Code} {category.Stxt} {category.Ltxt}".Trim();
        }

        private static bool IsAdminCategory(int? employeeCategoryId, string? employeeCategory)
        {
            var text = (employeeCategory ?? "").ToLowerInvariant();
            return employeeCategoryId == AdminEmployeeCategoryId || text.Contains("admin");
        }

        private static bool IsManagementCategory(int? employeeCategoryId, string? employeeCategory)
        {
            var text = (employeeCategory ?? "").ToLowerInvariant();
            return employeeCategoryId == ManagementEmployeeCategoryId || text.Contains("management");
        }

        private static bool IsHrDesignation(string? designation)
        {
            var text = (designation ?? "").Trim().ToLowerInvariant();
            return text.Contains("human resource") || text.Contains("hr");
        }

        private static IQueryable<Entities.ETLeavetranLegacy> ApplyHrDesignationScope(IQueryable<Entities.ETLeavetranLegacy> query)
        {
            return query.Where(x =>
                x.Employee != null &&
                x.Employee.Designation != null &&
                x.Employee.Designation.Stxt != null &&
                (EF.Functions.Like(x.Employee.Designation.Stxt, "%HR%") ||
                 EF.Functions.Like(x.Employee.Designation.Stxt, "%hr%") ||
                 EF.Functions.Like(x.Employee.Designation.Stxt, "%Human Resource%") ||
                 EF.Functions.Like(x.Employee.Designation.Stxt, "%human resource%")));
        }

        private static string? GetEmployeeCategoryName(int? employeeCategoryId, string? employeeCategory)
        {
            if (!string.IsNullOrWhiteSpace(employeeCategory))
                return employeeCategory;

            return employeeCategoryId switch
        {
            ManagementEmployeeCategoryId => "Management",
            AdminEmployeeCategoryId => "Admin",
            269 => "Teacher",
            _ => null
        };
        }
    }
}

