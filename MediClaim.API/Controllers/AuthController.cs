using MediatR;
using MediClaim.Application.Common.Interfaces;
using MediClaim.Application.Features.Auth.Commands.Login;
using MediClaim.Application
    .Features.Auth.Commands.RegisterTenant;
using MediClaim.Application.Features.Auth.RefreshToken;
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
    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        var userId =
            _currentUserService.UserId;

        var email =
           _currentUserService.Email;
        var tenantId =
            _currentUserService.TenantId;

        var role =
            _currentUserService.Role;

        return Ok(new
        {
            UserId = userId,
            Email = email,
            TenantId = tenantId,
            Role = role
        });
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
}