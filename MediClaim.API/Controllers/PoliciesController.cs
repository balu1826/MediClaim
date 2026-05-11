using MediatR;
using MediClaim.Application
    .Features.Policies.CreatePolicyType;
using MediClaim.Application.Features.Policies.EnrollPolicy;
using MediClaim.Application.Features.Policies.UpgradePolicy;
using MediClaim.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediClaim.API.Controllers;

[ApiController]
[Route("api/policies")]
[Authorize]
public class PoliciesController
    : ControllerBase
{
    private readonly IMediator
        _mediator;

    public PoliciesController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("types")]
    [Authorize(
        Roles =
            nameof(UserRole.TenantAdmin))]
    public async Task<IActionResult>
        CreatePolicyType(
            CreatePolicyTypeCommand command)
    {
        var policyTypeId =
            await _mediator
                .Send(command);

        return Ok(policyTypeId);
    }
    [HttpPost("enroll")]
    [Authorize(
    Roles =
        nameof(UserRole.TenantAdmin))]
    public async Task<IActionResult>
    Enroll(
        EnrollPolicyCommand command)
    {
        var policyId =
            await _mediator
                .Send(command);

        return Ok(policyId);
    }
    [HttpPost("{policyId}/upgrade")]
    [Authorize(
    Roles =
        nameof(UserRole.TenantAdmin))]
    public async Task<IActionResult>
    Upgrade(
        Guid policyId,
        UpgradePolicyCommand command)
    {
        command.PolicyId =
            policyId;

        await _mediator.Send(
            command);

        return NoContent();
    }
}