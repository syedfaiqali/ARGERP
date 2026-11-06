using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Organization.Service.DTOs;
using Organization.Service.Queries;

namespace Organization.Service.Controllers
{
    [ApiController]
    [Route("api/OrgLanguage/v3")]
    [Authorize]
    public sealed class OrgLanguageV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrgLanguageV3Controller> _logger;

        public OrgLanguageV3Controller(IMediator mediator, ILogger<OrgLanguageV3Controller> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrgLanguageDto>>> GetAll([FromQuery] bool includeInactive = false)
        {
            try
            {
                var result = await _mediator.Send(new GetAllLanguagesQuery
                {
                    IncludeInactive = includeInactive
                });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving organization languages");
                return StatusCode(500, new { error = "An error occurred while retrieving organization languages" });
            }
        }
    }
}
