using MediatR;
using MediClaim.Application.Features.Claims.ApproveClaim;
using MediClaim.Application
    .Features.Claims.CreateDraftClaim;
using MediClaim.Application.Features.Claims.EscalateClaim;
using MediClaim.Application.Features.Claims.GetFraudFlags;
using MediClaim.Application.Features.Claims.GetMyClaims;
using MediClaim.Application.Features.Claims.GetOfficerQueue;
using MediClaim.Application.Features.Claims.RejectClaim;
using MediClaim.Application.Features.Claims.SettleClaim;
using MediClaim.Application.Features.Claims.StartReview;
using MediClaim.Application.Features.Claims.SubmitClaim;
using MediClaim.Application.Features.Claims.TransitionClaim;
using MediClaim.Application.Features.Claims.UploadDocument;
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
    [HttpGet("my")]
    [Authorize(
    Roles =
        nameof(UserRole.Patient))]
    public async Task<IActionResult>
    GetMyClaims()
    {
        var claims =
            await _mediator.Send(
                new GetMyClaimsQuery());

        return Ok(claims);
    }
    [HttpPost("{claimId}/submit")]
    [Authorize(Roles = nameof(UserRole.Patient))]
    public async Task<IActionResult> Submit(
        Guid claimId)
    {
        await _mediator.Send(
            new SubmitClaimCommand
            {
                ClaimId = claimId
            });

        return NoContent();
    }
    [HttpGet("officer-queue")]
    [Authorize(Roles = nameof(UserRole.ClaimsOfficer))]
    public async Task<IActionResult> GetOfficerQueue()
    {
        var claims =
            await _mediator.Send(
                new GetOfficerQueueQuery());

        return Ok(claims);
    }
    [HttpPatch("{claimId}/transition")]
    [Authorize(Roles = nameof(UserRole.ClaimsOfficer))]
    public async Task<IActionResult> Transition(
         Guid claimId,
         TransitionClaimCommand command)
    {
        command.ClaimId = claimId;
        await _mediator.Send(command);
        return NoContent();
    }
    [HttpPost("{claimId}/approve")]
    [Authorize(Roles = nameof(UserRole.ClaimsOfficer))]
    public async Task<IActionResult> Approve(
        Guid claimId)
    {
        await _mediator.Send(
            new ApproveClaimCommand
            {
                ClaimId = claimId
            });

        return NoContent();
    }
    [HttpPost("{claimId}/reject")]
    [Authorize(Roles = nameof(UserRole.ClaimsOfficer))]
    public async Task<IActionResult> Reject(
        Guid claimId,
        RejectClaimCommand command)
    {
        command.ClaimId = claimId;
        await _mediator.Send(command);
        return NoContent();
    }
    [HttpPost("{claimId}/escalate")]
    [Authorize(Roles = nameof(UserRole.ClaimsOfficer))]
    public async Task<IActionResult> Escalate(
        Guid claimId)
    {
        await _mediator.Send(
            new EscalateClaimCommand
            {
                ClaimId = claimId
            });

        return NoContent();
    }
    [HttpPost("{claimId}/settle")]
    [Authorize(Roles = nameof(UserRole.TenantAdmin))]
    public async Task<IActionResult> Settle(
        Guid claimId)
    {
        await _mediator.Send(
            new SettleClaimCommand
            {
                ClaimId = claimId
            });

        return Ok();
    }
    [HttpPost("{claimId}/documents")]
    [Authorize(Roles = nameof(UserRole.Patient))]
    public async Task<IActionResult> UploadDocument(
    Guid claimId,
    IFormFile file)
    {
        await _mediator.Send(
            new UploadClaimDocumentCommand
            {
                ClaimId = claimId,
                File = file
            });

        return NoContent();
    }
    [HttpGet("fraud-flags")]
    [Authorize(Roles = nameof(UserRole.TenantAdmin))]
    public async Task<IActionResult> GetFraudFlags()
    {
        var result = await _mediator.Send(new GetFraudFlagsQuery());
        return Ok(result);
    }
}