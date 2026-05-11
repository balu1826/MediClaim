using MediClaim.Application
    .Features.Auth.Common;

namespace MediClaim.Application
    .Common.Interfaces;

public interface IAuthenticationService
{
    Task<AuthResponseDto>
        LoginAsync(
            string email,
            string password,
            string? ipAddress,
            string? userAgent,
            CancellationToken cancellationToken);
}