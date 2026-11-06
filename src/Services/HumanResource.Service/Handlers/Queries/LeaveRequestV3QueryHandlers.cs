using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HumanResource.Service.Handlers.Queries
{
    public sealed class LeaveRequestV3QueryHandlers : 
        IRequestHandler<GetLeaveRequestsV3Query, PaginationResponse<LeaveRequestV3Dto>>,
        IRequestHandler<GetLeaveRequestByIdV3Query, LeaveRequestV3Dto?>
    {
        private const int MaxAttachmentLookupIdsPerBatch = 2000;
        private readonly HumanResourceDbContext _context;

        public LeaveRequestV3QueryHandlers(HumanResourceDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationResponse<LeaveRequestV3Dto>> Handle(GetLeaveRequestsV3Query request, CancellationToken cancellationToken)
        {
            var query = _context.LeaveRequests
                .Include(x => x.Employee)
                .Include(x => x.LeaveType)
                .Where(x => x.ClientId == request.ClientId);

            if (request.EntityId.HasValue)
            {
                query = query.Where(x => x.EntityId == request.EntityId);
            }

            if (request.StatusId.HasValue)
            {
                query = query.Where(x => x.StatusId == request.StatusId);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(x => 
                    (x.RequestNo != null && x.RequestNo.Contains(request.SearchTerm)) ||
                    (x.Employee != null && (x.Employee.FirstName + " " + x.Employee.LastName).Contains(request.SearchTerm))
                );
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(x => x.Id)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => MapToDto(x))
                .ToListAsync(cancellationToken);

            items = await ApplyAttachmentsAsync(items, request.ClientId, request.EntityId, cancellationToken);

            return new PaginationResponse<LeaveRequestV3Dto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<LeaveRequestV3Dto?> Handle(GetLeaveRequestByIdV3Query request, CancellationToken cancellationToken)
        {
            var entity = await _context.LeaveRequests
                .Include(x => x.Employee)
                .Include(x => x.LeaveType)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.ClientId == request.ClientId && (!request.EntityId.HasValue || x.EntityId == request.EntityId), cancellationToken);

            if (entity == null)
            {
                return null;
            }

            var items = await ApplyAttachmentsAsync([MapToDto(entity)], request.ClientId, request.EntityId, cancellationToken);
            return items.FirstOrDefault();
        }

        private static LeaveRequestV3Dto MapToDto(Entities.ETLeaveRequestLegacy x)
        {
            return new LeaveRequestV3Dto(
                x.Id,
                x.RequestNo,
                x.RequestDate,
                x.EmployeeId,
                x.Employee?.EmployeeCode,
                x.Employee != null ? $"{x.Employee.FirstName} {x.Employee.LastName}".Trim() : null,
                x.LeaveTypeId,
                x.LeaveType?.stxt,
                x.LeaveUnitId,
                GetLeaveUnitTxt(x.LeaveUnitId),
                x.LeaveDays,
                x.DOB,
                x.InLieuDate,
                x.FromDate,
                x.TillDate,
                x.FromTime,
                x.TillTime,
                x.Reason,
                x.Remarks,
                x.StatusId,
                GetStatusTxt(x.StatusId),
                x.ApprovalDate,
                x.ApprovedById
            );
        }

        private async Task<List<LeaveRequestV3Dto>> ApplyAttachmentsAsync(
            List<LeaveRequestV3Dto> items,
            int clientId,
            int? entityId,
            CancellationToken cancellationToken)
        {
            if (items.Count == 0)
            {
                return items;
            }

            var ids = items.Select(x => x.Id).Distinct().ToArray();
            var attachments = await GetAttachmentMapAsync(ids, clientId, entityId, cancellationToken);

            return items
                .Select(item => attachments.TryGetValue(item.Id, out var attachment)
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

        private static string GetLeaveUnitTxt(int? unitId)
        {
            return unitId switch
            {
                255 => "Days",
                256 => "Hours",
                _ => "-"
            };
        }

        private static string GetStatusTxt(int? statusId)
        {
            return statusId switch
            {
                1 => "Approved",
                2 => "Pending", // Usually null or 0 is pending in legacy, but sometimes they use 2
                3 => "Rejected",
                _ => "Pending"
            };
        }
    }
}
