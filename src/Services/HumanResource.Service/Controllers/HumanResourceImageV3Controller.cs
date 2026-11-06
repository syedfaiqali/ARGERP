using HumanResource.Service.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/v3/image")]
    [Route("api/HumanResource/v3/image")]
    public sealed class HumanResourceImageV3Controller : ControllerBase
    {
        private readonly HumanResourceDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<HumanResourceImageV3Controller> _logger;

        public HumanResourceImageV3Controller(
            HumanResourceDbContext context,
            IWebHostEnvironment environment,
            ILogger<HumanResourceImageV3Controller> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<ActionResult<long>> Upload(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var uploadsPath = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads");
            Directory.CreateDirectory(uploadsPath);

            var filename = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsPath, filename);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;
            if (shouldClose)
            {
                await connection.OpenAsync(cancellationToken);
            }

            try
            {
                await using var command = connection.CreateCommand();
                command.CommandText =
                    """
                    declare @nextId bigint = isnull((select max(id) from GNImage), 0) + 1;
                    insert into GNImage (id, filename, logdate) values (@nextId, @filename, getdate());
                    select @nextId;
                    """;

                var idParameter = command.CreateParameter();
                idParameter.ParameterName = "@filename";
                idParameter.Value = filename;
                command.Parameters.Add(idParameter);

                var result = await command.ExecuteScalarAsync(cancellationToken);
                return Ok(Convert.ToInt64(result));
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }
        }

        [HttpGet("{id:long}")]
        [AllowAnonymous]
        public async Task<ActionResult> Get(long id, CancellationToken cancellationToken)
        {
            var filename = await GetFilename(id, cancellationToken);
            if (string.IsNullOrWhiteSpace(filename))
            {
                return NotFound("Image not found.");
            }

            var filePath = ResolveUploadPath(filename);
            if (filePath == null)
            {
                _logger.LogWarning("GNImage file not found for image id {ImageId} and filename {Filename}", id, filename);
                return NotFound("Image file not found.");
            }

            var bytes = await System.IO.File.ReadAllBytesAsync(filePath, cancellationToken);
            return File(bytes, GetContentType(filename));
        }

        private async Task<string?> GetFilename(long id, CancellationToken cancellationToken)
        {
            var connection = _context.Database.GetDbConnection();
            var shouldClose = connection.State != ConnectionState.Open;
            if (shouldClose)
            {
                await connection.OpenAsync(cancellationToken);
            }

            try
            {
                await using var command = connection.CreateCommand();
                command.CommandText = "select filename from GNImage where id = @id";
                var parameter = command.CreateParameter();
                parameter.ParameterName = "@id";
                parameter.Value = id;
                command.Parameters.Add(parameter);

                var result = await command.ExecuteScalarAsync(cancellationToken);
                return result == DBNull.Value ? null : Convert.ToString(result);
            }
            finally
            {
                if (shouldClose)
                {
                    await connection.CloseAsync();
                }
            }
        }

        private string? ResolveUploadPath(string filename)
        {
            var hrPath = Path.Combine(_environment.ContentRootPath, "wwwroot", "uploads", filename);
            if (System.IO.File.Exists(hrPath)) return hrPath;

            var siblingStudentPath = Path.GetFullPath(Path.Combine(
                _environment.ContentRootPath,
                "..",
                "Student.Service",
                "wwwroot",
                "uploads",
                filename));
            return System.IO.File.Exists(siblingStudentPath) ? siblingStudentPath : null;
        }

        private static string GetContentType(string filename)
            => Path.GetExtension(filename).ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".svg" => "image/svg+xml",
                _ => "application/octet-stream"
            };
    }
}
