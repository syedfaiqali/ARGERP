using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Route("api/PayrollRegister/v3")]
    [Authorize]
    public class PayrollRegisterV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;

        public PayrollRegisterV3Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PayrollRegisterV3Dto>>> Get([FromQuery] string? searchTerm = null)
        {
            var result = await _mediator.Send(new GetPayrollRegistersV3Query(GetClientId(), searchTerm));
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PayrollRegisterV3Dto>> GetById(int id)
        {
            var result = await _mediator.Send(new GetPayrollRegisterByIdV3Query(id, GetClientId()));
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<PayrollRegisterV3Dto>> Create([FromBody] CreatePayrollRegisterV3Request request)
        {
            try
            {
                var result = await _mediator.Send(new CreatePayrollRegisterV3Command(request, GetClientId()));
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePayrollRegisterV3Request request)
        {
            if (id != request.Id)
            {
                return BadRequest("ID mismatch");
            }

            try
            {
                var updated = await _mediator.Send(new UpdatePayrollRegisterV3Command(request, GetClientId()));
                if (!updated)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _mediator.Send(new DeletePayrollRegisterV3Command(id, GetClientId()));
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        private int GetClientId()
        {
            var clientIdClaim = User.Claims.FirstOrDefault(c => c.Type == "ClientId");
            if (clientIdClaim != null && int.TryParse(clientIdClaim.Value, out var claimClientId))
            {
                return claimClientId;
            }

            if (Request.Headers.TryGetValue("X-Client-Id", out var headerClientId)
                && int.TryParse(headerClientId, out var parsedHeaderClientId))
            {
                return parsedHeaderClientId;
            }

            throw new InvalidOperationException("ClientId not found in token or headers");
        }
    }
}
