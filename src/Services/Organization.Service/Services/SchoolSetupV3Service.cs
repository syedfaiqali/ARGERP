using System.Data;
using System.Data.Common;
using System.Globalization;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Organization.Service.DTOs;
using Organization.Service.Entities;
using Organization.Service.Infrastructure.Data;

namespace Organization.Service.Services
{
    public interface ISchoolSetupV3Service
    {
        Task<SchoolSetupTabResponseDto> GetTabAsync(string tabKey, int pageNumber, int pageSize, int clientId, int? entityId, CancellationToken cancellationToken);
        Task<SchoolSetupProcessResponseDto> ProcessTabAsync(string tabKey, SchoolSetupProcessRequestDto request, int clientId, int? entityId, CancellationToken cancellationToken);
    }

    public sealed class SchoolSetupV3Service : ISchoolSetupV3Service
    {
        private readonly OrganizationDbContext _context;

        private static readonly Dictionary<string, TabSpec> Tabs = new(StringComparer.OrdinalIgnoreCase)
        {
            ["entity"] = new("Entity", "OrgLegalentity", new[]
            {
                Col("id", "S.No", "e.Id"),
                Col("entityTypeId", "Entity Type", "e.entitytypeid"),
                Col("code", "Code", "e.entityno"),
                Col("shortName", "Short Name", "e.shortname"),
                Col("establishedDate", "Established Date", "e.estdate"),
                Col("instituteName", "Institute Name", "e.stxt"),
                Col("registerId", "Register", "e.registerid"),
                Col("statusId", "Status", "e.orgstatusid"),
            }, "OrgLegalentity e", "ISNULL(e.utblock, 0) = 0 AND e.clientid = @clientId", "e.stxt"),

            ["course"] = new("Course", "SMCourse", new[]
            {
                Col("id", "S.No", "c.Id"),
                Col("code", "Code", "c.code"),
                Col("course", "Course", "c.stxt"),
                Col("courseCategoryId", "Course Category", "c.lovtypeid"),
                Col("syllabusId", "Syllabus", "c.syllabusid"),
                Col("attendanceTypeId", "Attendance Type", "c.attendtypeid"),
            }, "SMCourse c", "ISNULL(c.utblock, 0) = 0 AND c.clientid = @clientId AND (@entityId IS NULL OR c.entityid IS NULL OR c.entityid = @entityId)", "c.indx, c.stxt"),

            ["section"] = new("Section", "SMSection", new[]
            {
                Col("id", "S.No", "s.Id"),
                Col("code", "Section Code", "s.code"),
                Col("section", "Section", "s.stxt"),
            }, "SMSection s", "ISNULL(s.utblock, 0) = 0 AND s.clientid = @clientId AND (@entityId IS NULL OR s.entityid IS NULL OR s.entityid = @entityId)", "s.stxt"),

            ["subjectType"] = new("Subject Type", "SMSubjecttype", new[]
            {
                Col("id", "S.No", "st.Id"),
                Col("code", "Code", "st.code"),
                Col("subjectType", "Subject Type", "st.stxt"),
            }, "SMSubjecttype st", "ISNULL(st.utblock, 0) = 0 AND st.clientid = @clientId", "st.stxt"),

            ["subject"] = new("Subject", "SMSubject", new[]
            {
                Col("id", "S.No", "sub.Id"),
                Col("code", "Code", "sub.code"),
                Col("subject", "Subject", "sub.stxt"),
                Col("subjectTypeId", "Subject Type", "sub.subjecttypeid"),
                Col("courseId", "Course", "sub.courseid"),
                Col("languageId", "Language", "sub.languageid"),
                Col("subjectCategoryId", "Subject Category", "sub.subjectcatid"),
                Col("subjectClassId", "Subject Class", "sub.subjectclassid"),
                Col("coordinatorId", "Coordinator", "sub.coordinatorid"),
                Col("inchargeId", "Incharge", "sub.inchargeid"),
                Col("compulsoryId", "Compulsory", "sub.complusoryid"),
                Col("assessmentTypeId", "Assessment Type", "sub.assessmenttypeid"),
            }, "SMSubject sub", "ISNULL(sub.utblock, 0) = 0 AND sub.clientid = @clientId", "sub.stxt"),

            ["books"] = new("Books", "SMBook", new[]
            {
                Col("id", "S.No", "b.id"),
                Col("courseId", "Course", "b.courseid"),
                Col("subjectId", "Subject", "b.subjectid"),
                Col("subjectTypeId", "Subject Type", "b.subjecttypeid"),
                Col("bookName", "Book Name", "b.stxt"),
                Col("authorId", "Author", "b.authorid"),
                Col("publisherId", "Publisher", "b.publisherid"),
            }, "SMBook b", "ISNULL(b.utblock, 0) = 0 AND b.clientid = @clientId AND (@entityId IS NULL OR b.entityid IS NULL OR b.entityid = @entityId)", "b.stxt"),

            ["employeeProfile"] = new("Employee Profile", "EREmployeeinfohdr", new[]
            {
                Col("id", "S.No", "e.Id"),
                Col("firstName", "First Name", "e.firstname"),
                Col("lastName", "Last Name", "e.lastname"),
                Col("positionTypeId", "Position Type", "e.lovpositiontypeid"),
                Col("joiningDate", "Joining Date", "e.joiningdate"),
                Col("employeeTypeId", "Employee Type", "e.employeetypeid"),
                Col("departmentId", "Department", "e.depid"),
                Col("designationId", "Designation", "e.designationid"),
                Col("statusId", "Status", "e.empstatusid"),
                Col("dateOfBirth", "Date of Birth", "e.dateofbirth"),
                Col("genderId", "Gender", "e.genderid"),
                Col("cellNo", "Cell No", "e.cellno"),
            }, "EREmployeeinfohdr e", "ISNULL(e.utblock, 0) = 0 AND e.clientid = @clientId AND (@entityId IS NULL OR e.entityid IS NULL OR e.entityid = @entityId)", "e.firstname, e.lastname"),

            ["studentInformation"] = new("Student Information", "SMStudentinfo", new[]
            {
                Col("id", "S.No", "s.id"),
                Col("grNo", "G-R No", "s.grno"),
                Col("joiningDate", "Joining Date", "s.joiningdate"),
                Col("firstName", "First Name", "s.firstname"),
                Col("lastName", "Last Name", "s.lastname"),
                Col("dateOfBirth", "Date of Birth", "s.dateofbirth"),
                Col("genderId", "Gender", "s.genderid"),
                Col("bloodGroupId", "Blood Group", "s.bloodgrpid"),
                Col("courseId", "Course", "s.curcourseid"),
                Col("fatherName", "Father Name", "s.fathername"),
                Col("fatherMobile", "Father Mobile", "s.fathermobile"),
                Col("mobile", "Mobile", "s.mobile"),
            }, "SMStudentinfo s", "ISNULL(s.utblock, 0) = 0 AND s.clientid = @clientId AND (@entityId IS NULL OR s.entityid IS NULL OR s.entityid = @entityId)", "s.firstname, s.lastname"),

            ["feeArears"] = new("Fee Arears", "SMFeeChallanhdr", new[]
            {
                Col("id", "S.No", "h.id"),
                Col("studentId", "Student", "h.studentid"),
                Col("academicYearId", "Academic Year", "h.acdyearid"),
                Col("challanNo", "Challan No", "h.challanno"),
                Col("challanDate", "Challan Date", "h.challandate"),
                Col("feeGroupId", "Fee Group", "h.feegrpid"),
                Col("amount", "Amount", "h.amount"),
            }, "SMFeeChallanhdr h", "ISNULL(h.utblock, 0) = 0 AND h.clientid = @clientId AND (@entityId IS NULL OR h.entityid IS NULL OR h.entityid = @entityId)", "h.challandate DESC"),
        };

