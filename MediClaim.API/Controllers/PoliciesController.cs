using MediatR;
using MediClaim.Application.Features.Claims.UpgradePolicy;
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
[Route("v1/policies")]
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
    /// <summary>
    /// Enroll patient into policy.
    /// </summary>
    /// <remarks>
    /// Creates a new policy enrollment for the specified patient.
    /// </remarks>
    /// <response code="200">
    /// Policy enrolled successfully.
    /// </response>
    /// <response code="400">
    /// Validation failure.
    /// </response>
    /// <response code="401">
    /// Unauthorized.
    /// </response>
    /// <response code="403">
    /// Forbidden.
    /// </response>
    /// <response code="500">
    /// Internal server error.
    /// </response>
    [HttpPost("enroll")]
    [Authorize(
    Roles = nameof(UserRole.TenantAdmin))]
    public async Task<IActionResult> Enroll(
        EnrollPolicyCommand command)
    {
        var policyId = await _mediator.Send(command);
        return Ok(policyId);
    }
    /// <summary>
    /// Upgrade existing policy.
    /// </summary>
    /// <remarks>
    /// Upgrades policy to another policy type.
    /// </remarks>
    /// <response code="204">
    /// Policy upgraded successfully.
    /// </response>
    /// <response code="400">
    /// Validation failure.
    /// </response>
    /// <response code="401">
    /// Unauthorized.
    /// </response>
    /// <response code="403">
    /// Forbidden.
    /// </response>
    /// <response code="404">
    /// Policy not found.
    /// </response>
    /// <response code="500">
    /// Internal server error.
    /// </response>
    [HttpPut("{policyId}/upgrade")]
    [Authorize(
    Roles = nameof(UserRole.TenantAdmin))]
    public async Task<IActionResult> Upgrade(
        Guid policyId,
        [FromBody] UpgradePolicyRequest request)
    {
        var command = new UpgradePolicyCommand
        {
            PolicyId = policyId,
            NewPolicyTypeId = request.NewPolicyTypeId
        };
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