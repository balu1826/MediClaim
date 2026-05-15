using MediatR;
using MediClaim.Application
    .Features.Policies.CreatePolicyType;
using MediClaim.Application.Features.Policies.EnrollPolicy;
using MediClaim.Application.Features.Policies.GetPolicyDetails;
using MediClaim.Application.Features.Policies.GetPolicyTypes;
using MediClaim.Application.Features.Policies.RenewPolicy;
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
    private readonly IMediator _mediator;
    public PoliciesController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("types")]
    [Authorize(Roles = nameof(UserRole.TenantAdmin))]
    public async Task<IActionResult> CreatePolicyType(
            CreatePolicyTypeCommand command)
    {
        var policyTypeId = await _mediator.Send(command);
        return Ok(policyTypeId);
    }
    [HttpPost("enroll")]
    [Authorize(
    Roles = nameof(UserRole.TenantAdmin))]
    public async Task<IActionResult> Enroll(
        EnrollPolicyCommand command)
    {
        var policyId = await _mediator.Send(command);
        return Ok(policyId);
    }
    [HttpPut("{policyId}/upgrade")]
    [Authorize(
    Roles = nameof(UserRole.TenantAdmin))]
    public async Task<IActionResult> Upgrade(
        Guid policyId,
        UpgradePolicyCommand command)
    {
        command.PolicyId = policyId;
        await _mediator.Send(command);
        return NoContent();
    }
    [HttpPost("{policyId}/renew")]
    [Authorize(Roles = nameof(UserRole.TenantAdmin))]
    public async Task<IActionResult> Renew(
    Guid policyId)
    {
        var renewedPolicyId =
            await _mediator.Send(
                new RenewPolicyCommand
                {
                    PolicyId = policyId
                });

        return Ok(renewedPolicyId);
    }
    [HttpGet("types")]
    [Authorize(
    Roles = $"{nameof(UserRole.Patient)},{nameof(UserRole.TenantAdmin)}")]
    public async Task<IActionResult> GetPolicyTypes(
    [FromQuery] Guid? cursor,
    [FromQuery] int pageSize)
    {
        var result =
            await _mediator.Send(
                new GetPolicyTypesQuery
                {
                    Cursor = cursor,
                    PageSize = pageSize
                });

        return Ok(result);
    }

    [HttpGet("{policyId}")]
    [Authorize(
    Roles = $"{nameof(UserRole.Patient)},{nameof(UserRole.TenantAdmin)}")]
    public async Task<IActionResult> GetDetails(
    Guid policyId,
    [FromQuery] Guid? cursor,
    [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(
                new GetPolicyDetailsQuery
                {
                    PolicyId = policyId,
                    Cursor = cursor,
                    PageSize = pageSize
                });
        return Ok(result);
    }
}