        public SchoolSetupV3Service(OrganizationDbContext context)
        {
            _context = context;
        }

        public async Task<SchoolSetupTabResponseDto> GetTabAsync(string tabKey, int pageNumber, int pageSize, int clientId, int? entityId, CancellationToken cancellationToken)
        {
            if (clientId <= 0) throw new ArgumentException("ClientId is required.");
            if (!Tabs.TryGetValue(tabKey, out var spec))
            {
                throw new ArgumentException($"Unknown school setup tab '{tabKey}'.", nameof(tabKey));
            }

            pageNumber = Math.Max(1, pageNumber);
            pageSize = Math.Clamp(pageSize, 1, 500);

            if (!await TableExistsAsync(spec.TableName, cancellationToken))
            {
                return new SchoolSetupTabResponseDto
                {
                    Tab = spec.Title,
                    Columns = spec.Columns.Select(x => new SchoolSetupColumnDto { Field = x.Field, Header = x.Header }).ToList(),
                    Rows = new List<Dictionary<string, object?>>(),
                    TotalCount = 0
                };
            }

            var offset = (pageNumber - 1) * pageSize;
            var selectList = string.Join(", ", spec.Columns.Select(x => $"{x.Expression} AS [{x.Field}]"));
            var sql = $@"
SELECT {selectList}
FROM {spec.FromClause}
WHERE {spec.WhereClause}
ORDER BY {spec.OrderBy}
OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;

SELECT COUNT(1)
FROM {spec.FromClause}
WHERE {spec.WhereClause};";

            var rows = new List<Dictionary<string, object?>>();
            var totalCount = 0;

            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            AddParameter(command, "@clientId", clientId);
            AddParameter(command, "@entityId", entityId);
            AddParameter(command, "@offset", offset);
            AddParameter(command, "@pageSize", pageSize);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var row = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);
                foreach (var column in spec.Columns)
                {
                    var value = reader[column.Field];
                    row[column.Field] = value == DBNull.Value ? null : value;
                }
                rows.Add(row);
            }

            if (await reader.NextResultAsync(cancellationToken) && await reader.ReadAsync(cancellationToken))
            {
                totalCount = Convert.ToInt32(reader.GetValue(0));
            }

