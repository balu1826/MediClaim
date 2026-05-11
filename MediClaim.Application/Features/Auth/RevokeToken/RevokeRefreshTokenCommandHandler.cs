using MediatR;
using MediClaim.Application
    .Common.Interfaces;

namespace MediClaim.Application
    .Features.Auth
    .RevokeToken;

public class RevokeRefreshTokenCommandHandler
    : IRequestHandler<
        RevokeRefreshTokenCommand>
{
    private readonly IRefreshTokenService
        _refreshTokenService;

    public RevokeRefreshTokenCommandHandler(
        IRefreshTokenService refreshTokenService)
    {
        _refreshTokenService =
            refreshTokenService;
    }

    public async Task Handle(
        RevokeRefreshTokenCommand request,
        CancellationToken cancellationToken)
    {
        await _refreshTokenService
            .RevokeAsync(
                request.RefreshToken,
                cancellationToken);
    }
}