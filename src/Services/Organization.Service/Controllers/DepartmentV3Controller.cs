using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Organization.Service.Commands;
using Organization.Service.DTOs;

using Organization.Service.Queries;

namespace Organization.Service.Controllers
{
    [ApiController]
    [Route("api/Department/v3")]
    [Authorize]
    public sealed class DepartmentV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DepartmentV3Controller> _logger;

        public DepartmentV3Controller(IMediator mediator, ILogger<DepartmentV3Controller> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<DepartmentV3Dto>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null)
        {
            var clientId = GetClientId();
            var query = new GetDepartmentsV3Query(pageNumber, pageSize, searchTerm, clientId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentV3Dto>> GetById(int id)
        {
            var clientId = GetClientId();
            var result = await _mediator.Send(new GetDepartmentByIdV3Query(id, clientId));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<DepartmentV3Dto>> Create([FromBody] CreateDepartmentV3Request request)
        {
            try
            {
                var clientId = GetClientId();
                var command = new CreateDepartmentV3Command(request, clientId);
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid department creation request");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating department");
                return StatusCode(500, new { error = "An error occurred while creating department" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateDepartmentV3Request request)
        {
            try
            {
                if (id != request.Id) return BadRequest(new { error = "ID mismatch" });
                var clientId = GetClientId();
                var result = await _mediator.Send(new UpdateDepartmentV3Command(request, clientId));
                if (!result) return NotFound(new { error = "Department not found" });
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid department update request");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating department");
                return StatusCode(500, new { error = "An error occurred while updating department" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var clientId = GetClientId();
                var result = await _mediator.Send(new DeleteDepartmentV3Command(id, clientId), HttpContext.RequestAborted);
                if (!result.Found) return NotFound(new { error = "Department not found" });
                if (!result.Deleted) return BadRequest(new { error = result.Message("Department"), usedIn = result.UsedIn });
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting department");
                return StatusCode(500, new { error = "An error occurred while deleting department" });
            }
        }

        [HttpGet("ficoa-lookup")]
        public async Task<ActionResult<IEnumerable<FicoaV3Dto>>> GetFicoaLookup([FromQuery] string? searchTerm = null)
        {
            var clientId = GetClientId();
            var result = await _mediator.Send(new GetFicoasV3Query(searchTerm, clientId));
            return Ok(result);
        }

        private int GetClientId()
        {
            return RequestScopeHelper.GetClientId(this);
        }
    }
}
