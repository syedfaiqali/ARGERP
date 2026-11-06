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
    [Route("api/State/v3")]
    [Authorize]
    public sealed class StateV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<StateV3Controller> _logger;

        public StateV3Controller(IMediator mediator, ILogger<StateV3Controller> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<StateV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchTerm = null,
            [FromQuery] bool includeInactive = false,
            [FromQuery] int? countryId = null)
        {
            try
            {
                var query = new GetStatesV3Query
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving states (v3)");
                return StatusCode(500, new { error = "An error occurred while retrieving states" });
            }
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<StateV3Dto>> GetById(int id)
        {
            try
            {
                var result = await _mediator.Send(new GetStateByIdV3Query
                {
                    StateId = id,
                    ClientId = GetClientIdFromHeader(),
                });

                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = $"State with ID {id} not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving state (v3)");
                return StatusCode(500, new { error = "An error occurred while retrieving the state" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<StateV3Dto>> Create([FromBody] CreateStateV3Request request)
        {
            try
            {
                var command = new CreateStateV3Command
                {
                    CountryId = request.CountryId,
                    StateName = request.StateName,
                    StateCode = request.StateCode,
                    IsActive = request.IsActive,
                    CreatedBy = GetCurrentUserId(),
                    ClientId = GetClientIdFromHeader(),
                };

                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id = result.StateId }, result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid state creation request (v3)");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating state (v3)");
                return StatusCode(500, new { error = "An error occurred while creating the state" });
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<StateV3Dto>> Update(int id, [FromBody] UpdateStateV3Request request)
        {
            try
            {
                var command = new UpdateStateV3Command
                {
                    StateId = id,
                    CountryId = request.CountryId,
                    StateName = request.StateName,
                    StateCode = request.StateCode,
                    IsActive = request.IsActive,
                    UpdatedBy = GetCurrentUserId(),
                    ClientId = GetClientIdFromHeader(),
                };

                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = $"State with ID {id} not found" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid state update request (v3)");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating state (v3)");
                return StatusCode(500, new { error = "An error occurred while updating the state" });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            try
            {
                var result = await _mediator.Send(new DeleteStateV3Command
                {
                    StateId = id,
                    DeletedBy = GetCurrentUserId(),
                    ClientId = GetClientIdFromHeader(),
                });

                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { error = $"State with ID {id} not found" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cannot delete state (v3)");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting state (v3)");
                return StatusCode(500, new { error = "An error occurred while deleting the state" });
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
