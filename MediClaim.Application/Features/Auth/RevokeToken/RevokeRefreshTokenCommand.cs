using MediatR;

namespace MediClaim.Application
    .Features.Auth
    .RevokeToken;

public class RevokeRefreshTokenCommand
    : IRequest
{
    public string RefreshToken
    {
        get; set;
    } = default!;
}