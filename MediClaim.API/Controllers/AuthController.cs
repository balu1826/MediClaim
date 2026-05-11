using MediatR;
using MediClaim.Application.Common.Interfaces;
using MediClaim.Application.Features.Auth.Commands.Login;
using MediClaim.Application
    .Features.Auth.Commands.RegisterTenant;
using MediClaim.Application.Features.Auth.RefreshToken;
using MediClaim.Application.Features.Auth.RevokeToken;
using MediClaim.Application.Features.Auth.UnlockUser;
using MediClaim.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace MediClaim.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IUserRepository
    _currentUserService;

    public AuthController(
        IMediator mediator, IUserRepository currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }
    //Register Tenant
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
    //Login for user
    [HttpPost("login")]
    public async Task<IActionResult> Login(
       LoginCommand command)
    {
        command.IpAddress =
           HttpContext
               .Connection
               .RemoteIpAddress
               ?.ToString();

        command.UserAgent =
            Request.Headers[
                "User-Agent"]
                    .ToString();
        var result =
            await _mediator.Send(command);

        return Ok(result);
    }
    [HttpPost("revoke")]
    [Authorize]
    public async Task<IActionResult>
       Revoke(
           RevokeRefreshTokenCommand command)
    {
        await _mediator.Send(
            command);

        return NoContent();
    }
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult>
    Refresh(
        RefreshTokenCommand command)
    {
        command.IpAddress =
            HttpContext
                .Connection
                .RemoteIpAddress
                ?.ToString();

        command.UserAgent =
            Request.Headers[
                "User-Agent"]
                    .ToString();

        try
        {
            var response =
                await _mediator.Send(
                    command);

            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(
                new
                {
                    error = ex.Message
                });
        }
    }
    [HttpPost("unlock/{userId}")]
    [Authorize(
    Roles =
        nameof(UserRole.TenantAdmin))]
    public async Task<IActionResult>
    Unlock(
        Guid userId)
    {
        await _mediator.Send(
            new UnlockUserCommand
            {
                UserId = userId
            });

        return NoContent();
    }
}