using HumanResource.Service.DTOs;
using HumanResource.Service.Entities;
using HumanResource.Service.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/HumanResource/EmployeeInformationRecords/v3")]
    [Route("api/EmployeeInformationRecords/v3")]
    [Route("EmployeeInformationRecords/v3")]
    public sealed class EmployeeInformationRecordsV3Controller : ControllerBase
    {
        private const int MaxAttachmentBytes = 10 * 1024 * 1024;
        private const string EmployeeProfilePermission = "Employee Profile";
        private const string CategoryLovType = "HRRCA";
        private static readonly HashSet<string> DisciplinaryCategories = new(StringComparer.OrdinalIgnoreCase)
        {
            "INQUIRY", "PUNISHMENT", "COURT_CASE"
        };
        private static readonly HashSet<string> DevelopmentCategories = new(StringComparer.OrdinalIgnoreCase)
        {
            "AWARD", "TRAINING"
        };
        private static readonly Dictionary<string, string> CategoryLovCodes = new(StringComparer.OrdinalIgnoreCase)
        {
            ["INQ"] = "INQUIRY", ["PUN"] = "PUNISHMENT", ["CRT"] = "COURT_CASE",
            ["AWD"] = "AWARD", ["TRN"] = "TRAINING"
        };
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".pdf", ".png", ".jpg", ".jpeg", ".doc", ".docx"
        };
        private static readonly Dictionary<string, HashSet<string>> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase)
        {
            ["INQUIRY"] = new(StringComparer.OrdinalIgnoreCase) { "Initiated", "In Progress", "Completed", "Closed" },
            ["PUNISHMENT"] = new(StringComparer.OrdinalIgnoreCase) { "Issued", "Active", "Completed", "Withdrawn" },
            ["COURT_CASE"] = new(StringComparer.OrdinalIgnoreCase) { "Pending", "In Progress", "Decided", "Closed" },
            ["TRAINING"] = new(StringComparer.OrdinalIgnoreCase) { "Planned", "In Progress", "Completed", "Cancelled" }
        };
        private static readonly HashSet<string> AllowedTrainingModes = new(StringComparer.OrdinalIgnoreCase)
        {
            "In-person", "Online", "Hybrid"
        };

        private readonly HumanResourceDbContext _context;
        private readonly FileExtensionContentTypeProvider _contentTypeProvider = new();

        public EmployeeInformationRecordsV3Controller(HumanResourceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<EmployeeInformationRecordV3Dto>>> GetAll(
            [FromQuery] int employeeId,
            [FromQuery] string group,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 100,
            CancellationToken cancellationToken = default)
        {
            await EnsureStorageAsync(cancellationToken);
            var normalizedGroup = NormalizeGroup(group);
            if (normalizedGroup == null) return BadRequest("Group must be DISCIPLINARY or DEVELOPMENT.");
            if (normalizedGroup == "DISCIPLINARY" && !CanAccessDisciplinaryRecords()) return Forbid();
            if (!await IsEmployeeInScope(employeeId, cancellationToken)) return NotFound("Employee was not found in the current scope.");

            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 1000);
            var categoryMap = await GetCategoryMap(cancellationToken);
            var categoryIds = categoryMap
                .Where(x => normalizedGroup == "DISCIPLINARY" ? IsDisciplinary(x.Value) : DevelopmentCategories.Contains(x.Value))
                .Select(x => x.Key)
                .ToList();
            var query = ScopedRecords().AsNoTracking()
                .Where(x => x.EmployeeId == employeeId && categoryIds.Contains(x.CategoryId));
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .OrderByDescending(x => x.RecordDate)
                .ThenByDescending(x => x.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return Ok(new PaginationResponse<EmployeeInformationRecordV3Dto>
            {
                Items = items.Select(x => Map(x, categoryMap.GetValueOrDefault(x.CategoryId, string.Empty))).ToList(),
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            });
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<EmployeeInformationRecordV3Dto>> GetById(int id, CancellationToken cancellationToken)
        {
            await EnsureStorageAsync(cancellationToken);
            var entity = await ScopedRecords().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (entity == null) return NotFound();
            var category = await GetCategoryCode(entity.CategoryId, cancellationToken);
            if (category == null) return BadRequest("The record category LOV is missing or invalid.");
            if (IsDisciplinary(category) && !CanAccessDisciplinaryRecords()) return Forbid();
            return Ok(Map(entity, category));
        }

        [HttpPost]
        public async Task<ActionResult<EmployeeInformationRecordV3Dto>> Create(
            [FromBody] UpsertEmployeeInformationRecordV3Request request,
            CancellationToken cancellationToken)
        {
            await EnsureStorageAsync(cancellationToken);
            var validation = await Validate(request, cancellationToken);
            if (validation != null) return BadRequest(validation);

            var category = await GetCategoryCode(request.CategoryId, cancellationToken);
            if (category == null) return BadRequest("A valid record category is required.");
            if (IsDisciplinary(category) && !CanAccessDisciplinaryRecords()) return Forbid();
            AttachmentPayload? attachment;
            try
            {
                attachment = DecodeAttachment(request);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            var now = DateTime.UtcNow;
            var entity = new EmployeeInformationRecord
            {
                EmployeeId = request.EmployeeId,
                ClientId = GetClientId(),
                EntityId = GetEntityId(),
                CategoryId = request.CategoryId,
                Category = Trim(category, 30) ?? string.Empty,
                Title = Trim(request.Title, 250)!,
                ReferenceNumber = Trim(request.ReferenceNumber, 100),
                RecordDate = request.RecordDate!.Value.Date,
                EndDate = request.EndDate?.Date,
                Status = category == "AWARD" ? null : Trim(request.Status, 50),
                Authority = Trim(request.Authority, 200),
                SubCategory = Trim(request.SubCategory, 100),
                Mode = category == "TRAINING" ? Trim(request.Mode, 50) : null,
                Details = Trim(request.Details, 2000),
                Outcome = IsDisciplinary(category) ? Trim(request.Outcome, 2000) : null,
                Remarks = Trim(request.Remarks, 2000),
                FileName = attachment?.FileName,
                FileContentType = attachment?.ContentType,
                FileData = attachment?.Bytes,
                CreatedAt = now,
                CreatedBy = GetUserName(),
                IsDeleted = false
            };

            _context.EmployeeInformationRecords.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = entity.Id }, Map(entity, category));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpsertEmployeeInformationRecordV3Request request,
            CancellationToken cancellationToken)
        {
            await EnsureStorageAsync(cancellationToken);
            var entity = await ScopedRecords().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (entity == null) return NotFound();
            var existingCategory = await GetCategoryCode(entity.CategoryId, cancellationToken);
            if (existingCategory == null) return BadRequest("The record category LOV is missing or invalid.");
            if (IsDisciplinary(existingCategory) && !CanAccessDisciplinaryRecords()) return Forbid();
            if (request.EmployeeId != entity.EmployeeId) return BadRequest("Employee cannot be changed for an existing record.");

            var validation = await Validate(request, cancellationToken);
            if (validation != null) return BadRequest(validation);
            var category = await GetCategoryCode(request.CategoryId, cancellationToken);
            if (category == null) return BadRequest("A valid record category is required.");
            if (IsDisciplinary(category) && !CanAccessDisciplinaryRecords()) return Forbid();

            AttachmentPayload? attachment;
            try
            {
                attachment = DecodeAttachment(request);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            entity.CategoryId = request.CategoryId;
            entity.Category = Trim(category, 30) ?? string.Empty;
            entity.Title = Trim(request.Title, 250)!;
            entity.ReferenceNumber = Trim(request.ReferenceNumber, 100);
            entity.RecordDate = request.RecordDate!.Value.Date;
            entity.EndDate = request.EndDate?.Date;
            entity.Status = category == "AWARD" ? null : Trim(request.Status, 50);
            entity.Authority = Trim(request.Authority, 200);
            entity.SubCategory = Trim(request.SubCategory, 100);
            entity.Mode = category == "TRAINING" ? Trim(request.Mode, 50) : null;
            entity.Details = Trim(request.Details, 2000);
            entity.Outcome = IsDisciplinary(category) ? Trim(request.Outcome, 2000) : null;
            entity.Remarks = Trim(request.Remarks, 2000);
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = GetUserName();

            if (request.RemoveAttachment)
            {
                entity.FileName = null;
                entity.FileContentType = null;
                entity.FileData = null;
            }
            if (attachment != null)
            {
                entity.FileName = attachment.FileName;
                entity.FileContentType = attachment.ContentType;
                entity.FileData = attachment.Bytes;
            }

            await _context.SaveChangesAsync(cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await EnsureStorageAsync(cancellationToken);
            var entity = await ScopedRecords().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (entity == null) return NotFound();
            var category = await GetCategoryCode(entity.CategoryId, cancellationToken);
            if (category == null) return BadRequest("The record category LOV is missing or invalid.");
            if (IsDisciplinary(category) && !CanAccessDisciplinaryRecords()) return Forbid();

            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = GetUserName();
            await _context.SaveChangesAsync(cancellationToken);
            return NoContent();
        }

        [HttpGet("{id:int}/attachment")]
        public async Task<IActionResult> DownloadAttachment(int id, CancellationToken cancellationToken)
        {
            await EnsureStorageAsync(cancellationToken);
            var entity = await ScopedRecords().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (entity == null) return NotFound();
            var category = await GetCategoryCode(entity.CategoryId, cancellationToken);
            if (category == null) return BadRequest("The record category LOV is missing or invalid.");
            if (IsDisciplinary(category) && !CanAccessDisciplinaryRecords()) return Forbid();
            if (entity.FileData == null || entity.FileData.Length == 0 || string.IsNullOrWhiteSpace(entity.FileName))
                return NotFound("No attachment is stored for this record.");

            return File(entity.FileData, entity.FileContentType ?? "application/octet-stream", entity.FileName);
        }

        private IQueryable<EmployeeInformationRecord> ScopedRecords()
        {
            var clientId = GetClientId();
            var entityId = GetEntityId();
            var query = _context.EmployeeInformationRecords
                .Where(x => x.ClientId == clientId && !x.IsDeleted);
            if (entityId.HasValue) query = query.Where(x => x.EntityId == entityId.Value);
            return query;
        }

        private async Task<string?> Validate(UpsertEmployeeInformationRecordV3Request request, CancellationToken cancellationToken)
        {
            var category = await GetCategoryCode(request.CategoryId, cancellationToken);
            if (category == null) return "A valid record category is required.";
            if (string.IsNullOrWhiteSpace(request.Title)) return "Title/Name is required.";
            if (!request.RecordDate.HasValue) return "The relevant date is required.";
            if (category != "AWARD" && string.IsNullOrWhiteSpace(request.Status)) return "Status is required.";
            if (category != "AWARD" &&
                (!AllowedStatuses.TryGetValue(category, out var statuses) || !statuses.Contains(request.Status!.Trim())))
                return "The selected status is not valid for this record category.";
            if (category == "TRAINING" && request.EndDate.HasValue && request.RecordDate.Value.Date > request.EndDate.Value.Date)
                return "Training Start Date cannot be greater than End Date.";
            if (category == "TRAINING" && !string.IsNullOrWhiteSpace(request.Mode) && !AllowedTrainingModes.Contains(request.Mode.Trim()))
                return "The selected training mode is not valid.";
            if (!await IsEmployeeInScope(request.EmployeeId, cancellationToken))
                return "Employee does not belong to the current client/entity.";
            return null;
        }

        private Task<bool> IsEmployeeInScope(int employeeId, CancellationToken cancellationToken)
        {
            var clientId = GetClientId();
            var entityId = GetEntityId();
            return _context.EmployeeProfiles.AnyAsync(x =>
                x.Id == employeeId &&
                x.ClientId == clientId &&
                (!entityId.HasValue || x.EntityId == entityId.Value) &&
                !(x.UtBlock ?? false), cancellationToken);
        }

        private bool CanAccessDisciplinaryRecords() => User.Identity?.IsAuthenticated == true;

        private async Task<Dictionary<int, string>> GetCategoryMap(CancellationToken cancellationToken)
        {
            var lovs = await _context.Lovs.AsNoTracking()
                .Where(x => !x.UtBlock && x.Vtype == CategoryLovType && x.Code != null)
                .Select(x => new { x.Id, x.Code })
                .ToListAsync(cancellationToken);
            return lovs.Select(x => new { x.Id, Category = NormalizeCategory(x.Code) })
                .Where(x => x.Category != null)
                .ToDictionary(x => x.Id, x => x.Category!);
        }

        private static string? NormalizeCategory(string? value) =>
            value != null && CategoryLovCodes.TryGetValue(value.Trim(), out var category) ? category : null;

        private async Task<string?> GetCategoryCode(int categoryId, CancellationToken cancellationToken)
        {
            if (categoryId <= 0) return null;
            var categoryMap = await GetCategoryMap(cancellationToken);
            return categoryMap.GetValueOrDefault(categoryId);
        }

        private static string? NormalizeGroup(string? value)
        {
            var normalized = value?.Trim().ToUpperInvariant();
            return normalized is "DISCIPLINARY" or "DEVELOPMENT" ? normalized : null;
        }

        private static bool IsDisciplinary(string category) => DisciplinaryCategories.Contains(category);
        private static string GroupFor(string category) => IsDisciplinary(category) ? "DISCIPLINARY" : "DEVELOPMENT";

        private EmployeeInformationRecordV3Dto Map(EmployeeInformationRecord entity, string category) => new(
            entity.Id,
            entity.EmployeeId,
            GroupFor(category),
            entity.CategoryId,
            category,
            entity.Title,
            entity.ReferenceNumber,
            entity.RecordDate,
            entity.EndDate,
            entity.Status,
            entity.Authority,
            entity.SubCategory,
            entity.Mode,
            entity.Details,
            entity.Outcome,
            entity.Remarks,
            entity.FileData != null && entity.FileData.Length > 0,
            entity.FileName,
            entity.CreatedAt,
            entity.CreatedBy,
            entity.UpdatedAt,
            entity.UpdatedBy);

        private AttachmentPayload? DecodeAttachment(UpsertEmployeeInformationRecordV3Request request)
        {
            if (string.IsNullOrWhiteSpace(request.FileContentBase64)) return null;
            if (string.IsNullOrWhiteSpace(request.FileName))
                throw new InvalidOperationException("Attachment file name is required.");

            var safeFileName = Path.GetFileName(request.FileName.Trim());
            var extension = Path.GetExtension(safeFileName);
            if (!AllowedExtensions.Contains(extension))
                throw new InvalidOperationException("Only PDF, image, Word, and Word Open XML attachments are allowed.");

            var commaIndex = request.FileContentBase64.IndexOf(',');
            var rawBase64 = commaIndex >= 0 ? request.FileContentBase64[(commaIndex + 1)..] : request.FileContentBase64;
            if (rawBase64.Length > (MaxAttachmentBytes * 4 / 3) + 16)
                throw new InvalidOperationException("Attachment size cannot exceed 10 MB.");

            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(rawBase64);
            }
            catch (FormatException ex)
            {
                throw new InvalidOperationException("Attachment file content is invalid.", ex);
            }
            if (bytes.Length > MaxAttachmentBytes)
                throw new InvalidOperationException("Attachment size cannot exceed 10 MB.");

            _contentTypeProvider.TryGetContentType(safeFileName, out var contentType);
            return new AttachmentPayload(safeFileName, contentType ?? "application/octet-stream", bytes);
        }

        private async Task EnsureStorageAsync(CancellationToken cancellationToken)
        {
            await _context.Database.ExecuteSqlRawAsync("""
                IF OBJECT_ID(N'dbo.HREmployeeInformationRecord', N'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.HREmployeeInformationRecord
                    (
                        id int IDENTITY(1,1) NOT NULL CONSTRAINT PK_HREmployeeInformationRecord PRIMARY KEY,
                        employeeid int NOT NULL,
                        clientid int NOT NULL,
                        entityid int NULL,
                        categoryid int NOT NULL,
                        category nvarchar(30) NOT NULL CONSTRAINT DF_HREmployeeInformationRecord_category DEFAULT N'',
                        title nvarchar(250) NOT NULL,
                        referencenumber nvarchar(100) NULL,
                        recorddate date NOT NULL,
                        enddate date NULL,
                        status nvarchar(50) NULL,
                        authority nvarchar(200) NULL,
                        subcategory nvarchar(100) NULL,
                        mode nvarchar(50) NULL,
                        details nvarchar(2000) NULL,
                        outcome nvarchar(2000) NULL,
                        remarks nvarchar(2000) NULL,
                        filename nvarchar(260) NULL,
                        filecontenttype nvarchar(150) NULL,
                        filedata varbinary(max) NULL,
                        createdat datetime2 NOT NULL CONSTRAINT DF_HREmployeeInformationRecord_createdat DEFAULT SYSUTCDATETIME(),
                        createdby nvarchar(100) NULL,
                        updatedat datetime2 NULL,
                        updatedby nvarchar(100) NULL,
                        isdeleted bit NOT NULL CONSTRAINT DF_HREmployeeInformationRecord_isdeleted DEFAULT 0
                    );
                    CREATE INDEX IX_HREmployeeInformationRecord_scope
                        ON dbo.HREmployeeInformationRecord(clientid, entityid, employeeid, categoryid, isdeleted);
                END

                -- Upgrade databases created by the earlier text-category implementation.
                IF COL_LENGTH(N'dbo.HREmployeeInformationRecord', N'categoryid') IS NULL
                BEGIN
                    ALTER TABLE dbo.HREmployeeInformationRecord ADD categoryid int NULL;
                    EXEC(N'
                        UPDATE r SET categoryid = l.id
                        FROM dbo.HREmployeeInformationRecord r
                        INNER JOIN dbo.lov l ON l.vtype = N''HRRCA''
                            AND l.code = CASE r.category
                                WHEN N''INQUIRY'' THEN N''INQ'' WHEN N''PUNISHMENT'' THEN N''PUN''
                                WHEN N''COURT_CASE'' THEN N''CRT'' WHEN N''AWARD'' THEN N''AWD''
                                WHEN N''TRAINING'' THEN N''TRN'' END
                            AND ISNULL(l.utblock, 0) = 0;');
                    IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.HREmployeeInformationRecord') AND name = N'IX_HREmployeeInformationRecord_scope')
                        DROP INDEX IX_HREmployeeInformationRecord_scope ON dbo.HREmployeeInformationRecord;
                    EXEC(N'
                        ALTER TABLE dbo.HREmployeeInformationRecord DROP COLUMN category;
                        ALTER TABLE dbo.HREmployeeInformationRecord ALTER COLUMN categoryid int NOT NULL;
                        CREATE INDEX IX_HREmployeeInformationRecord_scope_categoryid
                            ON dbo.HREmployeeInformationRecord(clientid, entityid, employeeid, categoryid, isdeleted);');
                END

                -- Legacy category code column; kept populated alongside categoryid.
                IF COL_LENGTH(N'dbo.HREmployeeInformationRecord', N'category') IS NULL
                    ALTER TABLE dbo.HREmployeeInformationRecord ADD category nvarchar(30) NOT NULL
                        CONSTRAINT DF_HREmployeeInformationRecord_category DEFAULT N'';
                """, cancellationToken);
        }

        private int GetClientId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type is "ClientId" or "clientId" or "client_id");
            if (claim != null && int.TryParse(claim.Value, out var value)) return value;
            if (Request.Headers.TryGetValue("X-Client-Id", out var header) && int.TryParse(header, out value)) return value;
            throw new InvalidOperationException("ClientId not found in token or headers.");
        }

        private int? GetEntityId()
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type is "EntityId" or "entityId" or "entity_id");
            if (claim != null && int.TryParse(claim.Value, out var value)) return value;
            if (Request.Headers.TryGetValue("X-Entity-Id", out var header) && int.TryParse(header, out value)) return value;
            return null;
        }

        private string? GetUserName() =>
            User.FindFirst("userName")?.Value ??
            User.FindFirst(ClaimTypes.Name)?.Value ??
            User.Identity?.Name;

        private static string? Trim(string? value, int maxLength)
        {
            var trimmed = value?.Trim();
            if (string.IsNullOrEmpty(trimmed)) return null;
            return trimmed.Length <= maxLength ? trimmed : trimmed[..maxLength];
        }

        private sealed record AttachmentPayload(string FileName, string ContentType, byte[] Bytes);
    }
}
