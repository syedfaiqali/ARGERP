using HumanResource.Service.PayrollAccounting.Api;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HumanResource.Service.Controllers
{
    /// <summary>Payroll JV backlog (gateway: /api/HumanResource/JvBacklog/v3).</summary>
    [Route("api/JvBacklog/v3")]
    public sealed class JvBacklogV3Controller : JvBacklogControllerBase
    {
        public JvBacklogV3Controller(IMediator mediator, DbContext db) : base(mediator, db)
        {
        }
    }
}
