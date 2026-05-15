using MediatR;
using MediClaim.Application.Features.Reports.AuditExport;
using MediClaim.Application.Features.Reports.ClaimSummary;
using MediClaim.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace MediClaim.API.Controllers
{

    [ApiController]
    [Route("api/reports")]
    [Authorize]
    public class ReportController : ControllerBase
    {
        private readonly IMediator _mediator;
        public ReportController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("claims-summary")]
        [Authorize(Roles = nameof(UserRole.TenantAdmin))]
        public async Task<IActionResult> GetClaimsSummary
            (GetClaimsSummaryQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpGet("officers-workload")]
        [Authorize(Roles = nameof(UserRole.TenantAdmin))]
        public async Task<IActionResult> GetOfficersWorkload
           (GetClaimsSummaryQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        [HttpGet("audit-export")]
        [Authorize(Roles =nameof(UserRole.TenantAdmin))]
        public async Task<IActionResult> AuditExport(
           [FromQuery] DateTime from,
           [FromQuery] DateTime to)
        {
            var file = await _mediator.Send(
                    new ExportAuditLogsQuery
                    {
                        From = from,
                        To = to
                    });
            return File(file, "text/csv", "audit-logs.csv");
        }
    }
}
