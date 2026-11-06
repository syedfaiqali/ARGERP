using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;
using Organization.Service.Infrastructure.Data;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Queries;

namespace Organization.Service.Controllers
{
    [ApiController]
    [Route("api/Institution/v3")]
    [Authorize]
    public sealed class InstitutionV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<InstitutionV3Controller> _logger;
        private readonly OrganizationDbContext _context;
        private readonly AuthorizationScopeDbContext _authorization;

        public InstitutionV3Controller(
            IMediator mediator,
            ILogger<InstitutionV3Controller> logger,
            OrganizationDbContext context,
            AuthorizationScopeDbContext authorization)
        {
            _mediator = mediator;
            _logger = logger;
            _context = context;
            _authorization = authorization;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<InstitutionV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool lightweight = false)
        {
            try
            {
                var clientId = GetClientId();
                var result = await _mediator.Send(new GetInstitutionsV3Query(pageNumber, pageSize, searchTerm, clientId, lightweight));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }


        [HttpGet("assigned-entity-ids")]
        public async Task<ActionResult<List<int>>> GetAssignedEntityIds(CancellationToken cancellationToken)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue("userId")
                ?? User.FindFirstValue("userid")
                ?? User.FindFirstValue("sub");
            if (!int.TryParse(userIdValue, out var userId) || userId <= 0)
                return Unauthorized(new { error = "User ID not found in token." });

            var entityIds = await _context.OrgLegalentityLegacy.AsNoTracking().Where(x => x.clientid == GetClientId()).Select(x => x.Id).ToListAsync(cancellationToken);
            return Ok(entityIds);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<InstitutionV3Dto>> GetById(int id)
        {
            try
            {
                var clientId = GetClientId();
                var result = await _mediator.Send(new GetInstitutionByIdV3Query(id, clientId));
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPost]
        public async Task<ActionResult<InstitutionV3Dto>> Create([FromBody] SaveInstitutionV3Request request)
        {
            try
            {
                var clientId = GetClientId();
                var result = await _mediator.Send(new CreateInstitutionV3Command(request, clientId));
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] SaveInstitutionV3Request request)
        {
            try
            {
                if (id != request.Id) return BadRequest("ID mismatch");
                var clientId = GetClientId();
                var result = await _mediator.Send(new UpdateInstitutionV3Command(request, clientId));
                if (!result) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var clientId = GetClientId();
                var result = await _mediator.Send(new DeleteInstitutionV3Command(id, clientId));
                if (!result) return NotFound();
                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("lookups")]
        public async Task<ActionResult<InstitutionLookupsV3Dto>> GetLookups([FromQuery] int? countryId = null, [FromQuery] int? stateId = null)
        {
            try
            {
                var clientId = GetClientId();
                var result = await _mediator.Send(new GetInstitutionLookupsV3Query(clientId, countryId, stateId));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        [HttpGet("entities")]
        public async Task<ActionResult<List<ApplicationEntityV3Dto>>> GetEntities(
            [FromQuery] int? clientId = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var effectiveClientId = clientId ?? GetClientId();
                var result = await _context.OrgLegalentityLegacy
                    .AsNoTracking()
                    .Where(x => x.clientid == effectiveClientId)
                    .OrderBy(x => x.stxt)
                    .Select(x => new ApplicationEntityV3Dto
                    {
                        Id = x.Id,
                        EntityId = x.Id,
                        Code = x.entityno ?? string.Empty,
                        EntityName = x.stxt ?? string.Empty
                    })
                    .ToListAsync(cancellationToken);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        private int GetClientId()
        {
            return RequestScopeHelper.GetClientId(this);
        }

        private ActionResult HandleException(Exception ex)
        {
            if (ex is DbUpdateException dbUpdateEx)
            {
                var dbSqlEx = dbUpdateEx.GetBaseException() as SqlException;
                var message = dbUpdateEx.GetBaseException().Message;

                _logger.LogError(dbUpdateEx,
                    "InstitutionV3 database update error. BaseMessage={BaseMessage}",
                    message);

                if (dbSqlEx != null)
                {
                    if (dbSqlEx.Number == 547)
                    {
                        var tableMatch = Regex.Match(message, "table \\\"(?<table>[^\\\"]+)\\\"", RegexOptions.IgnoreCase);
                        var usedIn = tableMatch.Success ? tableMatch.Groups["table"].Value : "another record";
                        return BadRequest(new
                        {
                            error = $"Institution cannot be deleted because it is used in {usedIn}."
                        });
                    }

                    return StatusCode(500, new
                    {
                        error = message,
                        sqlNumber = dbSqlEx.Number,
                        procedure = dbSqlEx.Procedure,
                        lineNumber = dbSqlEx.LineNumber
                    });
                }

                return StatusCode(500, new { error = message });
            }

            if (ex is SqlException sqlEx)
            {
                _logger.LogError(sqlEx,
                    "InstitutionV3 SQL error. Number={Number}, Procedure={Procedure}, Line={LineNumber}, Message={Message}",
                    sqlEx.Number, sqlEx.Procedure, sqlEx.LineNumber, sqlEx.Message);

                return StatusCode(500, new
                {
                    error = sqlEx.Message,
                    sqlNumber = sqlEx.Number,
                    procedure = sqlEx.Procedure,
                    lineNumber = sqlEx.LineNumber
                });
            }

            _logger.LogError(ex, "InstitutionV3 unexpected error");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    public sealed class ApplicationEntityV3Dto
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
    }
}
