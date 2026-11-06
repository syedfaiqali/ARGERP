using System.Data;
using System.Net.Mime;
using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Route("api/EmployeeDocuments/v3")]
    [Route("api/HumanResource/EmployeeDocuments/v3")]
    [Authorize]
    public sealed class EmployeeDocumentV3Controller : ControllerBase
    {
        private const int DocumentTypeReferenceType = 756;
        private const int InternalStorageType = 533;
        private readonly HumanResourceDbContext _context;

        public EmployeeDocumentV3Controller(HumanResourceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<object>> GetEmployeeAttachments([FromQuery] int employeeId, CancellationToken cancellationToken)
        {
            if (employeeId <= 0) return BadRequest("Valid employeeId is required.");

            var rows = await LoadAttachmentsByEmployee(employeeId, cancellationToken);
            return Ok(new { items = rows });
        }

        [HttpGet("document-types")]
        public async Task<ActionResult<object>> GetDocumentTypes(CancellationToken cancellationToken)
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync(cancellationToken);

            const string sql = """
                SELECT
                    dt.Id AS documenttypeid,
                    ISNULL(dt.code, '') AS code,
                    ISNULL(dt.stxt, '') AS documenttype,
                    ISNULL(dt.ltxt, '') AS description,
                    0 AS empdocid,
                    0 AS attachmentcount
                FROM HRDocType dt
                WHERE dt.clientid = @clientId
                  AND ISNULL(dt.utblock, 0) = 0
                ORDER BY dt.stxt
            """;

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.Add(new SqlParameter("@clientId", GetClientId()));

            var rows = new List<EmployeeDocumentTypeV3Dto>();
            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                rows.Add(new EmployeeDocumentTypeV3Dto(
                    Convert.ToInt32(reader["documenttypeid"]),
                    reader["code"]?.ToString() ?? string.Empty,
                    reader["documenttype"]?.ToString() ?? string.Empty,
                    reader["description"]?.ToString() ?? string.Empty,
                    Convert.ToInt32(reader["empdocid"]),
                    Convert.ToInt32(reader["attachmentcount"])
                ));
            }

            return Ok(new { items = rows });
        }

        [HttpGet("{employeeDocumentId:int}/attachments")]
        public async Task<ActionResult<object>> GetAttachments(int employeeDocumentId, CancellationToken cancellationToken)
        {
            if (employeeDocumentId <= 0) return Ok(new { items = Array.Empty<EmployeeDocumentAttachmentV3Dto>() });

            var rows = await LoadAttachments(employeeDocumentId, cancellationToken);
            return Ok(new { items = rows });
        }

        [HttpPost("attachments")]
        public async Task<ActionResult<EmployeeDocumentAttachmentV3Dto>> CreateAttachment(
            [FromBody] SaveEmployeeDocumentAttachmentV3Request request,
            CancellationToken cancellationToken)
        {
            int employeeDocumentId;
            try
            {
                employeeDocumentId = await EnsureEmployeeDocument(request, cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }

            var filePayload = DecodeFileIfProvided(request);
            var uploadPath = filePayload?.FileName ?? NormalizeNullable(request.UploadPath);
            var nextId = await GetNextInt("SMAttachments", "id", cancellationToken);
            var attachmentNo = string.IsNullOrWhiteSpace(request.AttachmentNo)
                ? await GenerateAttachmentNo(cancellationToken)
                : request.AttachmentNo.Trim();

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync(cancellationToken);

            const string sql = """
                INSERT INTO SMAttachments
                (
                    id, attchmentno, refdocid, doctypeid, docname, browsepath, uploadpath,
                    lovreftypeid, externallink, typeid, entityid, clientid,
                    utblock, utsysgen, utispartial, utrecordversion
                )
                VALUES
                (
                    @id, @attachmentNo, @refdocid, @doctypeid, @docname, @uploadpath,
                    @uploadpath, @lovreftypeid, @externallink, @typeid, @entityid, @clientid,
                    0, 0, 0, 1
                )
            """;

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            AddAttachmentParameters(cmd, nextId, attachmentNo, employeeDocumentId, request, uploadPath);
            _context.PrepareAuditCommand(cmd);
            await cmd.ExecuteNonQueryAsync(cancellationToken);
            if (filePayload != null)
                await UpsertAttachmentFile(nextId, filePayload, cancellationToken);

            return Ok((await LoadAttachments(employeeDocumentId, cancellationToken)).First(x => x.Id == nextId));
        }

        [HttpPut("attachments/{attachmentId:int}")]
        public async Task<ActionResult<EmployeeDocumentAttachmentV3Dto>> UpdateAttachment(
            int attachmentId,
            [FromBody] SaveEmployeeDocumentAttachmentV3Request request,
            CancellationToken cancellationToken)
        {
            if (attachmentId <= 0) return BadRequest("Valid attachmentId is required.");

            int employeeDocumentId;
            try
            {
                employeeDocumentId = await EnsureEmployeeDocument(request, cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }

            var existingUploadPath = await GetAttachmentUploadPath(attachmentId, cancellationToken);
            var filePayload = DecodeFileIfProvided(request);
            var uploadPath = filePayload?.FileName ?? NormalizeNullable(request.UploadPath) ?? existingUploadPath;

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync(cancellationToken);

            const string sql = """
                UPDATE SMAttachments
                   SET attchmentno = @attachmentNo,
                       refdocid = @refdocid,
                       docname = @docname,
                       uploadpath = @uploadpath,
                       lovreftypeid = @lovreftypeid,
                       externallink = @externallink,
                       typeid = @typeid
                 WHERE id = @id
                   AND doctypeid = @doctypeid
                   AND clientid = @clientid
                   AND ISNULL(utblock, 0) = 0
            """;

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            AddAttachmentParameters(cmd, attachmentId, request.AttachmentNo?.Trim(), employeeDocumentId, request, uploadPath);
            _context.PrepareAuditCommand(cmd);
            var affected = await cmd.ExecuteNonQueryAsync(cancellationToken);
            if (affected == 0) return NotFound();
            if (filePayload != null)
                await UpsertAttachmentFile(attachmentId, filePayload, cancellationToken);

            return Ok((await LoadAttachments(employeeDocumentId, cancellationToken)).First(x => x.Id == attachmentId));
        }

        [HttpDelete("{employeeDocumentId:int}/attachments/{attachmentId:int}")]
        public async Task<IActionResult> DeleteAttachment(int employeeDocumentId, int attachmentId, CancellationToken cancellationToken)
        {
            return await DeleteAttachmentById(attachmentId, employeeDocumentId, cancellationToken);
        }

        [HttpDelete("attachments/{attachmentId:int}")]
        public async Task<IActionResult> DeleteAttachment(int attachmentId, CancellationToken cancellationToken)
        {
            return await DeleteAttachmentById(attachmentId, null, cancellationToken);
        }

        private async Task<IActionResult> DeleteAttachmentById(int attachmentId, int? employeeDocumentId, CancellationToken cancellationToken)
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync(cancellationToken);

            const string sql = """
                UPDATE SMAttachments
                   SET utblock = 1
                 WHERE id = @id
                   AND (@refdocid IS NULL OR refdocid = @refdocid)
                   AND doctypeid = @doctypeid
                   AND clientid = @clientid
                   AND ISNULL(utblock, 0) = 0
            """;

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.Add(new SqlParameter("@id", attachmentId));
            cmd.Parameters.Add(new SqlParameter("@refdocid", (object?)employeeDocumentId ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@doctypeid", DocumentTypeReferenceType));
            cmd.Parameters.Add(new SqlParameter("@clientid", GetClientId()));

            _context.PrepareAuditCommand(cmd);
            var affected = await cmd.ExecuteNonQueryAsync(cancellationToken);
            if (affected <= 0) return NotFound();

            return NoContent();
        }

        [HttpGet("attachments/{attachmentId:int}/download")]
        public async Task<IActionResult> DownloadAttachment(int attachmentId, CancellationToken cancellationToken)
        {
            var info = await GetAttachmentFileInfo(attachmentId, cancellationToken);
            if (info == null) return NotFound("Attachment not found.");
            var attachmentInfo = info.Value;
            if (attachmentInfo.FileData is not { Length: > 0 })
                return NotFound("Attachment file was not found in database.");

            var fileName = attachmentInfo.FileName
                ?? Path.GetFileName((attachmentInfo.UploadPath ?? string.Empty).Replace('\\', '/'))
                ?? attachmentInfo.DocumentName
                ?? "document";
            var contentType = string.IsNullOrWhiteSpace(attachmentInfo.ContentType)
                ? ResolveContentType(fileName)
                : attachmentInfo.ContentType;

            return File(attachmentInfo.FileData, contentType, fileName);
        }

        private async Task<int> EnsureEmployeeDocument(SaveEmployeeDocumentAttachmentV3Request request, CancellationToken cancellationToken)
        {
            if (request.EmployeeId <= 0) throw new InvalidOperationException("Employee is required.");
            if (string.IsNullOrWhiteSpace(request.DocumentName)) throw new InvalidOperationException("Document Name is required.");

            var clientId = GetClientId();
            var documentTypeId = request.DocumentTypeId > 0
                ? request.DocumentTypeId
                : await ResolveDefaultDocumentTypeId(clientId, cancellationToken);

            var documentTypeExists = await _context.DocumentTypes.AsNoTracking()
                .AnyAsync(x => x.Id == documentTypeId && x.ClientId == clientId && x.UtBlock != true, cancellationToken);
            if (!documentTypeExists) throw new InvalidOperationException("Selected Document Type is not valid.");

            request.DocumentTypeId = documentTypeId;

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync(cancellationToken);

            await using (var findCmd = conn.CreateCommand())
            {
                findCmd.CommandText = """
                    SELECT TOP 1 Id
                    FROM EREmpDocument
                    WHERE employeeid = @employeeId
                      AND documenttypeid = @documentTypeId
                      AND ISNULL(utblock, 0) = 0
                    ORDER BY Id DESC
                """;
                findCmd.Parameters.Add(new SqlParameter("@employeeId", request.EmployeeId));
                findCmd.Parameters.Add(new SqlParameter("@documentTypeId", documentTypeId));
                var existing = await findCmd.ExecuteScalarAsync(cancellationToken);
                if (existing is not null && existing is not DBNull) return Convert.ToInt32(existing);
            }

            var nextId = await GetNextInt("EREmpDocument", "Id", cancellationToken);
            await using var insertCmd = conn.CreateCommand();
            insertCmd.CommandText = """
                INSERT INTO EREmpDocument
                (
                    Id, documenttypeid, employeeid, utdoctype, utsubdoctype,
                    utsysgen, utblock, utispartial, utrecordversion
                )
                VALUES
                (
                    @id, @documentTypeId, @employeeId, NULL, NULL,
                    0, 0, 0, 1
                )
            """;
            insertCmd.Parameters.Add(new SqlParameter("@id", nextId));
            insertCmd.Parameters.Add(new SqlParameter("@documentTypeId", documentTypeId));
            insertCmd.Parameters.Add(new SqlParameter("@employeeId", request.EmployeeId));
            _context.PrepareAuditCommand(insertCmd);
            await insertCmd.ExecuteNonQueryAsync(cancellationToken);

            return nextId;
        }

        private async Task<int> ResolveDefaultDocumentTypeId(int clientId, CancellationToken cancellationToken)
        {
            var defaultDocumentTypeId = await _context.DocumentTypes.AsNoTracking()
                .Where(x => x.ClientId == clientId && x.UtBlock != true)
                .OrderBy(x => x.Id)
                .Select(x => x.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (defaultDocumentTypeId <= 0)
                throw new InvalidOperationException("Please create a Document Type in Human Resource Setup before uploading employee documents.");

            return defaultDocumentTypeId;
        }

        private async Task<IReadOnlyList<EmployeeDocumentAttachmentV3Dto>> LoadAttachments(int employeeDocumentId, CancellationToken cancellationToken)
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync(cancellationToken);

            const string sql = """
                SELECT
                    a.id,
                    ISNULL(a.attchmentno, '') AS attachmentno,
                    ed.Id AS empdocid,
                    ISNULL(ed.documenttypeid, 0) AS documenttypeid,
                    ISNULL(dt.stxt, '') AS documenttype,
                    ISNULL(a.docname, '') AS docname,
                    ISNULL(a.uploadpath, '') AS uploadpath,
                    ISNULL(a.externallink, '') AS externallink
                FROM SMAttachments a
                INNER JOIN EREmpDocument ed ON ed.Id = a.refdocid
                LEFT JOIN HRDocType dt ON dt.Id = ed.documenttypeid
                WHERE a.refdocid = @employeeDocumentId
                  AND ISNULL(ed.utblock, 0) = 0
                  AND a.doctypeid = @referenceType
                  AND a.clientid = @clientId
                  AND ISNULL(a.utblock, 0) = 0
                ORDER BY a.id DESC
            """;

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.Add(new SqlParameter("@employeeDocumentId", employeeDocumentId));
            cmd.Parameters.Add(new SqlParameter("@referenceType", DocumentTypeReferenceType));
            cmd.Parameters.Add(new SqlParameter("@clientId", GetClientId()));

            var rows = new List<EmployeeDocumentAttachmentV3Dto>();
            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                rows.Add(new EmployeeDocumentAttachmentV3Dto(
                    Convert.ToInt32(reader["id"]),
                    reader["attachmentno"]?.ToString() ?? string.Empty,
                    Convert.ToInt32(reader["empdocid"]),
                    Convert.ToInt32(reader["documenttypeid"]),
                    reader["documenttype"]?.ToString() ?? string.Empty,
                    reader["docname"]?.ToString() ?? string.Empty,
                    Path.GetFileName((reader["uploadpath"]?.ToString() ?? string.Empty).Replace('\\', '/')),
                    reader["uploadpath"]?.ToString() ?? string.Empty,
                    reader["externallink"]?.ToString() ?? string.Empty
                ));
            }

            return rows;
        }

        private async Task<IReadOnlyList<EmployeeDocumentAttachmentV3Dto>> LoadAttachmentsByEmployee(int employeeId, CancellationToken cancellationToken)
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync(cancellationToken);

            const string sql = """
                SELECT
                    a.id,
                    ISNULL(a.attchmentno, '') AS attachmentno,
                    ed.Id AS empdocid,
                    ISNULL(ed.documenttypeid, 0) AS documenttypeid,
                    ISNULL(dt.stxt, '') AS documenttype,
                    ISNULL(a.docname, '') AS docname,
                    ISNULL(a.uploadpath, '') AS uploadpath,
                    ISNULL(a.externallink, '') AS externallink
                FROM SMAttachments a
                INNER JOIN EREmpDocument ed ON ed.Id = a.refdocid
                LEFT JOIN HRDocType dt ON dt.Id = ed.documenttypeid
                WHERE ed.employeeid = @employeeId
                  AND ISNULL(ed.utblock, 0) = 0
                  AND a.doctypeid = @referenceType
                  AND a.clientid = @clientId
                  AND ISNULL(a.utblock, 0) = 0
                ORDER BY a.id DESC
            """;

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.Add(new SqlParameter("@employeeId", employeeId));
            cmd.Parameters.Add(new SqlParameter("@referenceType", DocumentTypeReferenceType));
            cmd.Parameters.Add(new SqlParameter("@clientId", GetClientId()));

            var rows = new List<EmployeeDocumentAttachmentV3Dto>();
            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                rows.Add(new EmployeeDocumentAttachmentV3Dto(
                    Convert.ToInt32(reader["id"]),
                    reader["attachmentno"]?.ToString() ?? string.Empty,
                    Convert.ToInt32(reader["empdocid"]),
                    Convert.ToInt32(reader["documenttypeid"]),
                    reader["documenttype"]?.ToString() ?? string.Empty,
                    reader["docname"]?.ToString() ?? string.Empty,
                    Path.GetFileName((reader["uploadpath"]?.ToString() ?? string.Empty).Replace('\\', '/')),
                    reader["uploadpath"]?.ToString() ?? string.Empty,
                    reader["externallink"]?.ToString() ?? string.Empty
                ));
            }

            return rows;
        }

        private void AddAttachmentParameters(
            System.Data.Common.DbCommand cmd,
            int id,
            string? attachmentNo,
            int employeeDocumentId,
            SaveEmployeeDocumentAttachmentV3Request request,
            string? uploadPath)
        {
            cmd.Parameters.Add(new SqlParameter("@id", id));
            cmd.Parameters.Add(new SqlParameter("@attachmentNo", (object?)attachmentNo ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@refdocid", employeeDocumentId));
            cmd.Parameters.Add(new SqlParameter("@doctypeid", DocumentTypeReferenceType));
            cmd.Parameters.Add(new SqlParameter("@docname", request.DocumentName?.Trim() ?? string.Empty));
            cmd.Parameters.Add(new SqlParameter("@uploadpath", (object?)uploadPath ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@lovreftypeid", request.DocumentTypeId));
            cmd.Parameters.Add(new SqlParameter("@externallink", (object?)NormalizeNullable(request.ExternalLink) ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@typeid", InternalStorageType));
            cmd.Parameters.Add(new SqlParameter("@entityid", GetEntityId()));
            cmd.Parameters.Add(new SqlParameter("@clientid", GetClientId()));
        }

        private static AttachmentFilePayload? DecodeFileIfProvided(SaveEmployeeDocumentAttachmentV3Request request)
        {
            if (string.IsNullOrWhiteSpace(request.FileContentBase64) || string.IsNullOrWhiteSpace(request.FileName))
                return null;

            var contentType = ResolveDataUrlContentType(request.FileContentBase64);
            var commaIndex = request.FileContentBase64.IndexOf(',');
            var rawBase64 = commaIndex >= 0 ? request.FileContentBase64[(commaIndex + 1)..] : request.FileContentBase64;
            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(rawBase64);
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException("Attachment file content is invalid.", ex);
            }

            var extension = Path.GetExtension(request.FileName);
            var safeBaseName = Path.GetFileNameWithoutExtension(request.FileName)
                .Replace(" ", "_")
                .Replace("/", "_")
                .Replace("\\", "_");
            var fileName = $"{safeBaseName}_{DateTime.UtcNow:yyyyMMddHHmmssfff}{extension}";
            return new AttachmentFilePayload(fileName, contentType ?? ResolveContentType(fileName), bytes);
        }

        private async Task<string?> GetAttachmentUploadPath(int attachmentId, CancellationToken cancellationToken)
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync(cancellationToken);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT TOP 1 uploadpath
                FROM SMAttachments
                WHERE id = @id
                  AND doctypeid = @doctypeid
                  AND clientid = @clientid
                  AND ISNULL(utblock, 0) = 0
            """;
            cmd.Parameters.Add(new SqlParameter("@id", attachmentId));
            cmd.Parameters.Add(new SqlParameter("@doctypeid", DocumentTypeReferenceType));
            cmd.Parameters.Add(new SqlParameter("@clientid", GetClientId()));
            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            return result is null or DBNull ? null : result.ToString();
        }

        private async Task<(string UploadPath, string DocumentName, string? FileName, string? ContentType, byte[]? FileData)?> GetAttachmentFileInfo(int attachmentId, CancellationToken cancellationToken)
        {
            await EnsureAttachmentFileTable(cancellationToken);

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync(cancellationToken);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT TOP 1
                    ISNULL(a.uploadpath, '') AS uploadpath,
                    ISNULL(a.docname, '') AS docname,
                    f.filename,
                    f.contenttype,
                    f.filedata
                FROM SMAttachments a
                LEFT JOIN HREmployeeDocumentFile f
                    ON f.attachmentid = a.id
                   AND f.clientid = a.clientid
                WHERE a.id = @id
                  AND a.clientid = @clientid
                  AND ISNULL(a.utblock, 0) = 0
            """;
            cmd.Parameters.Add(new SqlParameter("@id", attachmentId));
            cmd.Parameters.Add(new SqlParameter("@clientid", GetClientId()));

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken)) return null;
            return (
                reader["uploadpath"]?.ToString() ?? string.Empty,
                reader["docname"]?.ToString() ?? string.Empty,
                reader["filename"] is DBNull ? null : reader["filename"]?.ToString(),
                reader["contenttype"] is DBNull ? null : reader["contenttype"]?.ToString(),
                reader["filedata"] is DBNull ? null : (byte[])reader["filedata"]
            );
        }

        private async Task UpsertAttachmentFile(int attachmentId, AttachmentFilePayload filePayload, CancellationToken cancellationToken)
        {
            await EnsureAttachmentFileTable(cancellationToken);

            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync(cancellationToken);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                MERGE HREmployeeDocumentFile AS target
                USING (SELECT @attachmentid AS attachmentid, @clientid AS clientid) AS source
                   ON target.attachmentid = source.attachmentid
                  AND target.clientid = source.clientid
                WHEN MATCHED THEN
                    UPDATE SET filename = @filename,
                               contenttype = @contenttype,
                               filedata = @filedata,
                               updatedat = SYSUTCDATETIME()
                WHEN NOT MATCHED THEN
                    INSERT (attachmentid, clientid, filename, contenttype, filedata, createdat, updatedat)
                    VALUES (@attachmentid, @clientid, @filename, @contenttype, @filedata, SYSUTCDATETIME(), SYSUTCDATETIME());
            """;
            cmd.Parameters.Add(new SqlParameter("@attachmentid", attachmentId));
            cmd.Parameters.Add(new SqlParameter("@clientid", GetClientId()));
            cmd.Parameters.Add(new SqlParameter("@filename", filePayload.FileName));
            cmd.Parameters.Add(new SqlParameter("@contenttype", filePayload.ContentType));
            cmd.Parameters.Add(new SqlParameter("@filedata", SqlDbType.VarBinary, -1) { Value = filePayload.Bytes });
            _context.PrepareAuditCommand(cmd);
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }

        private async Task EnsureAttachmentFileTable(CancellationToken cancellationToken)
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync(cancellationToken);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                IF OBJECT_ID(N'dbo.HREmployeeDocumentFile', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.HREmployeeDocumentFile
                    (
                        attachmentid int NOT NULL,
                        clientid int NOT NULL,
                        filename nvarchar(260) NOT NULL,
                        contenttype nvarchar(150) NOT NULL,
                        filedata varbinary(max) NOT NULL,
                        createdat datetime2 NOT NULL,
                        updatedat datetime2 NOT NULL,
                        CONSTRAINT PK_HREmployeeDocumentFile PRIMARY KEY (attachmentid, clientid)
                    );
                END
            """;
            _context.PrepareAuditCommand(cmd);
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }

        private static string? ResolveDataUrlContentType(string dataUrl)
        {
            if (!dataUrl.StartsWith("data:", StringComparison.OrdinalIgnoreCase)) return null;
            var semicolonIndex = dataUrl.IndexOf(';');
            return semicolonIndex > 5 ? dataUrl[5..semicolonIndex] : null;
        }

        private static string ResolveContentType(string fileName)
        {
            var provider = new FileExtensionContentTypeProvider();
            return provider.TryGetContentType(fileName, out var contentType)
                ? contentType
                : MediaTypeNames.Application.Octet;
        }

        private async Task<int> GetNextInt(string tableName, string columnName, CancellationToken cancellationToken)
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync(cancellationToken);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT ISNULL(MAX([{columnName}]), 0) + 1 FROM [{tableName}]";
            var scalar = await cmd.ExecuteScalarAsync(cancellationToken);
            return scalar is null or DBNull ? 1 : Convert.ToInt32(scalar);
        }

        private async Task<string> GenerateAttachmentNo(CancellationToken cancellationToken)
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
                await conn.OpenAsync(cancellationToken);

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT ISNULL(MAX(CAST(RIGHT(ISNULL(attchmentno, 'ATT-000000'), 6) AS int)), 0) + 1
                FROM SMAttachments
                WHERE clientid = @clientid
            """;
            cmd.Parameters.Add(new SqlParameter("@clientid", GetClientId()));
            var scalar = await cmd.ExecuteScalarAsync(cancellationToken);
            return $"ATT-{(scalar is null or DBNull ? 1 : Convert.ToInt32(scalar)):D6}";
        }

        private int GetClientId()
        {
            if (int.TryParse(Request.Headers["X-Client-Id"].FirstOrDefault(), out var clientId))
                return clientId;

            return int.TryParse(User.FindFirst("client_id")?.Value ?? User.FindFirst("ClientId")?.Value, out var claimClientId)
                ? claimClientId
                : 0;
        }

        private int GetEntityId()
        {
            if (int.TryParse(Request.Headers["X-Entity-Id"].FirstOrDefault(), out var entityId))
                return entityId;

            return int.TryParse(User.FindFirst("entity_id")?.Value ?? User.FindFirst("EntityId")?.Value, out var claimEntityId)
                ? claimEntityId
                : 0;
        }

        private static string? NormalizeNullable(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private sealed record AttachmentFilePayload(string FileName, string ContentType, byte[] Bytes);
    }
}
