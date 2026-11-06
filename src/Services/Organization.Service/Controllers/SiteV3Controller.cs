using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Queries;
using System.Security.Claims;

namespace Organization.Service.Controllers
{
    [ApiController]
    [Route("api/Site/v3")]
    [Route("api/organization/Site/v3")]
    [Authorize]
    public sealed class SiteV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<SiteV3Controller> _logger;

        public SiteV3Controller(IMediator mediator, ILogger<SiteV3Controller> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<SiteV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] int? regionId = null)
        {
            try
            {
                var (clientId, entityId) = GetScope();
                var result = await _mediator.Send(new GetSitesV3Query
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SearchTerm = searchTerm,
                    RegionId = regionId,
                    ClientId = clientId,
                    EntityId = entityId,
                });
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sites (v3)");
                return StatusCode(500, new { error = "An error occurred while retrieving sites" });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SiteV3Dto>> GetById(int id)
        {
            try
            {
                var (clientId, entityId) = GetScope();
                var result = await _mediator.Send(new GetSiteByIdV3Query
                {
                    SiteId = id,
                    ClientId = clientId,
                    EntityId = entityId,
                });
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { error = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = $"Site with ID {id} not found" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving site (v3)");
                return StatusCode(500, new { error = "An error occurred while retrieving site" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<SiteV3Dto>> Create([FromBody] CreateSiteV3Request request)
        {
            try
            {
                var (clientId, entityId) = GetScope();
                var result = await _mediator.Send(new CreateSiteV3Command
                {
                    Code = request.Code,
                    SiteName = request.SiteName,
                    SiteTypeId = request.SiteTypeId,
                    SiteForId = request.SiteForId,
                    RegionId = request.RegionId,
                    ForId = request.ForId,
                    ClientId = clientId,
                    EntityId = entityId,
                    CreatedBy = GetCurrentUserId(),
                });

                return CreatedAtAction(nameof(GetById), new { id = result.SiteId }, result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                var message = GetInnermostMessage(ex);
                _logger.LogError(ex, "Database error creating site (v3): {Message}", message);
                return StatusCode(500, new { error = message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating site (v3)");
                return StatusCode(500, new { error = "An error occurred while creating site" });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<SiteV3Dto>> Update(int id, [FromBody] UpdateSiteV3Request request)
        {
            try
            {
                var (clientId, entityId) = GetScope();
                var result = await _mediator.Send(new UpdateSiteV3Command
                {
                    SiteId = id,
                    Code = request.Code,
                    SiteName = request.SiteName,
                    SiteTypeId = request.SiteTypeId,
                    SiteForId = request.SiteForId,
                    RegionId = request.RegionId,
                    ForId = request.ForId,
                    ClientId = clientId,
                    EntityId = entityId,
                    UpdatedBy = GetCurrentUserId(),
                });
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { error = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = $"Site with ID {id} not found" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                var message = GetInnermostMessage(ex);
                _logger.LogError(ex, "Database error updating site (v3): {Message}", message);
                return StatusCode(500, new { error = message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating site (v3)");
                return StatusCode(500, new { error = "An error occurred while updating site" });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            try
            {
                var (clientId, entityId) = GetScope();
                var result = await _mediator.Send(new DeleteSiteV3Command
                {
                    SiteId = id,
                    ClientId = clientId,
                    EntityId = entityId,
                    DeletedBy = GetCurrentUserId(),
                });
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { error = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = $"Site with ID {id} not found" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                var message = GetInnermostMessage(ex);
                _logger.LogError(ex, "Database error deleting site (v3): {Message}", message);
                return StatusCode(500, new { error = message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting site (v3)");
                return StatusCode(500, new { error = "An error occurred while deleting site" });
            }
        }

        [HttpGet("lookups/site-types")]
        public async Task<ActionResult<List<SiteLookupDto>>> GetSiteTypes()
        {
            try
            {
                var result = await _mediator.Send(new GetSiteTypesV3Query());
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving site types (v3)");
                return StatusCode(500, new { error = "An error occurred while retrieving site types" });
            }
        }

        [HttpGet("lookups/site-for")]
        public async Task<ActionResult<List<SiteLookupDto>>> GetSiteFor()
        {
            try
            {
                var result = await _mediator.Send(new GetSiteForOptionsV3Query());
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving site for options (v3)");
                return StatusCode(500, new { error = "An error occurred while retrieving site for options" });
            }
        }

        [HttpGet("lookups/parent-sites")]
        public async Task<ActionResult<List<SiteLookupDto>>> GetParentSites()
        {
            try
            {
                var (clientId, entityId) = GetScope();
                var result = await _mediator.Send(new GetSiteParentOptionsV3Query
                {
                    ClientId = clientId,
                    EntityId = entityId,
                });
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving parent sites (v3)");
                return StatusCode(500, new { error = "An error occurred while retrieving parent sites" });
            }
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("userId")?.Value
                ?? User.FindFirst("sub")?.Value;

            return int.TryParse(claim, out var userId) ? userId : 0;
        }

        private static string GetInnermostMessage(Exception exception)
        {
            var current = exception;
            while (current.InnerException != null)
                current = current.InnerException;

            return string.IsNullOrWhiteSpace(current.Message)
                ? "A database error occurred while saving site"
                : current.Message;
        }

        private (int ClientId, int EntityId) GetScope()
        {
            var entityId = RequestScopeHelper.GetOptionalEntityId(this);
            if (!entityId.HasValue)
                throw new UnauthorizedAccessException("Authenticated user does not contain a valid entity scope");

            return (RequestScopeHelper.GetClientId(this), entityId.Value);
        }
    }
}
