using MediatR;
using MediClaim.Application
    .Features.Claims.CreateDraftClaim;
using MediClaim.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediClaim.API.Controllers;

[ApiController]
[Route("api/claims")]
[Authorize]
public class ClaimsController
    : ControllerBase
{
    private readonly IMediator
        _mediator;

    public ClaimsController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("draft")]
    [Authorize(
        Roles =
            nameof(UserRole.Patient))]
    public async Task<IActionResult>
        CreateDraft(
            CreateDraftClaimCommand command)
    {
        var claimId =
            await _mediator
                .Send(command);

        return Ok(claimId);
    }
}