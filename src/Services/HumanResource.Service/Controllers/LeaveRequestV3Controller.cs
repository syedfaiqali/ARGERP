using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Mime;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Route("api/LeaveRequest/v3")]
    [Route("api/HumanResource/LeaveRequest/v3")]
    [Authorize]
    public sealed class LeaveRequestV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly HumanResourceDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public LeaveRequestV3Controller(
            IMediator mediator,
            HumanResourceDbContext context,
            IWebHostEnvironment environment)
        {
            _mediator = mediator;
            _context = context;
            _environment = environment;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<LeaveRequestV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? statusId = null)
        {
            var result = await _mediator.Send(new GetLeaveRequestsV3Query(pageNumber, pageSize, GetClientId(), GetEntityId(), searchTerm, statusId));
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LeaveRequestV3Dto>> GetById(int id)
        {
            var result = await _mediator.Send(new GetLeaveRequestByIdV3Query(id, GetClientId(), GetEntityId()));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<LeaveRequestV3Dto>> Create([FromBody] CreateLeaveRequestV3Request request)
        {
            try
            {
                var result = await _mediator.Send(new CreateLeaveRequestV3Command(request, GetClientId(), GetEntityId()));
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateLeaveRequestV3Request request)
        {
            try
            {
                var updated = await _mediator.Send(new UpdateLeaveRequestV3Command(request, GetClientId(), GetEntityId()));
                if (!updated) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _mediator.Send(new DeleteLeaveRequestV3Command(id, GetClientId(), GetEntityId()));
                if (!deleted) return NotFound();
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("attachments/{attachmentId:int}/view")]
        public async Task<IActionResult> ViewAttachment(int attachmentId)
        {
            return await OpenAttachment(attachmentId, inline: true);
        }

        [HttpGet("attachments/{attachmentId:int}/download")]
        public async Task<IActionResult> DownloadAttachment(int attachmentId)
        {
            return await OpenAttachment(attachmentId, inline: false);
        }

        private async Task<IActionResult> OpenAttachment(int attachmentId, bool inline)
        {
            var attachment = await GetAttachmentAsync(attachmentId);
            if (attachment == null)
            {
                return NotFound("Attachment not found.");
            }

            if (!string.IsNullOrWhiteSpace(attachment.ExternalLink))
            {
                return Redirect(attachment.ExternalLink);
            }

            var path = NormalizePath(!string.IsNullOrWhiteSpace(attachment.UploadPath)
                ? attachment.UploadPath
                : attachment.BrowsePath);
            if (string.IsNullOrWhiteSpace(path))
            {
                return NotFound("Attachment file path is empty.");
            }

            if (Uri.TryCreate(path, UriKind.Absolute, out var absoluteUri) &&
                (absoluteUri.Scheme == Uri.UriSchemeHttp || absoluteUri.Scheme == Uri.UriSchemeHttps))
            {
                return Redirect(path);
            }

            var filePath = ResolveLocalFilePath(path);
            if (filePath == null || !System.IO.File.Exists(filePath))
            {
                return NotFound($"Attachment file was not found on server. Path: {path}");
            }

            var fileName = Path.GetFileName(filePath);
            var downloadFileName = GetDownloadFileName(attachment.DocumentName, fileName);
            var contentType = GetContentType(fileName);
            Response.Headers.ContentDisposition = inline
                ? new ContentDisposition { Inline = true, FileName = downloadFileName }.ToString()
                : new ContentDisposition { Inline = false, FileName = downloadFileName }.ToString();

            return PhysicalFile(filePath, contentType, inline ? null : downloadFileName);
        }

        private async Task<AttachmentFileInfo?> GetAttachmentAsync(int attachmentId)
        {
            var conn = _context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
            }

            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT TOP 1
                    ISNULL(docname, '') AS docname,
                    ISNULL(browsepath, '') AS browsepath,
                    ISNULL(uploadpath, '') AS uploadpath,
                    ISNULL(externallink, '') AS externallink
                FROM SMAttachments
                WHERE (id = @id OR refdocid = @id)
                  AND clientid = @clientId
                  AND (
                        @entityId IS NULL
                        OR entityid = @entityId
                        OR entityid IS NULL
                        OR entityid = 0
                      )
                  AND ISNULL(utblock, 0) = 0
                ORDER BY CASE WHEN id = @id THEN 0 ELSE 1 END, id
            """;
            cmd.Parameters.Add(new SqlParameter("@id", attachmentId));
            cmd.Parameters.Add(new SqlParameter("@clientId", GetClientId()));
            cmd.Parameters.Add(new SqlParameter("@entityId", (object?)GetEntityId() ?? DBNull.Value));

            await using var reader = await cmd.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            return new AttachmentFileInfo(
                reader["docname"]?.ToString() ?? string.Empty,
                reader["browsepath"]?.ToString() ?? string.Empty,
                reader["uploadpath"]?.ToString() ?? string.Empty,
                reader["externallink"]?.ToString() ?? string.Empty
            );
        }

        private string? ResolveLocalFilePath(string path)
        {
            var normalized = NormalizePath(path);
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return null;
            }

            if (Path.IsPathRooted(normalized))
            {
                return normalized;
            }

            var relative = normalized.TrimStart('/', '\\');
            var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var candidateRoots = new[]
            {
                webRoot,
                _environment.ContentRootPath,
                Path.GetDirectoryName(_environment.ContentRootPath) ?? _environment.ContentRootPath,
                Path.GetPathRoot(_environment.ContentRootPath) ?? _environment.ContentRootPath
            };

            foreach (var root in candidateRoots.Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var candidate = Path.GetFullPath(Path.Combine(root, relative));
                if (System.IO.File.Exists(candidate))
                {
                    return candidate;
                }
            }

            return Path.GetFullPath(Path.Combine(webRoot, relative));
        }

        private static string NormalizePath(string? value)
        {
            return (value ?? string.Empty).Trim().Replace('\\', '/');
        }

        private static string GetDownloadFileName(string? documentName, string fallbackFileName)
        {
            var fallback = Path.GetFileName(fallbackFileName);
            var preferred = Path.GetFileName(NormalizePath(documentName));
            var fileName = string.IsNullOrWhiteSpace(preferred) ? fallback : preferred;
            var fallbackExtension = Path.GetExtension(fallback);

            if (string.IsNullOrWhiteSpace(Path.GetExtension(fileName)) && !string.IsNullOrWhiteSpace(fallbackExtension))
            {
                fileName += fallbackExtension;
            }

            foreach (var invalid in Path.GetInvalidFileNameChars())
            {
                fileName = fileName.Replace(invalid, '_');
            }

            return string.IsNullOrWhiteSpace(fileName) ? "attachment" : fileName;
        }

        private static string GetContentType(string fileName)
        {
            return Path.GetExtension(fileName).ToLowerInvariant() switch
            {
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };
        }

        private sealed record AttachmentFileInfo(
            string DocumentName,
            string BrowsePath,
            string UploadPath,
            string ExternalLink
        );

        private int GetClientId()
        {
            var clientIdClaim = User.Claims.FirstOrDefault(c => c.Type == "ClientId");
            if (clientIdClaim != null && int.TryParse(clientIdClaim.Value, out var claimClientId)) return claimClientId;
            if (Request.Headers.TryGetValue("X-Client-Id", out var headerClientId) && int.TryParse(headerClientId, out var parsedHeaderClientId)) return parsedHeaderClientId;
            throw new InvalidOperationException("ClientId not found in token or headers");
        }

        private int? GetEntityId()
        {
            var entityIdClaim = User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if (entityIdClaim != null && int.TryParse(entityIdClaim.Value, out var claimEntityId)) return claimEntityId;
            if (Request.Headers.TryGetValue("X-Entity-Id", out var headerEntityId) && int.TryParse(headerEntityId, out var parsedHeaderEntityId)) return parsedHeaderEntityId;
            return null;
        }
    }
}
