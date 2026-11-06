using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace HumanResource.Service.Handlers.Commands
{
    public sealed class LeaveRequestV3CommandHandlers : 
        IRequestHandler<CreateLeaveRequestV3Command, LeaveRequestV3Dto>,
        IRequestHandler<UpdateLeaveRequestV3Command, bool>,
        IRequestHandler<DeleteLeaveRequestV3Command, bool>
    {
        private const int InternalStorageType = 533;

        private readonly HumanResourceDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public LeaveRequestV3CommandHandlers(HumanResourceDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<LeaveRequestV3Dto> Handle(CreateLeaveRequestV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            await ValidateRequestBeforeDaysAsync(request.LeaveTypeId, request.FromDate, cancellationToken);
            await ValidateSixMonthEligibilityAsync(request.LeaveTypeId, request.EmployeeId, cancellationToken);

            var nextId = await GetNextLeaveRequestIdAsync(cancellationToken);
            
            string requestNo = await GenerateRequestNo(command.EntityId ?? 0, cancellationToken);

            var entity = new ETLeaveRequestLegacy
            {
                Id = nextId,
                RequestNo = requestNo,
                RequestDate = DateTime.Now,
                EmployeeId = request.EmployeeId,
                LeaveTypeId = request.LeaveTypeId,
                LeaveUnitId = request.LeaveUnitId,
                LeaveDays = request.LeaveDays,
                DOB = request.ChildBirthDate,
                InLieuDate = request.InLieuDate,
                FromDate = request.FromDate,
                TillDate = request.TillDate,
                FromTime = request.FromTime,
                TillTime = request.TillTime,
                Reason = request.Reason,
                Remarks = request.Remarks,
                StatusId = 2, // Pending
                ClientId = command.ClientId,
                EntityId = command.EntityId,
                UtBlock = false
            };

            await _context.LeaveRequests.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            if (request.Attachments != null)
            {
                await SyncAttachmentsAsync(nextId, request.Attachments, command.ClientId, command.EntityId, cancellationToken);
            }

            // Re-fetch to include relationships for DTO mapping if needed, 
            // but for simplicity we'll just return what we have and map it.
            return MapToDto(entity);
        }

        public async Task<bool> Handle(UpdateLeaveRequestV3Command command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var entity = await _context.LeaveRequests.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            
            if (entity == null) return false;

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            entity.EmployeeId = request.EmployeeId;
            entity.LeaveTypeId = request.LeaveTypeId;
            entity.LeaveUnitId = request.LeaveUnitId;
            entity.LeaveDays = request.LeaveDays;
            entity.DOB = request.ChildBirthDate;
            entity.InLieuDate = request.InLieuDate;
            entity.FromDate = request.FromDate;
            entity.TillDate = request.TillDate;
            entity.FromTime = request.FromTime;
            entity.TillTime = request.TillTime;
            entity.Reason = request.Reason;
            entity.Remarks = request.Remarks;
            entity.StatusId = request.StatusId;

            if (request.StatusId == 1) // Approved
            {
                entity.ApprovalDate = DateTime.Now;
                // In a real scenario, we might want to set ApprovedById from the current user
            }

            await _context.SaveChangesAsync(cancellationToken);
            if (request.Attachments != null)
            {
                await SyncAttachmentsAsync(request.Id, request.Attachments, command.ClientId, command.EntityId, cancellationToken);
            }
            else
            {
                await InsertAttachmentIfProvidedAsync(
                    request.Id,
                    request.AttachmentDocumentName,
                    request.AttachmentUploadPath,
                    request.AttachmentExternalLink,
                    request.AttachmentDocumentTypeId,
                    command.ClientId,
                    command.EntityId,
                    cancellationToken
                );
            }
            await transaction.CommitAsync(cancellationToken);

            return true;
        }

        private async Task<int> GetNextLeaveRequestIdAsync(CancellationToken cancellationToken)
        {
            return await _context.Database
                .SqlQueryRaw<int>("SELECT ISNULL(MAX([Id]), 0) + 1 AS [Value] FROM [dbo].[ETLeaveRequest]")
                .SingleAsync(cancellationToken);
        }

        private async Task ValidateRequestBeforeDaysAsync(int? leaveTypeId, DateTime? fromDate, CancellationToken cancellationToken)
        {
            if (!leaveTypeId.HasValue || !fromDate.HasValue)
            {
                return;
            }

            var requestBeforeDays = await _context.LeaveTypes
                .AsNoTracking()
                .Where(x => x.Id == leaveTypeId.Value)
                .Select(x => x.requestbefdays)
                .FirstOrDefaultAsync(cancellationToken);

            if (!requestBeforeDays.HasValue)
            {
                return;
            }

            var daysUntilLeave = (fromDate.Value.Date - DateTime.Now.Date).TotalDays;
            if (daysUntilLeave < requestBeforeDays.Value)
            {
                throw new InvalidOperationException(
                    $"In-lieu leave request must be submitted at least {requestBeforeDays.Value} days in advance.");
            }
        }

        private async Task ValidateSixMonthEligibilityAsync(int? leaveTypeId, int? employeeId, CancellationToken cancellationToken)
        {
            if (!leaveTypeId.HasValue || !employeeId.HasValue)
            {
                return;
            }

            var noOfMonth = await _context.LeaveTypes
                .AsNoTracking()
                .Where(x => x.Id == leaveTypeId.Value)
                .Select(x => x.nom)
                .FirstOrDefaultAsync(cancellationToken);

            if (!noOfMonth.HasValue || noOfMonth.Value <= 0)
            {
                return;
            }

            var joinDate = await _context.Employees
                .AsNoTracking()
                .Where(x => x.Id == employeeId.Value)
                .Select(x => x.JoiningDate)
                .FirstOrDefaultAsync(cancellationToken);

            if (!joinDate.HasValue)
            {
                return;
            }

            var currentDate = DateTime.Now.Date;
            var joiningDate = joinDate.Value.Date;
            var duration = ((currentDate.Year - joiningDate.Year) * 12) + currentDate.Month - joiningDate.Month;

            if (currentDate.Day < joiningDate.Day)
            {
                duration--;
            }

            if (duration < noOfMonth.Value)
            {
                throw new InvalidOperationException($"You can't apply for this leave before {noOfMonth.Value} Months.");
            }
        }

        public async Task<bool> Handle(DeleteLeaveRequestV3Command command, CancellationToken cancellationToken)
        {
            var entity = await _context.LeaveRequests.FirstOrDefaultAsync(x => x.Id == command.Id, cancellationToken);
            if (entity == null) return false;

            var isUsedInLeaveTransaction = await _context.LeaveTransactions
                .AsNoTracking()
                .AnyAsync(x => x.RequestId == command.Id && !x.UtBlock, cancellationToken);

            if (isUsedInLeaveTransaction)
            {
                throw new InvalidOperationException("Leave Request is used in Leave Transaction and cannot be deleted.");
            }

            _context.LeaveRequests.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        private async Task<string> GenerateRequestNo(int entityId, CancellationToken cancellationToken)
        {
            string prefix = "LR";
            var lastRequest = await _context.LeaveRequests
                .Where(x => x.EntityId == entityId && x.RequestNo != null && x.RequestNo.StartsWith(prefix))
                .OrderByDescending(x => x.Id)
                .Select(x => x.RequestNo)
                .FirstOrDefaultAsync(cancellationToken);

            int counter = 1;
            if (lastRequest != null && lastRequest.Length > prefix.Length)
            {
                if (int.TryParse(lastRequest.Substring(prefix.Length), out int lastCounter))
                {
                    counter = lastCounter + 1;
                }
            }

            return $"{prefix}{counter}";
        }

        private static LeaveRequestV3Dto MapToDto(ETLeaveRequestLegacy x)
        {
            return new LeaveRequestV3Dto(
                x.Id,
                x.RequestNo,
                x.RequestDate,
                x.EmployeeId,
                null, // EmployeeCode - not available on entity without join
                null, // EmployeeName - not available on entity without join
                x.LeaveTypeId,
                null, // LeaveTypeTxt
                x.LeaveUnitId,
                x.LeaveUnitId == 255 ? "Days" : "Hours",
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

        private async Task InsertAttachmentIfProvidedAsync(
            int leaveRequestId,
            string? attachmentDocumentName,
            string? attachmentUploadPath,
            string? attachmentExternalLink,
            int? attachmentDocumentTypeId,
            int clientId,
            int? entityId,
            CancellationToken cancellationToken)
        {
            var documentName = FirstNonEmpty(
                attachmentDocumentName,
                attachmentUploadPath,
                attachmentExternalLink
            );

            if (string.IsNullOrWhiteSpace(documentName))
            {
                return;
            }

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync(cancellationToken);
            }

            var nextAttachmentId = await GetNextAttachmentIdAsync(cancellationToken);
            var attachmentNo = await GenerateAttachmentNoAsync(clientId, entityId, cancellationToken);
            var attachmentReferenceTypeId = await ResolveAttachmentReferenceTypeIdAsync(cancellationToken);
            var attachmentDocumentTypeIdToUse = attachmentDocumentTypeId.HasValue && attachmentDocumentTypeId.Value > 0
                ? attachmentDocumentTypeId.Value
                : await ResolveAttachmentDocumentTypeIdAsync(documentName, attachmentUploadPath, attachmentExternalLink, cancellationToken);

            var existingAttachmentId = await GetExistingAttachmentIdAsync(leaveRequestId, clientId, entityId, cancellationToken);
            if (existingAttachmentId.HasValue)
            {
                const string updateSql = """
                    UPDATE SMAttachments
                       SET doctypeid = @doctypeid,
                           docname = @docname,
                           browsepath = '',
                           uploadpath = @uploadpath,
                           lovreftypeid = @lovreftypeid,
                           externallink = @externallink,
                           typeid = @typeid,
                           utrecordversion = ISNULL(utrecordversion, 0) + 1,
                           utblock = 0
                     WHERE id = @id
                """;

                await using var updateCmd = conn.CreateCommand();
                updateCmd.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();
                updateCmd.CommandText = updateSql;
                updateCmd.Parameters.Add(new SqlParameter("@id", existingAttachmentId.Value));
                updateCmd.Parameters.Add(new SqlParameter("@doctypeid", attachmentReferenceTypeId));
                updateCmd.Parameters.Add(new SqlParameter("@docname", documentName.Trim()));
                updateCmd.Parameters.Add(new SqlParameter("@uploadpath", (object?)NullIfWhiteSpace(attachmentUploadPath) ?? DBNull.Value));
                updateCmd.Parameters.Add(new SqlParameter("@lovreftypeid", attachmentDocumentTypeIdToUse));
                updateCmd.Parameters.Add(new SqlParameter("@externallink", (object?)NullIfWhiteSpace(attachmentExternalLink) ?? DBNull.Value));
                updateCmd.Parameters.Add(new SqlParameter("@typeid", InternalStorageType));
                _context.PrepareAuditCommand(updateCmd);
                await updateCmd.ExecuteNonQueryAsync(cancellationToken);

                await DeleteDuplicateAttachmentsAsync(leaveRequestId, existingAttachmentId.Value, clientId, entityId, cancellationToken);
                return;
            }

            const string sql = """
                INSERT INTO SMAttachments
                (
                    id, attchmentno, refdocid, doctypeid, docname, browsepath, uploadpath,
                    lovreftypeid, externallink, typeid, entityid, clientid,
                    utblock, utsysgen, utispartial, utrecordversion
                )
                VALUES
                (
                    @id, @attchmentno, @refdocid, @doctypeid, @docname, '',
                    @uploadpath, @lovreftypeid, @externallink, @typeid, @entityid, @clientid,
                    0, 0, 0, 1
                )
            """;

            await using var cmd = conn.CreateCommand();
            cmd.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();
            cmd.CommandText = sql;
            cmd.Parameters.Add(new SqlParameter("@id", nextAttachmentId));
            cmd.Parameters.Add(new SqlParameter("@attchmentno", attachmentNo));
            cmd.Parameters.Add(new SqlParameter("@refdocid", leaveRequestId));
            cmd.Parameters.Add(new SqlParameter("@doctypeid", attachmentReferenceTypeId));
            cmd.Parameters.Add(new SqlParameter("@docname", documentName.Trim()));
            cmd.Parameters.Add(new SqlParameter("@uploadpath", (object?)NullIfWhiteSpace(attachmentUploadPath) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@lovreftypeid", attachmentDocumentTypeIdToUse));
            cmd.Parameters.Add(new SqlParameter("@externallink", (object?)NullIfWhiteSpace(attachmentExternalLink) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@typeid", InternalStorageType));
            cmd.Parameters.Add(new SqlParameter("@entityid", (object?)entityId ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@clientid", clientId));

            _context.PrepareAuditCommand(cmd);

            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }

        private async Task SyncAttachmentsAsync(
            int leaveRequestId,
            IReadOnlyCollection<LeaveRequestAttachmentV3Dto> attachments,
            int clientId,
            int? entityId,
            CancellationToken cancellationToken)
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync(cancellationToken);
            }

            await using (var deleteCmd = conn.CreateCommand())
            {
                deleteCmd.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();
                deleteCmd.CommandText = """
                    UPDATE SMAttachments SET utblock = 1
                     WHERE refdocid = @refdocid
                       AND clientid = @clientId
                       AND (@entityId IS NULL OR entityid = @entityId)
                """;
                deleteCmd.Parameters.Add(new SqlParameter("@refdocid", leaveRequestId));
                deleteCmd.Parameters.Add(new SqlParameter("@clientId", clientId));
                deleteCmd.Parameters.Add(new SqlParameter("@entityId", (object?)entityId ?? DBNull.Value));
                _context.PrepareAuditCommand(deleteCmd);
                await deleteCmd.ExecuteNonQueryAsync(cancellationToken);
            }

            foreach (var attachment in attachments)
            {
                await InsertAttachmentRowAsync(
                    leaveRequestId,
                    attachment.DocumentName,
                    attachment.UploadPath,
                    attachment.ExternalLink,
                    attachment.DocumentTypeId,
                    attachment.FileName,
                    attachment.FileContentBase64,
                    clientId,
                    entityId,
                    cancellationToken);
            }
        }

        private async Task InsertAttachmentRowAsync(
            int leaveRequestId,
            string? attachmentDocumentName,
            string? attachmentUploadPath,
            string? attachmentExternalLink,
            int? attachmentDocumentTypeId,
            string? fileName,
            string? fileContentBase64,
            int clientId,
            int? entityId,
            CancellationToken cancellationToken)
        {
            var savedUploadPath = await SaveAttachmentFileIfProvidedAsync(
                leaveRequestId,
                fileName,
                fileContentBase64,
                cancellationToken);
            if (!string.IsNullOrWhiteSpace(savedUploadPath))
            {
                attachmentUploadPath = savedUploadPath;
                attachmentDocumentName = FirstNonEmpty(attachmentDocumentName, fileName, Path.GetFileName(savedUploadPath));
            }

            var documentName = FirstNonEmpty(
                attachmentDocumentName,
                attachmentUploadPath,
                attachmentExternalLink
            );

            if (string.IsNullOrWhiteSpace(documentName))
            {
                return;
            }

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync(cancellationToken);
            }

            var nextAttachmentId = await GetNextAttachmentIdAsync(cancellationToken);
            var attachmentNo = await GenerateAttachmentNoAsync(clientId, entityId, cancellationToken);
            var attachmentReferenceTypeId = await ResolveAttachmentReferenceTypeIdAsync(cancellationToken);
            var attachmentDocumentTypeIdToUse = attachmentDocumentTypeId.HasValue && attachmentDocumentTypeId.Value > 0
                ? attachmentDocumentTypeId.Value
                : await ResolveAttachmentDocumentTypeIdAsync(documentName, attachmentUploadPath, attachmentExternalLink, cancellationToken);

            const string sql = """
                INSERT INTO SMAttachments
                (
                    id, attchmentno, refdocid, doctypeid, docname, browsepath, uploadpath,
                    lovreftypeid, externallink, typeid, entityid, clientid,
                    utblock, utsysgen, utispartial, utrecordversion
                )
                VALUES
                (
                    @id, @attchmentno, @refdocid, @doctypeid, @docname, '',
                    @uploadpath, @lovreftypeid, @externallink, @typeid, @entityid, @clientid,
                    0, 0, 0, 1
                )
            """;

            await using var cmd = conn.CreateCommand();
            cmd.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();
            cmd.CommandText = sql;
            cmd.Parameters.Add(new SqlParameter("@id", nextAttachmentId));
            cmd.Parameters.Add(new SqlParameter("@attchmentno", attachmentNo));
            cmd.Parameters.Add(new SqlParameter("@refdocid", leaveRequestId));
            cmd.Parameters.Add(new SqlParameter("@doctypeid", attachmentReferenceTypeId));
            cmd.Parameters.Add(new SqlParameter("@docname", documentName.Trim()));
            cmd.Parameters.Add(new SqlParameter("@uploadpath", (object?)NullIfWhiteSpace(attachmentUploadPath) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@lovreftypeid", attachmentDocumentTypeIdToUse));
            cmd.Parameters.Add(new SqlParameter("@externallink", (object?)NullIfWhiteSpace(attachmentExternalLink) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@typeid", InternalStorageType));
            cmd.Parameters.Add(new SqlParameter("@entityid", (object?)entityId ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@clientid", clientId));

            _context.PrepareAuditCommand(cmd);

            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }

        private async Task<string?> SaveAttachmentFileIfProvidedAsync(
            int leaveRequestId,
            string? fileName,
            string? fileContentBase64,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(fileContentBase64))
            {
                return null;
            }

            var commaIndex = fileContentBase64.IndexOf(',');
            var base64 = commaIndex >= 0 ? fileContentBase64[(commaIndex + 1)..] : fileContentBase64;
            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(base64);
            }
            catch (FormatException)
            {
                throw new InvalidOperationException("Attachment file content is invalid.");
            }

            var safeFileName = SanitizeFileName(fileName);
            var relativeDirectory = Path.Combine("liveattachments", "leave-requests", leaveRequestId.ToString());
            var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var absoluteDirectory = Path.Combine(webRoot, relativeDirectory);
            Directory.CreateDirectory(absoluteDirectory);

            var uniqueFileName = $"{Path.GetFileNameWithoutExtension(safeFileName)}_{DateTime.UtcNow:yyyyMMddHHmmssfff}{Path.GetExtension(safeFileName)}";
            var absolutePath = Path.Combine(absoluteDirectory, uniqueFileName);
            await File.WriteAllBytesAsync(absolutePath, bytes, cancellationToken);

            return Path.Combine(relativeDirectory, uniqueFileName).Replace('\\', '/');
        }

        private static string SanitizeFileName(string fileName)
        {
            var name = Path.GetFileName(fileName);
            foreach (var invalid in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(invalid, '_');
            }

            return string.IsNullOrWhiteSpace(name) ? "attachment" : name;
        }

        private async Task<int?> GetExistingAttachmentIdAsync(
            int leaveRequestId,
            int clientId,
            int? entityId,
            CancellationToken cancellationToken)
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync(cancellationToken);
            }

            await using var cmd = conn.CreateCommand();
            cmd.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();
            cmd.CommandText = """
                SELECT TOP 1 id
                FROM SMAttachments
                WHERE refdocid = @refdocid
                  AND clientid = @clientId
                  AND (@entityId IS NULL OR entityid = @entityId)
                  AND ISNULL(utblock, 0) = 0
                ORDER BY id DESC
            """;
            cmd.Parameters.Add(new SqlParameter("@refdocid", leaveRequestId));
            cmd.Parameters.Add(new SqlParameter("@clientId", clientId));
            cmd.Parameters.Add(new SqlParameter("@entityId", (object?)entityId ?? DBNull.Value));

            var scalar = await cmd.ExecuteScalarAsync(cancellationToken);
            return scalar is DBNull or null ? null : Convert.ToInt32(scalar);
        }

        private async Task DeleteDuplicateAttachmentsAsync(
            int leaveRequestId,
            int keepAttachmentId,
            int clientId,
            int? entityId,
            CancellationToken cancellationToken)
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync(cancellationToken);
            }

            await using var cmd = conn.CreateCommand();
            cmd.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();
            cmd.CommandText = """
                UPDATE SMAttachments SET utblock = 1
                 WHERE refdocid = @refdocid
                   AND id <> @keepAttachmentId
                   AND clientid = @clientId
                   AND (@entityId IS NULL OR entityid = @entityId)
            """;
            cmd.Parameters.Add(new SqlParameter("@refdocid", leaveRequestId));
            cmd.Parameters.Add(new SqlParameter("@keepAttachmentId", keepAttachmentId));
            cmd.Parameters.Add(new SqlParameter("@clientId", clientId));
            cmd.Parameters.Add(new SqlParameter("@entityId", (object?)entityId ?? DBNull.Value));

            _context.PrepareAuditCommand(cmd);

            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }

        private async Task<int> ResolveAttachmentReferenceTypeIdAsync(CancellationToken cancellationToken)
        {
            var lovId = await ResolveLovIdByNamesAsync(
                ["Leave Request", "LeaveRequest", "ETLeaveRequest", "Message", "SMMessage", "Communication"],
                cancellationToken
            );

            if (lovId.HasValue)
            {
                return lovId.Value;
            }

            lovId = await GetMostUsedAttachmentLovIdAsync("doctypeid", cancellationToken);
            if (lovId.HasValue)
            {
                return lovId.Value;
            }

            throw new InvalidOperationException("Attachment reference type is not configured in LOV.");
        }

        private async Task<int> ResolveAttachmentDocumentTypeIdAsync(
            string documentName,
            string? attachmentUploadPath,
            string? attachmentExternalLink,
            CancellationToken cancellationToken)
        {
            var extension = Path.GetExtension(FirstNonEmpty(attachmentUploadPath, attachmentExternalLink, documentName) ?? string.Empty)
                .TrimStart('.')
                .Trim();

            var names = extension.ToLowerInvariant() switch
            {
                "pdf" => new[] { "PDF", "Pdf", "Document", "File", "Attachment" },
                "doc" or "docx" => new[] { "Word", "Document", "File", "Attachment" },
                "xls" or "xlsx" => new[] { "Excel", "Document", "File", "Attachment" },
                "jpg" or "jpeg" or "png" or "gif" or "webp" => new[] { "Image", "Picture", "File", "Attachment" },
                _ => new[] { "Document", "File", "Attachment" }
            };

            var lovId = await ResolveLovIdByNamesAsync(names, cancellationToken);
            if (lovId.HasValue)
            {
                return lovId.Value;
            }

            lovId = await GetMostUsedAttachmentLovIdAsync("lovreftypeid", cancellationToken);
            if (lovId.HasValue)
            {
                return lovId.Value;
            }

            throw new InvalidOperationException("Attachment document type is not configured in LOV.");
        }

        private async Task<int?> ResolveLovIdByNamesAsync(string[] names, CancellationToken cancellationToken)
        {
            var normalizedNames = names
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim().ToLowerInvariant())
                .ToArray();

            return await _context.Lovs
                .AsNoTracking()
                .Where(x => !x.UtBlock && (
                    (x.Stxt != null && normalizedNames.Contains(x.Stxt.Trim().ToLower())) ||
                    (x.Code != null && normalizedNames.Contains(x.Code.Trim().ToLower()))
                ))
                .OrderBy(x => x.Id)
                .Select(x => (int?)x.Id)
                .FirstOrDefaultAsync(cancellationToken);
        }

        private async Task<int?> GetMostUsedAttachmentLovIdAsync(string columnName, CancellationToken cancellationToken)
        {
            if (columnName != "doctypeid" && columnName != "lovreftypeid")
            {
                throw new ArgumentOutOfRangeException(nameof(columnName));
            }

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync(cancellationToken);
            }

            await using var cmd = conn.CreateCommand();
            cmd.Transaction = _context.Database.CurrentTransaction?.GetDbTransaction();
            cmd.CommandText = $"""
                SELECT TOP 1 a.{columnName}
                FROM SMAttachments a
                INNER JOIN lov l ON l.Id = a.{columnName}
                WHERE a.{columnName} IS NOT NULL
                  AND ISNULL(a.utblock, 0) = 0
                  AND ISNULL(l.utblock, 0) = 0
                GROUP BY a.{columnName}
                ORDER BY COUNT(1) DESC, a.{columnName}
            """;

            var scalar = await cmd.ExecuteScalarAsync(cancellationToken);
            return scalar is DBNull or null ? null : Convert.ToInt32(scalar);
        }

        private async Task<int> GetNextAttachmentIdAsync(CancellationToken cancellationToken)
        {
            return await _context.Database
                .SqlQueryRaw<int>("SELECT ISNULL(MAX([id]), 0) + 1 AS [Value] FROM [dbo].[SMAttachments]")
                .SingleAsync(cancellationToken);
        }

        private async Task<string> GenerateAttachmentNoAsync(int clientId, int? entityId, CancellationToken cancellationToken)
        {
            var seq = await _context.Database
                .SqlQueryRaw<int>(
                    """
                    SELECT ISNULL(MAX(CAST(RIGHT(ISNULL(attchmentno, 'ATT-000000'), 6) AS int)), 0) + 1 AS [Value]
                    FROM [dbo].[SMAttachments]
                    WHERE clientid = @clientId
                      AND (@entityId IS NULL OR entityid = @entityId)
                    """,
                    new SqlParameter("@clientId", clientId),
                    new SqlParameter("@entityId", (object?)entityId ?? DBNull.Value)
                )
                .SingleAsync(cancellationToken);

            return $"ATT-{seq:D6}";
        }

        private static string? FirstNonEmpty(params string?[] values)
        {
            return values.FirstOrDefault(value => !string.IsNullOrWhiteSpace(value));
        }

        private static string? NullIfWhiteSpace(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static string GetStatusTxt(int? statusId)
        {
            return statusId switch
            {
                1 => "Approved",
                2 => "Pending",
                3 => "Rejected",
                _ => "Pending"
            };
        }
    }
}
