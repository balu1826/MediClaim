using MediatR;
using System.IdentityModel.Tokens.Jwt;
using MediClaim.Application.Features.Auth.Commands.Login;
using MediClaim.Application
    .Features.Auth.Commands.RegisterTenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MediClaim.Application.Common.Interfaces;

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
}