using HumanResource.Service.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Route("api/Lov/v3")]
    [Route("api/HumanResource/Lov/v3")]
    [Authorize]
    public class LovV3Controller : ControllerBase
    {
        private readonly HumanResourceDbContext _context;

        public LovV3Controller(HumanResourceDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetLovs([FromQuery] string lovType)
        {
            return Ok(await GetLovRows(lovType));
        }

        [HttpGet("{lovType}")]
        public async Task<IActionResult> GetLovsByPath(string lovType)
        {
            return Ok(await GetLovRows(lovType));
        }

        private async Task<object> GetLovRows(string lovType)
        {
            var lovs = await _context.Lovs
                .Where(x => x.Vtype == lovType && !x.UtBlock)
                .Select(x => new { x.Id, x.Code, Name = x.Stxt, LongName = x.Ltxt })
                .ToListAsync();
            return lovs;
        }
    }
}
