using MediatR;
using MediClaim.Application
    .Features.Auth.Common;

namespace MediClaim.Application
    .Features.Auth.Commands.Login;

public class LoginCommand
    : IRequest<AuthResponseDto>
{
    public string Email { get; set; }
        = default!;

    public string Password { get; set; }
        = default!;
}