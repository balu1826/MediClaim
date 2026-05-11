using MediatR;
using MediClaim.Application.Features.Auth.Commands.Login;
using MediClaim.Application.Features.Auth.Common;
using MediClaim.Application
    .Common.Interfaces;

public class LoginCommandHandler
    : IRequestHandler<
        LoginCommand,
        AuthResponseDto>
{
    private readonly IAuthenticationService
        _authenticationService;

    public LoginCommandHandler(
        IAuthenticationService authenticationService)
    {
        _authenticationService =
            authenticationService;
    }

    public async Task<AuthResponseDto>
        Handle(
            LoginCommand request,
            CancellationToken cancellationToken)
    {
        return await _authenticationService
            .LoginAsync(
                request.Email,
                request.Password,
                request.IpAddress,
                request.UserAgent,
                cancellationToken);
    }
}