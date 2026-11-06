using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Organization.Service.DTOs;
using Organization.Service.Services;

namespace Organization.Service.Controllers
{
    /// <summary>
    /// Serves the read-only institution-type terminology map (keyword swaps + per-language translations)
    /// used by the frontend to relabel the UI for the logged-in institution's type.
    /// Reachable via the gateway at /api/organization/InstitutionTerminology/v3.
    /// Anonymous: non-sensitive static config, fetched by the app shell (including the login screen).
    /// </summary>
    [ApiController]
    [Route("api/InstitutionTerminology/v3")]
    [AllowAnonymous]
    public sealed class InstitutionTerminologyV3Controller : ControllerBase
    {
        private readonly IInstitutionTerminologyService _terminology;
        private readonly ILogger<InstitutionTerminologyV3Controller> _logger;

        public InstitutionTerminologyV3Controller(
            IInstitutionTerminologyService terminology,
            ILogger<InstitutionTerminologyV3Controller> logger)
        {
            _terminology = terminology;
            _logger = logger;
        }

        /// <summary>
        /// Returns the full terminology map, or a single-type map when <paramref name="type"/> is supplied
        /// (e.g. ?type=Day Care). Unknown types return an empty list for that key.
        /// </summary>
        [HttpGet]
        public ActionResult<IReadOnlyDictionary<string, List<TerminologyMappingDto>>> GetAll([FromQuery] string? type = null)
        {
            try
            {
                var all = _terminology.GetAll();

                if (string.IsNullOrWhiteSpace(type))
                {
                    return Ok(all);
                }

                var key = InstitutionTerminologyService.NormalizeKey(type);
                var list = all.TryGetValue(key, out var mappings) ? mappings : new List<TerminologyMappingDto>();
                return Ok(new Dictionary<string, List<TerminologyMappingDto>> { [key] = list });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving institution terminology");
                return StatusCode(500, new { error = "An error occurred while retrieving institution terminology" });
            }
        }
    }
}
