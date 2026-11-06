using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organization.Service.Commands;
using Organization.Service.DTOs;
using Organization.Service.Queries;
using System.Security.Claims;

namespace Organization.Service.Controllers
{
    [ApiController]
    [Route("api/City/v3")]
    [Authorize]
    public sealed class CityV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CityV3Controller> _logger;

        public CityV3Controller(IMediator mediator, ILogger<CityV3Controller> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<CityV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool includeInactive = false,
            [FromQuery] int? countryId = null,
            [FromQuery] int? stateId = null)
        {
            try
            {
                var query = new GetCitiesV3Query
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SearchTerm = searchTerm,
                    IncludeInactive = includeInactive,
                    CountryId = countryId,
                    StateId = stateId,
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
                _logger.LogError(ex, "Error retrieving cities (v3)");
                return StatusCode(500, new { error = "An error occurred while retrieving cities" });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CityV3Dto>> GetById(int id)
        {
            try
            {
                var result = await _mediator.Send(new GetCityByIdV3Query
                {
                    CityId = id,
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
                return NotFound(new { error = $"City with ID {id} not found" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving city (v3)");
                return StatusCode(500, new { error = "An error occurred while retrieving the city" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<CityV3Dto>> Create([FromBody] CreateCityV3Request request)
        {
            try
            {
                var command = new CreateCityV3Command
                {
                    CityCode = request.CityCode,
                    CityName = request.CityName,
                    DialCode = request.DialCode,
                    CountryId = request.CountryId,
                    StateId = request.StateId,
                    IsActive = request.IsActive,
                    CreatedBy = GetCurrentUserId(),
                    ClientId = GetClientIdFromHeader(),
                };

                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id = result.CityId }, result);
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
                _logger.LogWarning(ex, "Invalid city creation request (v3)");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating city (v3)");
                return StatusCode(500, new { error = "An error occurred while creating the city" });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CityV3Dto>> Update(int id, [FromBody] UpdateCityV3Request request)
        {
            try
            {
                var command = new UpdateCityV3Command
                {
                    CityId = id,
                    CityCode = request.CityCode,
                    CityName = request.CityName,
                    DialCode = request.DialCode,
                    CountryId = request.CountryId,
                    StateId = request.StateId,
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
                return NotFound(new { error = $"City with ID {id} not found" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid city update request (v3)");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating city (v3)");
                return StatusCode(500, new { error = "An error occurred while updating the city" });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            try
            {
                var result = await _mediator.Send(new DeleteCityV3Command
                {
                    CityId = id,
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
                return NotFound(new { error = $"City with ID {id} not found" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cannot delete city (v3)");
                return BadRequest(new { error = ex.Message });
            }
            catch (DbUpdateException ex)
            {
                var message = ex.GetBaseException().Message;
                _logger.LogError(ex, "Database error deleting city (v3). BaseMessage={BaseMessage}", message);
                return Conflict(new { error = message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting city (v3)");
                return StatusCode(500, new { error = "An error occurred while deleting the city" });
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
