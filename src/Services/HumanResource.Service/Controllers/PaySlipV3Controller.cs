using HumanResource.Service.DTOs;
using HumanResource.Service.Handlers.Commands;
using HumanResource.Service.Handlers.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Route("api/PaySlip/v3")]
    [Authorize]
    public class PaySlipV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;

        public PaySlipV3Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("fetch")]
        public async Task<ActionResult<List<PaySlipV3Dto>>> FetchPaySlipData([FromBody] PaySlipFetchRequestV3 request)
        {
            var clientId = GetClientId();
            var entityId = GetEntityId();

            if (entityId == null)
                return BadRequest("Missing or invalid EntityId.");

            var result = await _mediator.Send(new GetPaySlipProcessQuery(request, clientId, entityId.Value));
            return Ok(result);
        }

        [HttpPost("process")]
        public async Task<ActionResult<PaySlipV3Dto>> ProcessPaySlip([FromBody] PaySlipV3Dto request)
        {
            var clientId = GetClientId();
            var entityId = GetEntityId();

            if (entityId == null)
                return BadRequest("Missing or invalid EntityId.");

            var result = await _mediator.Send(new SavePaySlipProcessCommand(request, clientId, entityId.Value));
            return Ok(result);
        }

        private int GetClientId()
        {
            var claim = User.Claims.FirstOrDefault(c => 
                c.Type.Equals("ClientId", StringComparison.OrdinalIgnoreCase) || 
                c.Type.Equals("CID", StringComparison.OrdinalIgnoreCase))?.Value;
            
            if (!string.IsNullOrEmpty(claim) && int.TryParse(claim, out var cid)) return cid;
            
            foreach (var header in Request.Headers)
            {
                if (header.Key.Equals("X-Client-Id", StringComparison.OrdinalIgnoreCase) ||
                    header.Key.Equals("Client-Id", StringComparison.OrdinalIgnoreCase) ||
                    header.Key.Equals("CID", StringComparison.OrdinalIgnoreCase))
                {
                    var val = header.Value.ToString().Split(',')[0].Trim();
                    if (int.TryParse(val, out var id)) return id;
                }
            }

            throw new InvalidOperationException("ClientId not found in token or headers");
        }

        private int? GetEntityId()
        {
            var claim = User.Claims.FirstOrDefault(c => 
                c.Type.Equals("EntityId", StringComparison.OrdinalIgnoreCase) || 
                c.Type.Equals("EID", StringComparison.OrdinalIgnoreCase) ||
                c.Type.Equals("SelectedEntity", StringComparison.OrdinalIgnoreCase))?.Value;

            if (!string.IsNullOrEmpty(claim) && int.TryParse(claim, out var eid)) return eid;

            foreach (var header in Request.Headers)
            {
                var key = header.Key;
                if (key.Equals("X-Entity-Id", StringComparison.OrdinalIgnoreCase) ||
                    key.Equals("Entity-Id", StringComparison.OrdinalIgnoreCase) ||
                    key.Equals("X-Selected-Entity", StringComparison.OrdinalIgnoreCase) ||
                    key.Equals("SelectedEntity", StringComparison.OrdinalIgnoreCase) ||
                    key.Equals("EID", StringComparison.OrdinalIgnoreCase) ||
                    key.Equals("OrganizationId", StringComparison.OrdinalIgnoreCase) ||
                    key.Equals("X-Organization-Id", StringComparison.OrdinalIgnoreCase) ||
                    key.Equals("X-Campus-Id", StringComparison.OrdinalIgnoreCase) ||
                    key.Equals("Campus-Id", StringComparison.OrdinalIgnoreCase))
                {
                    var val = header.Value.ToString().Split(',')[0].Trim();
                    if (int.TryParse(val, out var id)) return id;
                }
            }

            return null;
        }
    }
}
