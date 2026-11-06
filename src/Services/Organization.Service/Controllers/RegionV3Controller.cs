using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Queries;
using System.Security.Claims;

namespace Organization.Service.Controllers
{
    [ApiController]
    [Route("api/Region/v3")]
    [Authorize]
    public sealed class RegionV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<RegionV3Controller> _logger;

        public RegionV3Controller(IMediator mediator, ILogger<RegionV3Controller> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<RegionV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchTerm = null,
            [FromQuery] bool includeInactive = false,
            [FromQuery] int? countryId = null)
        {
            try
            {
                var query = new GetRegionsV3Query
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SearchTerm = searchTerm,
                    IncludeInactive = includeInactive,
                    CountryId = countryId,
                    ClientId = GetClientIdFromHeader(),
                };

                var result = await _mediator.Send(query);
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
                _logger.LogError(ex, "Error retrieving regions (v3)");
                return StatusCode(500, new { error = "An error occurred while retrieving regions" });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<RegionV3Dto>> GetById(int id)
        {
            try
            {
                var result = await _mediator.Send(new GetRegionByIdV3Query
                {
                    RegionId = id,
                    ClientId = GetClientIdFromHeader(),
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { error = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = $"Region with ID {id} not found" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving region (v3)");
                return StatusCode(500, new { error = "An error occurred while retrieving the region" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<RegionV3Dto>> Create([FromBody] CreateRegionV3Request request)
        {
            try
            {
                var command = new CreateRegionV3Command
                {
                    RegionCode = request.RegionCode,
                    RegionName = request.RegionName,
                    CountryId = request.CountryId,
                    IsActive = request.IsActive,
                    CreatedBy = GetCurrentUserId(),
                    ClientId = GetClientIdFromHeader(),
                };

                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id = result.RegionId }, result);
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
                _logger.LogWarning(ex, "Invalid region creation request (v3)");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating region (v3)");
                return StatusCode(500, new { error = "An error occurred while creating the region" });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "PlatformAdmin")]
        public async Task<ActionResult<RegionV3Dto>> Update(int id, [FromBody] UpdateRegionV3Request request)
        {
            try
            {
                var command = new UpdateRegionV3Command
                {
                    RegionId = id,
                    RegionCode = request.RegionCode,
                    RegionName = request.RegionName,
                    CountryId = request.CountryId,
                    IsActive = request.IsActive,
                    UpdatedBy = GetCurrentUserId(),
                    ClientId = GetClientIdFromHeader(),
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { error = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = $"Region with ID {id} not found" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid region update request (v3)");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating region (v3)");
                return StatusCode(500, new { error = "An error occurred while updating the region" });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            try
            {
                var result = await _mediator.Send(new DeleteRegionV3Command
                {
                    RegionId = id,
                    DeletedBy = GetCurrentUserId(),
                    ClientId = GetClientIdFromHeader(),
                });

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { error = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = $"Region with ID {id} not found" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cannot delete region (v3)");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting region (v3)");
                return StatusCode(500, new { error = "An error occurred while deleting the region" });
            }
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("userId")?.Value
                ?? User.FindFirst("sub")?.Value;

            return int.TryParse(claim, out var userId) ? userId : 0;
        }

        private int? GetClientIdFromHeader()
        {
            return RequestScopeHelper.GetClientId(this);
        }
    }
}
