using MediatR;
using MediClaim.Application
    .Common.Interfaces;

namespace MediClaim.Application
    .Features.Auth
    .RefreshToken;

public class RefreshTokenCommandHandler
    : IRequestHandler<
        RefreshTokenCommand,
        AuthResponse>
{
    private readonly IRefreshTokenService
        _refreshTokenService;

    public RefreshTokenCommandHandler(
        IRefreshTokenService refreshTokenService)
    {
        _refreshTokenService =
            refreshTokenService;
    }

    public async Task<AuthResponse>
        Handle(
            RefreshTokenCommand request,
            CancellationToken cancellationToken)
    {
        return await _refreshTokenService
            .RefreshAsync(
                request.RefreshToken,
                request.IpAddress,
                request.UserAgent,
                cancellationToken);
    }
}