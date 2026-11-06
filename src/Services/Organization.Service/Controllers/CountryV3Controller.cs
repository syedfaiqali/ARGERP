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
    [Route("api/Country/v3")]
    [Authorize]
    //[AllowAnonymous]
    public class CountryV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CountryV3Controller> _logger;

        public CountryV3Controller(IMediator mediator, ILogger<CountryV3Controller> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<CountryV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] bool includeInactive = false)
        {
            try
            {
                var clientId = GetClientIdFromHeader();
                var query = new GetCountriesV3Query
                {
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SearchTerm = searchTerm,
                    IncludeInactive = includeInactive,
                    ClientId = clientId
                };

                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving countries (v3)");
                // Return detailed error in dev to help debugging
                return StatusCode(500, new { 
                    error = "An error occurred while retrieving countries",
                    detail = ex.Message,
                    stack = ex.StackTrace 
                });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CountryV3Dto>> GetById(int id)
        {
            try
            {
                var result = await _mediator.Send(new GetCountryByIdV3Query
                {
                    CountryId = id,
                    ClientId = GetClientIdFromHeader()
                });

                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = $"Country with ID {id} not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving country (v3)");
                return StatusCode(500, new { error = "An error occurred while retrieving the country" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<CountryV3Dto>> Create([FromBody] CreateCountryV3Request request)
        {
            try
            {
                var command = new CreateCountryV3Command
                {
                    CountryCode = request.CountryCode,
                    CountryName = request.CountryName,
                    IsoCode = request.IsoCode,
                    DialCode = request.DialCode,
                    CreatedBy = GetCurrentUserId(),
                    ClientId = GetClientIdFromHeader()
                };

                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id = result.CountryId }, result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid country creation request (v3)");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating country (v3)");
                return StatusCode(500, new { error = "An error occurred while creating the country" });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CountryV3Dto>> Update(int id, [FromBody] UpdateCountryV3Request request)
        {
            try
            {
                var command = new UpdateCountryV3Command
                {
                    CountryId = id,
                    CountryCode = request.CountryCode,
                    CountryName = request.CountryName,
                    IsoCode = request.IsoCode,
                    DialCode = request.DialCode,
                    UpdatedBy = GetCurrentUserId(),
                    ClientId = GetClientIdFromHeader()
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = $"Country with ID {id} not found" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid country update request (v3)");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating country (v3)");
                return StatusCode(500, new { error = "An error occurred while updating the country" });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            try
            {
                var result = await _mediator.Send(new DeleteCountryV3Command
                {
                    CountryId = id,
                    DeletedBy = GetCurrentUserId(),
                    ClientId = GetClientIdFromHeader()
                });

                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = $"Country with ID {id} not found" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cannot delete country (v3)");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting country (v3)");
                return StatusCode(500, new { error = "An error occurred while deleting the country" });
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
