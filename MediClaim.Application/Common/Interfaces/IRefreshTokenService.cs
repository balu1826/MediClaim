using MediClaim.Application
    .Features.Auth;

namespace MediClaim.Application
    .Common.Interfaces;

public interface IRefreshTokenService
{
    Task<AuthResponse>
        RefreshAsync(
            string refreshToken,
            string? ipAddress,
            string? userAgent,
            CancellationToken cancellationToken);
}