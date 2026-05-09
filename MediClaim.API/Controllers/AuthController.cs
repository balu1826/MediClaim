using MediatR;
using MediClaim.Application
    .Features.Auth.Commands.RegisterTenant;
using Microsoft.AspNetCore.Mvc;

namespace MediClaim.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("registerTenant")]
    public async Task<IActionResult>
        RegisterTenant(
            RegisterTenantCommand command)
    {
        var tenantId =
            await _mediator.Send(command);

        return Ok(new
        {
            TenantId = tenantId
        });
    }
}