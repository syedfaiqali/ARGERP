using HumanResource.Service.Commands;
using HumanResource.Service.DTOs;
using HumanResource.Service.Infrastructure.Data;
using HumanResource.Service.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace HumanResource.Service.Controllers
{
    [ApiController]
    [Route("api/PayrollPeriod/v3")]
    [Route("api/HumanResource/PayrollPeriod/v3")]
    [Authorize]
    public class PayrollPeriodV3Controller : ControllerBase
    {
        private readonly IMediator _mediator;

        public PayrollPeriodV3Controller(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PaginationResponse<PayrollPeriodV3Dto>>> Get([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 100, [FromQuery] string? searchTerm = null)
        {
            var query = new GetPayrollPeriodsV3Query(GetClientId(), GetEntityId())
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SearchTerm = searchTerm
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        private int GetClientId()
        {
            if (int.TryParse(Request.Headers["X-Client-Id"], out int clientId))
                return clientId;

            var clientIdClaim = User.Claims.FirstOrDefault(c => c.Type == "ClientId");
            if (clientIdClaim != null && int.TryParse(clientIdClaim.Value, out int claimClientId))
                return claimClientId;
            return 0;
        }

        private int? GetEntityId()
        {
            var entityIdClaim = User.Claims.FirstOrDefault(c => c.Type == "EntityId");
            if (entityIdClaim != null && int.TryParse(entityIdClaim.Value, out int claimEntityId))
                return claimEntityId;
            if (int.TryParse(Request.Headers["X-Entity-Id"], out int entityId))
                return entityId;
            return null;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PayrollPeriodHeaderV3Dto>> GetById(int id)
        {
            var result = await _mediator.Send(new GetPayrollPeriodByIdV3Query(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("{id}/details")]
        public async Task<ActionResult<List<PayrollPeriodDetailV3Dto>>> GetDetails(int id)
        {
            var result = await _mediator.Send(new GetPayrollPeriodDetailsV3Query(id));
            return Ok(result);
        }

        [HttpGet("generate")]
        public async Task<ActionResult<List<PayrollPeriodDetailV3Dto>>> Generate([FromQuery] int atnTypeId, [FromQuery] DateTime fromDate, [FromQuery] int noOfPeriod)
        {
            var result = await _mediator.Send(new GeneratePayrollPeriodDetailsV3Query(atnTypeId, fromDate, noOfPeriod));
            return Ok(result);
        }

        [HttpGet("next-register-date/{payrollRegisterId}")]
        public async Task<ActionResult<NextPayrollRegisterDateV3Dto?>> GetNextRegisterDate(int payrollRegisterId)
        {
            var result = await _mediator.Send(new GetNextPayrollRegisterDateV3Query(payrollRegisterId));
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<int>> Create([FromHeader(Name = "X-Client-Id")] int clientId, [FromQuery] int? entityId, [FromBody] CreatePayrollPeriodV3Request request)
        {
            var id = await _mediator.Send(new CreatePayrollPeriodV3Command(clientId, entityId, request));
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromHeader(Name = "X-Client-Id")] int clientId, [FromQuery] int? entityId, [FromBody] UpdatePayrollPeriodV3Request request)
        {
            if (id != request.Id) return BadRequest();
            var success = await _mediator.Send(new UpdatePayrollPeriodV3Command(clientId, entityId, request));
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var success = await _mediator.Send(new DeletePayrollPeriodV3Command(id));
            if (!success) return NotFound();
            return NoContent();
        }
    }
}
