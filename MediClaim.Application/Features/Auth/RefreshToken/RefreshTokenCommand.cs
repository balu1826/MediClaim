using MediatR;

namespace MediClaim.Application
    .Features.Auth
    .RefreshToken;

public class RefreshTokenCommand
    : IRequest<AuthResponse>
{
    public string RefreshToken
    {
        get; set;
    } = default!;

    public string? IpAddress
    {
        get; set;
    }

    public string? UserAgent
    {
        get; set;
    }
}