            return new SchoolSetupTabResponseDto
            {
                Tab = spec.Title,
                Columns = spec.Columns.Select(x => new SchoolSetupColumnDto { Field = x.Field, Header = x.Header }).ToList(),
                Rows = rows,
                TotalCount = totalCount
            };
        }

        public async Task<SchoolSetupProcessResponseDto> ProcessTabAsync(string tabKey, SchoolSetupProcessRequestDto request, int clientId, int? entityId, CancellationToken cancellationToken)
        {
            if (clientId <= 0) throw new ArgumentException("ClientId is required.");
            if (!Tabs.TryGetValue(tabKey, out var spec))
            {
                throw new ArgumentException($"Unknown school setup tab '{tabKey}'.", nameof(tabKey));
            }

            if (string.Equals(tabKey, "employeeProfile", StringComparison.OrdinalIgnoreCase))
            {
                var employeeProcessedCount = await ProcessEmployeeProfilesAsync(request.Rows ?? new(), clientId, entityId, cancellationToken);
                return new SchoolSetupProcessResponseDto
                {
                    Tab = spec.Title,
                    ProcessedCount = employeeProcessedCount,
                    Message = $"{employeeProcessedCount} employee profile row(s) saved successfully."
                };
            }

            var rows = request.Rows ?? new();
            var saved = tabKey.ToLowerInvariant() switch
            {
                "entity" => await ProcessEntitiesAsync(rows, clientId, cancellationToken),
                "course" => await ProcessCoursesAsync(rows, clientId, entityId, cancellationToken),
                "section" => await ProcessSectionsAsync(rows, clientId, entityId, cancellationToken),
                "subjecttype" => await ProcessSubjectTypesAsync(rows, clientId, entityId, cancellationToken),
                "subject" => await ProcessSubjectsAsync(rows, clientId, entityId, cancellationToken),
                "books" => await ProcessBooksAsync(rows, clientId, entityId, cancellationToken),
                "studentinformation" => await ProcessStudentsAsync(rows, clientId, entityId, cancellationToken),
                "feearears" => await ProcessFeeArearsAsync(rows, clientId, entityId, cancellationToken),
                _ => throw new ArgumentException($"Import processing for '{spec.Title}' is not configured yet.")
            };
            return new SchoolSetupProcessResponseDto
            {
                Tab = spec.Title,
                ProcessedCount = saved,
                Message = $"{saved} {spec.Title.ToLowerInvariant()} row(s) saved successfully."
            };

        }

        private async Task<int> ProcessEntitiesAsync(List<Dictionary<string, object?>> rows, int clientId, CancellationToken cancellationToken)
        {
            const string title = "Entity";
            return await InsertRowsAsync(rows, title, new[] { "code", "entityTypeId", "shortName", "establishedDate", "instituteName" }, async (connection, transaction, row, rowNumber) =>
            {
                var code = ReadText(row, "code");
                var name = ReadText(row, "instituteName");
                var shortName = ReadText(row, "shortName").ToUpperInvariant();
                ValidateEntityRow(code, shortName, name, rowNumber);
                if (!int.TryParse(ReadText(row, "entityTypeId"), out var entityTypeId) || entityTypeId <= 0)
                    throw new ArgumentException($"Row {rowNumber}: Entity Type is required.");
                if (!TryParseImportDate(ReadText(row, "establishedDate"), out var establishedDate))
                    throw new ArgumentException($"Row {rowNumber}: Established Date is invalid.");
                if (establishedDate.Date > DateTime.Today)
                    throw new ArgumentException($"Row {rowNumber}: Established Date cannot be in the future.");

                var entityTypeExists = await LookupIdAsync(connection, transaction,
                    @"SELECT TOP 1 Id
FROM lov
WHERE Id=@value
  AND ISNULL(utblock,0)=0
  AND (UPPER(ISNULL(vtype,''))='OTYP'
       OR UPPER(ISNULL(vtype,'')) LIKE '%ENTITY%'
       OR UPPER(ISNULL(code,'')) LIKE '%ENTITY%'
       OR UPPER(ISNULL(vtype,'')) LIKE '%ORGTYPE%'
       OR UPPER(ISNULL(code,'')) LIKE '%ORGTYPE%')",
                    clientId, entityTypeId.ToString(CultureInfo.InvariantCulture), cancellationToken);
                if (entityTypeExists is null)
                    throw new ArgumentException($"Row {rowNumber}: selected Entity Type is invalid.");

                var duplicateCode = await LookupIdAsync(connection, transaction,
                    "SELECT TOP 1 Id FROM OrgLegalentity WHERE clientid=@clientId AND entityno=@value AND ISNULL(utblock,0)=0",
                    clientId, code, cancellationToken);
                if (duplicateCode is not null)
                    throw new ArgumentException($"Row {rowNumber}: Entity Code '{code}' already exists.");

                var duplicateName = await LookupIdAsync(connection, transaction,
                    "SELECT TOP 1 Id FROM OrgLegalentity WHERE clientid=@clientId AND stxt=@value AND ISNULL(utblock,0)=0",
                    clientId, name, cancellationToken);
                if (duplicateName is not null)
                    throw new ArgumentException($"Row {rowNumber}: Institute Name '{name}' already exists.");

                await InsertAsync(connection, transaction, @"IF EXISTS (SELECT 1 FROM OrgLegalentity WHERE clientid=@clientId AND (entityno=@code OR stxt=@name) AND ISNULL(utblock,0)=0)
    THROW 50001, 'An entity with the same code or institute name already exists.', 1;
DECLARE @id int=ISNULL((SELECT MAX(Id) FROM OrgLegalentity WITH (UPDLOCK,HOLDLOCK)),0)+1;
INSERT INTO OrgLegalentity (Id,entityno,clientid,stxt,shortname,estdate,entitytypeid,utsysgen,utblock,utispartial,utaddtime,utadduser,utedittime,utedituser,utrecordversion)
VALUES (@id,@code,@clientId,@name,@shortName,@establishedDate,@entityTypeId,0,0,0,GETDATE(),'v3-service',GETDATE(),'v3-service',0);",
                    new Dictionary<string, object?>
                    {
                        ["@clientId"] = clientId,
                        ["@code"] = code,
                        ["@name"] = name,
                        ["@shortName"] = shortName,
                        ["@establishedDate"] = establishedDate,
                        ["@entityTypeId"] = entityTypeId
                    }, cancellationToken);
            }, cancellationToken);
        }

        private static void ValidateEntityRow(string code, string shortName, string name, int rowNumber)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException($"Row {rowNumber}: Code is required.");
            if (code.Length > 20)
                throw new ArgumentException($"Row {rowNumber}: Code cannot exceed 20 characters.");
            if (string.IsNullOrWhiteSpace(shortName))
                throw new ArgumentException($"Row {rowNumber}: Short Name is required.");
            if (shortName.Length > 20)
                throw new ArgumentException($"Row {rowNumber}: Short Name cannot exceed 20 characters.");
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException($"Row {rowNumber}: Institute Name is required.");
            if (name.Length > 150)
                throw new ArgumentException($"Row {rowNumber}: Institute Name cannot exceed 150 characters.");
        }

        private async Task<int> ProcessCoursesAsync(List<Dictionary<string, object?>> rows, int clientId, int? entityId, CancellationToken cancellationToken)
        {
            RequireEntityAndRows(rows, entityId, "Course");
            var categories = await _context.Lovs.AsNoTracking().Where(x => !x.utblock).ToListAsync(cancellationToken);
            return await InsertRowsAsync(rows, "Course", new[] { "coursecode", "coursestxt", "coursecat" }, async (connection, transaction, row, rowNumber) =>
            {
                var category = categories.FirstOrDefault(x => Same(x.stxt, ReadText(row, "coursecat"))) ?? throw new ArgumentException($"Row {rowNumber} has an unknown coursecat.");
                await InsertAsync(connection, transaction, @"DECLARE @id int=ISNULL((SELECT MAX(Id) FROM SMCourse WITH (UPDLOCK,HOLDLOCK)),0)+1; INSERT INTO SMCourse (Id,clientid,entityid,code,stxt,lovtypeid,syllabusid,attendtypeid,utsysgen,utblock,utispartial,utaddtime,utadduser,utedittime,utedituser,utrecordversion) VALUES (@id,@clientId,@entityId,@code,@name,@category,1,1,0,0,0,GETDATE(),'v3-service',GETDATE(),'v3-service',0);", new Dictionary<string, object?> { ["@clientId"] = clientId, ["@entityId"] = entityId, ["@code"] = ReadText(row,"coursecode"), ["@name"] = ReadText(row,"coursestxt"), ["@category"] = category.Id }, cancellationToken);
            }, cancellationToken);
        }

        private async Task<int> ProcessSectionsAsync(List<Dictionary<string, object?>> rows, int clientId, int? entityId, CancellationToken cancellationToken) =>
            await InsertRowsAsync(rows, "Section", new[] { "sectioncode", "sectionstxt" }, async (connection, transaction, row, rowNumber) =>
                await InsertAsync(connection, transaction, @"DECLARE @id int=ISNULL((SELECT MAX(Id) FROM SMSection WITH (UPDLOCK,HOLDLOCK)),0)+1; INSERT INTO SMSection (Id,clientid,entityid,code,stxt,utsysgen,utblock,utispartial,utaddtime,utadduser,utedittime,utedituser,utrecordversion) VALUES (@id,@clientId,@entityId,@code,@name,0,0,0,GETDATE(),'v3-service',GETDATE(),'v3-service',0);", new Dictionary<string, object?> { ["@clientId"] = clientId, ["@entityId"] = entityId, ["@code"] = ReadText(row,"sectioncode"), ["@name"] = ReadText(row,"sectionstxt") }, cancellationToken), cancellationToken);

        private async Task<int> ProcessSubjectTypesAsync(List<Dictionary<string, object?>> rows, int clientId, int? entityId, CancellationToken cancellationToken) =>
            await InsertRowsAsync(rows, "Subject Type", new[] { "subjecttypecode", "subjecttypestxt" }, async (connection, transaction, row, rowNumber) =>
                await InsertAsync(connection, transaction, @"DECLARE @id int=ISNULL((SELECT MAX(Id) FROM SMSubjecttype WITH (UPDLOCK,HOLDLOCK)),0)+1; INSERT INTO SMSubjecttype (Id,clientid,code,stxt,utsysgen,utblock,utispartial,utaddtime,utadduser,utedittime,utedituser,utrecordversion) VALUES (@id,@clientId,@code,@name,0,0,0,GETDATE(),'v3-service',GETDATE(),'v3-service',0);", new Dictionary<string, object?> { ["@clientId"] = clientId, ["@code"] = ReadText(row,"subjecttypecode"), ["@name"] = ReadText(row,"subjecttypestxt") }, cancellationToken), cancellationToken);

        private async Task<int> ProcessSubjectsAsync(List<Dictionary<string, object?>> rows, int clientId, int? entityId, CancellationToken cancellationToken)
        {
            RequireEntityAndRows(rows, entityId, "Subject");
            var lovs = await _context.Lovs.AsNoTracking().Where(x => !x.utblock).ToListAsync(cancellationToken);
            return await InsertRowsAsync(rows, "Subject", new[] { "subjectcode", "subjectstxt", "subjecttypestxt", "coursestxt", "langstxt", "subjectcatstxt", "subjectclassstxt", "complusorystxt", "assessmenttypestxt" }, async (connection, transaction, row, rowNumber) =>
            {
                var courseId = await LookupIdAsync(connection, transaction, "SELECT TOP 1 Id FROM SMCourse WHERE clientid=@clientId AND stxt=@value AND ISNULL(utblock,0)=0", clientId, ReadText(row, "coursestxt"), cancellationToken);
                var typeId = await LookupIdAsync(connection, transaction, "SELECT TOP 1 Id FROM SMSubjecttype WHERE clientid=@clientId AND stxt=@value AND ISNULL(utblock,0)=0", clientId, ReadText(row, "subjecttypestxt"), cancellationToken);
                var languageId = await LookupIdAsync(connection, transaction, "SELECT TOP 1 Id FROM OrgLanguage WHERE stxt=@value AND ISNULL(utblock,0)=0", clientId, ReadText(row, "langstxt"), cancellationToken);
                var category = lovs.FirstOrDefault(x => Same(x.stxt, ReadText(row, "subjectcatstxt")))?.Id;
                var subjectClass = lovs.FirstOrDefault(x => Same(x.stxt, ReadText(row, "subjectclassstxt")))?.Id;
                var compulsory = lovs.FirstOrDefault(x => Same(x.stxt, ReadText(row, "complusorystxt")))?.Id;
                var assessment = lovs.FirstOrDefault(x => Same(x.stxt, ReadText(row, "assessmenttypestxt")))?.Id;
                if (courseId is null || typeId is null || languageId is null || category is null || subjectClass is null || compulsory is null || assessment is null)
                    throw new ArgumentException($"Row {rowNumber} contains a course, subject type, language, category, class, compulsory, or assessment value that does not exist.");
                await InsertAsync(connection, transaction, @"DECLARE @id int=ISNULL((SELECT MAX(Id) FROM SMSubject WITH (UPDLOCK,HOLDLOCK)),0)+1; INSERT INTO SMSubject (Id,clientid,courseid,subjecttypeid,languageid,subjectcatid,subjectclassid,complusoryid,assessmenttypeid,code,stxt,utsysgen,utblock,utispartial,utaddtime,utadduser,utedittime,utedituser,utrecordversion) VALUES (@id,@clientId,@courseId,@typeId,@languageId,@category,@classId,@compulsory,@assessment,@code,@name,0,0,0,GETDATE(),'v3-service',GETDATE(),'v3-service',0);", new Dictionary<string, object?> { ["@clientId"] = clientId, ["@courseId"] = courseId, ["@typeId"] = typeId, ["@languageId"] = languageId, ["@category"] = category, ["@classId"] = subjectClass, ["@compulsory"] = compulsory, ["@assessment"] = assessment, ["@code"] = ReadText(row,"subjectcode"), ["@name"] = ReadText(row,"subjectstxt") }, cancellationToken);
            }, cancellationToken);
        }

        private async Task<int> ProcessBooksAsync(List<Dictionary<string, object?>> rows, int clientId, int? entityId, CancellationToken cancellationToken)
        {
            RequireEntityAndRows(rows, entityId, "Books");
            return await InsertRowsAsync(rows, "Books", new[] { "coursestxt", "subjectstxt", "subjecttypestxt", "bookstxt", "authorstxt", "publisherstxt" }, async (connection, transaction, row, rowNumber) =>
            {
                var courseId = await LookupIdAsync(connection, transaction, "SELECT TOP 1 Id FROM SMCourse WHERE clientid=@clientId AND stxt=@value AND ISNULL(utblock,0)=0", clientId, ReadText(row,"coursestxt"), cancellationToken);
                var subjectId = await LookupIdAsync(connection, transaction, "SELECT TOP 1 Id FROM SMSubject WHERE clientid=@clientId AND stxt=@value AND ISNULL(utblock,0)=0", clientId, ReadText(row,"subjectstxt"), cancellationToken);
                var subjectTypeId = await LookupIdAsync(connection, transaction, "SELECT TOP 1 Id FROM SMSubjecttype WHERE clientid=@clientId AND stxt=@value AND ISNULL(utblock,0)=0", clientId, ReadText(row,"subjecttypestxt"), cancellationToken);
                var authorId = await LookupIdAsync(connection, transaction, "SELECT TOP 1 id FROM LMAuthor WHERE clientid=@clientId AND name=@value AND ISNULL(utblock,0)=0", clientId, ReadText(row,"authorstxt"), cancellationToken);
                var publisherId = await LookupIdAsync(connection, transaction, "SELECT TOP 1 id FROM LMPublisher WHERE clientid=@clientId AND stxt=@value AND ISNULL(utblock,0)=0", clientId, ReadText(row,"publisherstxt"), cancellationToken);
                if (courseId is null || subjectId is null || subjectTypeId is null || authorId is null || publisherId is null) throw new ArgumentException($"Row {rowNumber} contains a course, subject, subject type, author, or publisher that does not exist.");
                await InsertAsync(connection, transaction, @"DECLARE @id int=ISNULL((SELECT MAX(id) FROM SMBook WITH (UPDLOCK,HOLDLOCK)),0)+1; DECLARE @generatedBookNo varchar(50)=RIGHT('00000000'+CONVERT(varchar(20),ISNULL((SELECT MAX(TRY_CONVERT(int,bookno)) FROM SMBook),0)+1),8); INSERT INTO SMBook (id,clientid,entityid,bookno,stxt,courseid,subjectid,subjecttypeid,authorid,publisherid,utsysgen,utblock,utispartial,utaddtime,utadduser,utedittime,utedituser,utrecordversion) VALUES (@id,@clientId,@entityId,@generatedBookNo,@name,@courseId,@subjectId,@subjectTypeId,@authorId,@publisherId,0,0,0,GETDATE(),'v3-service',GETDATE(),'v3-service',0);", new Dictionary<string,object?> { ["@clientId"]=clientId,["@entityId"]=entityId,["@name"]=ReadText(row,"bookstxt"),["@courseId"]=courseId,["@subjectId"]=subjectId,["@subjectTypeId"]=subjectTypeId,["@authorId"]=authorId,["@publisherId"]=publisherId }, cancellationToken);
            }, cancellationToken);
        }

        private async Task<int> ProcessStudentsAsync(List<Dictionary<string, object?>> rows, int clientId, int? entityId, CancellationToken cancellationToken)
        {
            RequireEntityAndRows(rows, entityId, "Student Information");
            var lovs = await _context.Lovs.AsNoTracking().Where(x => !x.utblock).ToListAsync(cancellationToken);
            return await InsertRowsAsync(rows, "Student Information", new[] { "grno", "joiningdate", "regdate", "firstname", "dateofbirth", "genderstxt", "bloodgrpstxt", "coursestxt", "sectionstxt", "fathername", "fathermobile" }, async (connection, transaction, row, rowNumber) =>
            {
                if (!TryParseImportDate(ReadText(row,"joiningdate"), out var joining) || !TryParseImportDate(ReadText(row,"regdate"), out var registration) || !TryParseImportDate(ReadText(row,"dateofbirth"), out var birth)) throw new ArgumentException($"Row {rowNumber} has an invalid joiningdate, regdate, or dateofbirth.");
                var courseId = await LookupIdAsync(connection, transaction, "SELECT TOP 1 Id FROM SMCourse WHERE clientid=@clientId AND stxt=@value AND ISNULL(utblock,0)=0", clientId, ReadText(row,"coursestxt"), cancellationToken);
                var sectionId = await LookupIdAsync(connection, transaction, "SELECT TOP 1 Id FROM SMSection WHERE clientid=@clientId AND stxt=@value AND ISNULL(utblock,0)=0", clientId, ReadText(row,"sectionstxt"), cancellationToken);
                var gender = lovs.FirstOrDefault(x => Same(x.stxt, ReadText(row,"genderstxt")))?.Id;
                var blood = lovs.FirstOrDefault(x => Same(x.stxt, ReadText(row,"bloodgrpstxt")))?.Id;
                if (courseId is null || sectionId is null || gender is null || blood is null) throw new ArgumentException($"Row {rowNumber} contains a course, section, gender, or blood group that does not exist.");
                await InsertAsync(connection, transaction, @"DECLARE @id int=ISNULL((SELECT MAX(Id) FROM SMStudentinfo WITH (UPDLOCK,HOLDLOCK)),0)+1; INSERT INTO SMStudentinfo (Id,clientid,entityid,grno,studentno,joiningdate,regdate,firstname,lastname,dateofbirth,genderid,bloodgrpid,curcourseid,cursectionid,fathername,fathermobile,fathercnic,mothername,mothercnic,mothermobile,address,mobile,emergencyno,isgurdian,ismother_gurdian,utsysgen,utblock,utispartial,utaddtime,utadduser,utedittime,utedituser,utrecordversion) VALUES (@id,@clientId,@entityId,@grno,@studentNo,@joining,@registration,@first,@last,@birth,@gender,@blood,@course,@section,@father,@fatherMobile,@fatherCnic,@mother,@motherCnic,@motherMobile,@address,@mobile,@emergency,@guardian,@motherGuardian,0,0,0,GETDATE(),'v3-service',GETDATE(),'v3-service',0);", new Dictionary<string,object?> { ["@clientId"]=clientId,["@entityId"]=entityId,["@grno"]=ReadText(row,"grno"),["@studentNo"]=ReadText(row,"studentid"),["@joining"]=joining,["@registration"]=registration,["@first"]=ReadText(row,"firstname"),["@last"]=ReadText(row,"lastname"),["@birth"]=birth,["@gender"]=gender,["@blood"]=blood,["@course"]=courseId,["@section"]=sectionId,["@father"]=ReadText(row,"fathername"),["@fatherMobile"]=ReadText(row,"fathermobile"),["@fatherCnic"]=ReadText(row,"fathercnic"),["@mother"]=ReadText(row,"mothername"),["@motherCnic"]=ReadText(row,"mothercnic"),["@motherMobile"]=ReadText(row,"mothermobile"),["@address"]=ReadText(row,"address"),["@mobile"]=ReadText(row,"mobile"),["@emergency"]=ReadText(row,"emergencyno"),["@guardian"]=Same(ReadText(row,"isguardian"),"Yes"),["@motherGuardian"]=Same(ReadText(row,"ismother_gurdian"),"Yes") }, cancellationToken);
            }, cancellationToken);
        }

        private async Task<int> ProcessFeeArearsAsync(List<Dictionary<string, object?>> rows, int clientId, int? entityId, CancellationToken cancellationToken)
        {
            RequireEntityAndRows(rows, entityId, "Fee Arears");
            var required = new[] { "studentname", "grno", "academicyearstxt", "challanno", "challandate", "feegrpstxt", "feetypestxt", "amount", "periodstxt", "perioddtlstxt", "duedate", "validtill" };
            return await InsertRowsAsync(rows, "Fee Arears", required, async (connection, transaction, row, rowNumber) =>
            {
                if (!TryParseImportDate(ReadText(row, "challandate"), out var challanDate)
                    || !TryParseImportDate(ReadText(row, "duedate"), out var dueDate)
                    || !TryParseImportDate(ReadText(row, "validtill"), out var validTill))
                    throw new ArgumentException($"Row {rowNumber} has an invalid challandate, duedate, or validtill.");
                if (!decimal.TryParse(ReadText(row, "amount"), NumberStyles.Number, CultureInfo.InvariantCulture, out var amount) || amount <= 0)
                    throw new ArgumentException($"Row {rowNumber} has an invalid amount.");

                var student = await LookupRowAsync(connection, transaction, @"SELECT TOP 1 Id, firstname, CONCAT(ISNULL(firstname,''), ' ', ISNULL(lastname,'')) AS fullName, curcourseid, cursectionid FROM SMStudentinfo WHERE clientid=@clientId AND entityid=@entityId AND grno=@value AND ISNULL(utblock,0)=0", clientId, entityId.Value, ReadText(row, "grno"), cancellationToken);
                var feeGroupId = await LookupIdAsync(connection, transaction, "SELECT TOP 1 Id FROM SMFeegrphdr WHERE clientid=@clientId AND entityid=@entityId AND stxt=@value AND ISNULL(utblock,0)=0", clientId, entityId.Value, ReadText(row, "feegrpstxt"), cancellationToken);
                var feeTypeId = await LookupIdAsync(connection, transaction, "SELECT TOP 1 Id FROM SMFeetype WHERE clientid=@clientId AND entityid=@entityId AND stxt=@value AND ISNULL(utblock,0)=0", clientId, entityId.Value, ReadText(row, "feetypestxt"), cancellationToken);
                var academicYearId = await LookupIdAsync(connection, transaction, "SELECT TOP 1 Id FROM SMAcademicyear WHERE clientid=@clientId AND entityid=@entityId AND stxt=@value AND ISNULL(utblock,0)=0", clientId, entityId.Value, ReadText(row, "academicyearstxt"), cancellationToken);
                var periodDetailId = await LookupIdAsync(connection, transaction, @"SELECT TOP 1 d.Id FROM SMPerioddtl d INNER JOIN SMPeriodhdr h ON h.Id=d.periodid WHERE h.clientid=@clientId AND h.entityid=@entityId AND h.stxt=@value AND d.stxt=@detail AND ISNULL(h.utblock,0)=0 AND ISNULL(d.utblock,0)=0", clientId, entityId.Value, ReadText(row, "periodstxt"), ReadText(row, "perioddtlstxt"), cancellationToken);
                if (student is null || feeGroupId is null || feeTypeId is null || academicYearId is null || periodDetailId is null)
                    throw new ArgumentException($"Row {rowNumber} contains a student, fee group, fee type, academic year, or period that does not exist.");

                var studentName = ReadText(row, "studentname");
                if (!string.IsNullOrWhiteSpace(studentName))
                {
                    var actualName = student["fullName"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(actualName) && !Same(actualName, studentName))
                        throw new ArgumentException($"Row {rowNumber} studentname does not match the supplied grno.");
                }

                var values = new Dictionary<string, object?>
                {
                    ["@clientId"] = clientId, ["@entityId"] = entityId, ["@studentId"] = student["Id"], ["@courseId"] = student["curcourseid"], ["@sectionId"] = student["cursectionid"],
                    ["@academicYearId"] = academicYearId, ["@feeGroupId"] = feeGroupId, ["@periodDetailId"] = periodDetailId, ["@challanNo"] = ReadText(row, "challanno"), ["@challanDate"] = challanDate,
                    ["@dueDate"] = dueDate, ["@validTill"] = validTill, ["@amount"] = amount, ["@feeTypeId"] = feeTypeId
                };
                var challanId = await ExecuteScalarLongAsync(connection, transaction, @"IF EXISTS (SELECT 1 FROM SMFeechallanhdr WHERE clientid=@clientId AND entityid=@entityId AND challanno=@challanNo AND ISNULL(utblock,0)=0) THROW 50001, 'A challan with this number already exists.', 1; DECLARE @id bigint=ISNULL((SELECT MAX(Id) FROM SMFeechallanhdr WITH (UPDLOCK,HOLDLOCK)),0)+1; INSERT INTO SMFeechallanhdr (Id,clientid,entityid,studentid,courseid,sectionid,acdyearid,feegrpid,perioddtlid,challanno,challandate,duedate,challanvalidity,totalamount,amount,balanceamount,feestatusid,challanstatusid,feenatureid,utsysgen,utblock,utispartial,utaddtime,utadduser,utedittime,utedituser,utrecordversion) VALUES (@id,@clientId,@entityId,@studentId,@courseId,@sectionId,@academicYearId,@feeGroupId,@periodDetailId,@challanNo,@challanDate,@dueDate,@validTill,@amount,@amount,@amount,83,220,60,0,0,0,GETDATE(),'v3-service',GETDATE(),'v3-service',0); SELECT @id;", values, cancellationToken);
                values["@challanId"] = challanId;
                await InsertAsync(connection, transaction, @"DECLARE @id bigint=ISNULL((SELECT MAX(Id) FROM SMFeechallandtl WITH (UPDLOCK,HOLDLOCK)),0)+1; INSERT INTO SMFeechallandtl (Id,challanid,lineindex,feeid,lovgrouptypeid,amount,netamount,utsysgen,utblock,utispartial,utaddtime,utadduser,utedittime,utedituser,utrecordversion) VALUES (@id,@challanId,1,@feeTypeId,60,@amount,@amount,0,0,0,GETDATE(),'v3-service',GETDATE(),'v3-service',0);", values, cancellationToken);
            }, cancellationToken);
        }

        private static void RequireEntityAndRows(List<Dictionary<string, object?>> rows, int? entityId, string title)
        {
            if (!entityId.HasValue) throw new ArgumentException($"Select an entity before importing {title}.");
            if (rows.Count == 0) throw new ArgumentException($"Select at least one {title} row before proceeding.");
        }

        private async Task<int> InsertRowsAsync(List<Dictionary<string, object?>> rows, string title, IReadOnlyList<string> required, Func<System.Data.Common.DbConnection, System.Data.Common.DbTransaction, Dictionary<string, object?>, int, Task> insert, CancellationToken cancellationToken)
        {
            if (rows.Count == 0) throw new ArgumentException($"Select at least one {title} row before proceeding.");
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open) await connection.OpenAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
            try
            {
                for (var index = 0; index < rows.Count; index++)
                {
                    var missing = required.Where(field => string.IsNullOrWhiteSpace(ReadText(rows[index], field))).ToList();
                    if (missing.Count > 0) throw new ArgumentException($"Row {index + 2} is missing mandatory values: {string.Join(", ", missing)}.");
                    await insert(connection, transaction, rows[index], index + 2);
                }
                await transaction.CommitAsync(cancellationToken);
                return rows.Count;
            }
            catch { await transaction.RollbackAsync(cancellationToken); throw; }
        }

        private async Task InsertAsync(System.Data.Common.DbConnection connection, System.Data.Common.DbTransaction transaction, string sql, IReadOnlyDictionary<string, object?> values, CancellationToken cancellationToken)
        {
            await using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = sql;
            _context.AuditImport(command);
            foreach (var value in values) AddParameter(command, value.Key, value.Value);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        private static async Task<int?> LookupIdAsync(System.Data.Common.DbConnection connection, System.Data.Common.DbTransaction transaction, string sql, int clientId, string value, CancellationToken cancellationToken) =>
            await LookupIdAsync(connection, transaction, sql, clientId, null, value, null, cancellationToken);

        private static async Task<int?> LookupIdAsync(System.Data.Common.DbConnection connection, System.Data.Common.DbTransaction transaction, string sql, int clientId, int entityId, string value, CancellationToken cancellationToken) =>
            await LookupIdAsync(connection, transaction, sql, clientId, entityId, value, null, cancellationToken);

        private static async Task<int?> LookupIdAsync(System.Data.Common.DbConnection connection, System.Data.Common.DbTransaction transaction, string sql, int clientId, int entityId, string value, string detail, CancellationToken cancellationToken) =>
            await LookupIdAsync(connection, transaction, sql, clientId, (int?)entityId, value, detail, cancellationToken);

        private static async Task<int?> LookupIdAsync(System.Data.Common.DbConnection connection, System.Data.Common.DbTransaction transaction, string sql, int clientId, int? entityId, string value, string? detail, CancellationToken cancellationToken)
        {
            await using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = sql;
            AddParameter(command, "@clientId", clientId);
            if (entityId.HasValue) AddParameter(command, "@entityId", entityId.Value);
            AddParameter(command, "@value", value);
            if (detail != null) AddParameter(command, "@detail", detail);
            var result = await command.ExecuteScalarAsync(cancellationToken);
            return result is null or DBNull ? null : Convert.ToInt32(result);
        }

        private async Task<long> ExecuteScalarLongAsync(System.Data.Common.DbConnection connection, System.Data.Common.DbTransaction transaction, string sql, IReadOnlyDictionary<string, object?> values, CancellationToken cancellationToken)
        {
            await using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = sql;
            _context.AuditImport(command);
            foreach (var value in values) AddParameter(command, value.Key, value.Value);
            return Convert.ToInt64(await command.ExecuteScalarAsync(cancellationToken));
        }

        private static async Task<Dictionary<string, object?>?> LookupRowAsync(System.Data.Common.DbConnection connection, System.Data.Common.DbTransaction transaction, string sql, int clientId, int entityId, string value, CancellationToken cancellationToken)
        {
            await using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = sql;
            AddParameter(command, "@clientId", clientId);
            AddParameter(command, "@entityId", entityId);
            AddParameter(command, "@value", value);
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (!await reader.ReadAsync(cancellationToken)) return null;
            return Enumerable.Range(0, reader.FieldCount).ToDictionary(reader.GetName, index => reader.IsDBNull(index) ? null : reader.GetValue(index));
        }

        private async Task<int> ProcessEmployeeProfilesAsync(List<Dictionary<string, object?>> rows, int clientId, int? entityId, CancellationToken cancellationToken)
        {
            if (!entityId.HasValue) throw new ArgumentException("Select an entity before importing Employee Profiles.");
            if (rows.Count == 0) throw new ArgumentException("Select at least one employee row before proceeding.");

            var required = new[] { "employeecode", "firstname", "joiningdate", "depstxt", "designationstxt", "dateofbirth", "genderstxt", "employeetypestxt", "employecatstxt", "cellno" };
            var normalizedRows = rows.Select((row, index) => new { Row = row, RowNumber = index + 2 }).ToList();
            foreach (var item in normalizedRows)
            {
                var missing = required.Where(field => string.IsNullOrWhiteSpace(ReadText(item.Row, field))).ToList();
                if (missing.Count > 0) throw new ArgumentException($"Row {item.RowNumber} is missing mandatory values: {string.Join(", ", missing)}.");
            }

            var departments = await _context.OrgDepartmentLegacy.AsNoTracking().Where(x => x.clientid == clientId && !(x.utblock ?? false)).ToListAsync(cancellationToken);
            var designations = await _context.OrgDesignationLegacy.AsNoTracking().Where(x => x.clientid == clientId && !(x.utblock ?? false)).ToListAsync(cancellationToken);
            var lovs = await _context.Lovs.AsNoTracking().Where(x => !x.utblock).ToListAsync(cancellationToken);
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open) await connection.OpenAsync(cancellationToken);
            await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
            try
            {
                var mapped = new List<EmployeeProfileImportRow>();
                foreach (var item in normalizedRows)
                {
                    var departmentName = ReadText(item.Row, "depstxt");
                    var designationName = ReadText(item.Row, "designationstxt");
                    var department = departments.FirstOrDefault(x => Same(x.depstxt, departmentName));
                    if (department == null)
                    {
                        var departmentId = await CreateDepartmentAsync(connection, transaction, clientId, departmentName, cancellationToken);
                        department = new OrgDepartmentLegacy { Id = departmentId, depstxt = departmentName, clientid = clientId, utblock = false };
                        departments.Add(department);
                    }

                    var designation = designations.FirstOrDefault(x => Same(x.stxt, designationName));
                    if (designation == null)
                    {
                        var designationId = await CreateDesignationAsync(connection, transaction, clientId, designationName, cancellationToken);
                        designation = new OrgDesignationLegacy { Id = designationId, stxt = designationName, clientid = clientId, utblock = false };
                        designations.Add(designation);
                    }

                    var gender = lovs.FirstOrDefault(x => Same(x.stxt, ReadText(item.Row, "genderstxt")));
                    var employeeType = lovs.FirstOrDefault(x => Same(x.stxt, ReadText(item.Row, "employeetypestxt")));
                    var employeeCategory = lovs.FirstOrDefault(x => Same(x.stxt, ReadText(item.Row, "employecatstxt")));
                    var teacherCategoryText = ReadText(item.Row, "teachercatstxt");
                    var teacherCategory = string.IsNullOrWhiteSpace(teacherCategoryText) ? null : lovs.FirstOrDefault(x => Same(x.stxt, teacherCategoryText));
                    var invalidLookups = new List<string>();
                    if (gender == null) invalidLookups.Add($"Gender '{ReadText(item.Row, "genderstxt")}'");
                    if (employeeType == null) invalidLookups.Add($"Employee Type '{ReadText(item.Row, "employeetypestxt")}'");
                    if (employeeCategory == null) invalidLookups.Add($"Employee Category '{ReadText(item.Row, "employecatstxt")}'");
                    if (!string.IsNullOrWhiteSpace(teacherCategoryText) && teacherCategory == null)
                        invalidLookups.Add($"Teacher Category '{teacherCategoryText}'");
                    if (invalidLookups.Count > 0)
                        throw new ArgumentException($"Row {item.RowNumber}: {string.Join(", ", invalidLookups)} does not exist for the selected client.");
                    if (!TryParseImportDate(ReadText(item.Row, "joiningdate"), out var joiningDate) || !TryParseImportDate(ReadText(item.Row, "dateofbirth"), out var birthDate))
                        throw new ArgumentException($"Row {item.RowNumber} has an invalid joiningdate or dateofbirth.");

                    mapped.Add(new EmployeeProfileImportRow(item.RowNumber, item.Row, department.Id, designation.Id, gender.Id, employeeType.Id, employeeCategory.Id, teacherCategory?.Id, joiningDate, birthDate));
                }

                var duplicateCodes = mapped.GroupBy(x => ReadText(x.Data, "employeecode"), StringComparer.OrdinalIgnoreCase).FirstOrDefault(x => x.Count() > 1);
                if (duplicateCodes != null) throw new ArgumentException($"Employee Code '{duplicateCodes.Key}' appears more than once in the uploaded rows.");

                foreach (var employee in mapped)
                {
                    await using var duplicateCheck = connection.CreateCommand();
                    duplicateCheck.Transaction = transaction;
                    duplicateCheck.CommandText = "SELECT COUNT(1) FROM EREmployeeinfohdr WHERE clientid = @clientId AND employeecode = @employeeCode AND ISNULL(utblock, 0) = 0";
                    AddParameter(duplicateCheck, "@clientId", clientId);
                    AddParameter(duplicateCheck, "@employeeCode", ReadText(employee.Data, "employeecode"));
                    if (Convert.ToInt32(await duplicateCheck.ExecuteScalarAsync(cancellationToken)) > 0)
                        throw new ArgumentException($"Employee Code '{ReadText(employee.Data, "employeecode")}' already exists.");

                    await using var command = connection.CreateCommand();
                    command.Transaction = transaction;
                    command.CommandText = @"DECLARE @nextId int = ISNULL((SELECT MAX(Id) FROM EREmployeeinfohdr WITH (UPDLOCK, HOLDLOCK)), 0) + 1;
INSERT INTO EREmployeeinfohdr (Id, clientid, entityid, machinecode, employeecode, firstname, lastname, cellno, depid, designationid, dateofbirth, genderid, joiningdate, employeetypeid, lvemployeecateg, coursecategoryid, empstatusid, utsysgen, utblock, utispartial, utaddtime, utadduser, utedittime, utedituser, utrecordversion)
VALUES (@nextId, @clientId, @entityId, @machineCode, @employeeCode, @firstName, @lastName, @cellNo, @departmentId, @designationId, @birthDate, @genderId, @joiningDate, @employeeTypeId, @employeeCategoryId, @teacherCategoryId, 1, 0, 0, 0, GETDATE(), 'v3-service', GETDATE(), 'v3-service', 0);";
                    AddParameter(command, "@clientId", clientId);
                    AddParameter(command, "@entityId", entityId);
                    _context.AuditImport(command);
                    AddParameter(command, "@machineCode", ReadText(employee.Data, "Machinecode"));
                    AddParameter(command, "@employeeCode", ReadText(employee.Data, "employeecode"));
                    AddParameter(command, "@firstName", ReadText(employee.Data, "firstname"));
                    AddParameter(command, "@lastName", ReadText(employee.Data, "lastname"));
                    AddParameter(command, "@cellNo", ReadText(employee.Data, "cellno"));
                    AddParameter(command, "@departmentId", employee.DepartmentId);
                    AddParameter(command, "@designationId", employee.DesignationId);
                    AddParameter(command, "@birthDate", employee.BirthDate);
                    AddParameter(command, "@genderId", employee.GenderId);
                    AddParameter(command, "@joiningDate", employee.JoiningDate);
                    AddParameter(command, "@employeeTypeId", employee.EmployeeTypeId);
                    AddParameter(command, "@employeeCategoryId", employee.EmployeeCategoryId);
                    AddParameter(command, "@teacherCategoryId", employee.TeacherCategoryId);
                    await command.ExecuteNonQueryAsync(cancellationToken);
                }
                await transaction.CommitAsync(cancellationToken);
                return mapped.Count;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        private async Task<int> CreateDepartmentAsync(DbConnection connection, DbTransaction transaction, int clientId, string departmentName, CancellationToken cancellationToken)
        {
            await using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = @"DECLARE @nextId int = ISNULL((SELECT MAX(Id) FROM OrgDepartment WITH (UPDLOCK, HOLDLOCK)), 0) + 1;
INSERT INTO OrgDepartment (Id, depcode, depstxt, clientid, utsysgen, utblock, utispartial, utaddtime, utadduser, utedittime, utedituser, utrecordversion)
VALUES (@nextId, CONCAT('DEP', @nextId), @departmentName, @clientId, 0, 0, 0, GETDATE(), 'v3-service', GETDATE(), 'v3-service', 0);
SELECT @nextId;";
            _context.AuditImport(command);
            AddParameter(command, "@departmentName", departmentName);
            AddParameter(command, "@clientId", clientId);
            return Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
        }

        private async Task<int> CreateDesignationAsync(DbConnection connection, DbTransaction transaction, int clientId, string designationName, CancellationToken cancellationToken)
        {
            await using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = @"DECLARE @nextId int = ISNULL((SELECT MAX(Id) FROM OrgDesignation WITH (UPDLOCK, HOLDLOCK)), 0) + 1;
DECLARE @codeSeed int = @nextId;
DECLARE @designationCode varchar(5) = CONCAT('D', RIGHT(CONCAT('0000', @codeSeed), 4));
WHILE EXISTS (SELECT 1 FROM OrgDesignation WHERE clientid = @clientId AND desgcode = @designationCode)
BEGIN
    SET @codeSeed = @codeSeed + 1;
    SET @designationCode = CONCAT('D', RIGHT(CONCAT('0000', @codeSeed), 4));
END;
INSERT INTO OrgDesignation (Id, desgcode, stxt, clientid, blank, utsysgen, utblock, utispartial, utaddtime, utadduser, utedittime, utedituser, utrecordversion)
VALUES (@nextId, @designationCode, @designationName, @clientId, 0, 0, 0, 0, GETDATE(), 'v3-service', GETDATE(), 'v3-service', 0);
SELECT @nextId;";
            _context.AuditImport(command);
            AddParameter(command, "@designationName", designationName);
            AddParameter(command, "@clientId", clientId);
            return Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
        }

        private static string ReadText(Dictionary<string, object?> row, string key) => row.FirstOrDefault(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase)).Value?.ToString()?.Trim() ?? string.Empty;
        private static bool Same(string? left, string right) => string.Equals(left?.Trim(), right.Trim(), StringComparison.OrdinalIgnoreCase);
        private static bool TryParseImportDate(string value, out DateTime date) =>
            DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)
            || DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out date)
            || DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out date);

        private async Task<bool> TableExistsAsync(string tableName, CancellationToken cancellationToken)
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(cancellationToken);
            }

            await using var command = connection.CreateCommand();
            command.CommandText = "SELECT CASE WHEN OBJECT_ID(@tableName, 'U') IS NULL THEN 0 ELSE 1 END";
            AddParameter(command, "@tableName", tableName);
            var result = await command.ExecuteScalarAsync(cancellationToken);
            return Convert.ToInt32(result) == 1;
        }

        private static void AddParameter(System.Data.Common.DbCommand command, string name, object? value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }

        private static ColumnSpec Col(string field, string header, string expression) => new(field, header, expression);

        private sealed record TabSpec(
            string Title,
            string TableName,
            IReadOnlyList<ColumnSpec> Columns,
            string FromClause,
            string WhereClause,
            string OrderBy);

        private sealed record ColumnSpec(string Field, string Header, string Expression);
        private sealed record EmployeeProfileImportRow(int RowNumber, Dictionary<string, object?> Data, int DepartmentId, int DesignationId, int GenderId, int EmployeeTypeId, int EmployeeCategoryId, int? TeacherCategoryId, DateTime JoiningDate, DateTime BirthDate);
    }
}
