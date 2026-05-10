using MediatR;
using MediClaim.Application
    .Features.Users.Commands.CreateUser;
using MediClaim.Application.Features.Users.Queries.GetUsers;
using MediClaim.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MediClaim.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController
    : ControllerBase
{
    private readonly IMediator
        _mediator;

    public UsersController(
        IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize(
        Roles =
            nameof(UserRole.TenantAdmin))]
    public async Task<IActionResult>
        Create(
            CreateUserCommand command)
    {
        var userId =
            await _mediator
                .Send(command);

        return Ok(userId);
    }
    [HttpGet]
    [Authorize(
    Roles =
        nameof(UserRole.TenantAdmin))]
    public async Task<IActionResult>
    Get()
    {
        var users =
            await _mediator.Send(
                new GetUsersQuery());

        return Ok(users);
    }
}