using BCrypt.Net;
using MediatR;
using MediClaim.Application.Common.Exceptions;
using MediClaim.Application.Common.Interfaces;
using MediClaim.Application
    .Features.Auth.Common;

namespace MediClaim.Application
    .Features.Auth.Commands.Login;

public class LoginCommandHandler
    : IRequestHandler<
        LoginCommand,
        AuthResponseDto>
{
    private readonly IUserRepository
        _userRepository;

    private readonly IJwtTokenGenerator
        _jwtTokenGenerator;

    public LoginCommandHandler(
        IUserRepository userRepository,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepository = userRepository;

        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<AuthResponseDto> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var user =
            await _userRepository
                .EmailExistsAsync(
                    request.Email);

        if (user is null)
        {
            throw new ConflictException(
                "Invalid credentials");
        }

        var passwordValid =
            BCrypt.Net.BCrypt.Verify(
                request.Password,
                user.PasswordHash);

        if (!passwordValid)
        {
            throw new ConflictException(
                "Invalid credentials");
        }

        var token =
            _jwtTokenGenerator
                .GenerateToken(user);

        return new AuthResponseDto
        {
            AccessToken = token
        };
    }
}