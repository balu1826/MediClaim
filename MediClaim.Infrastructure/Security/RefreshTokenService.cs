using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Application
    .Features.Auth;
using MediClaim.Domain.Entities;
using MediClaim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Infrastructure
    .Security;

public class RefreshTokenService
    : IRefreshTokenService
{
    private readonly ApplicationDbContext
        _context;

    private readonly IJwtTokenGenerator
        _jwtTokenGenerator;

    private readonly IUnitOfWork
        _unitOfWork;

    public RefreshTokenService(
        ApplicationDbContext context,
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _context = context;

        _jwtTokenGenerator =
            jwtTokenGenerator;

        _unitOfWork = unitOfWork;
    }

    public async Task<AuthResponse>
        RefreshAsync(
            string refreshToken,
            string? ipAddress,
            string? userAgent,
            CancellationToken cancellationToken)
    {
        // Hash incoming token

        var tokenHash =
            RefreshTokenHasher
                .Hash(refreshToken);

        // Find token

        var existingToken =
            await _context.RefreshTokens
                .FirstOrDefaultAsync(
                    x =>
                        x.TokenHash ==
                            tokenHash,
                    cancellationToken);

        if (existingToken is null)
        {
            throw new UnauthorizedAccessException(
                "Invalid refresh token");
        }

        // Expired

        if (existingToken.ExpiresAt
            <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException(
                "Refresh token expired");
        }

        // Revoked

        if (existingToken.IsRevoked)
        {
            throw new UnauthorizedAccessException(
                "Refresh token revoked");
        }

        // TOKEN REUSE DETECTED

        if (existingToken.IsUsed)
        {
            await HandleReuseDetectionAsync(
                existingToken,
                ipAddress,
                cancellationToken);

            throw new UnauthorizedAccessException(
                "Token reuse detected. All sessions revoked.");
        }

        // Load user

        var user =
            await _context.Users
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId ==
                            existingToken.UserId,
                    cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException(
                "User not found");
        }

        // Rotate token

        existingToken.IsUsed = true;

        var newRefreshToken =
            Guid.NewGuid()
                .ToString();

        var newTokenHash =
            RefreshTokenHasher
                .Hash(newRefreshToken);

        existingToken.ReplacedByTokenHash =
            newTokenHash;

        // Create replacement token

        var replacementToken =
            new RefreshToken
            {
                TokenId = Guid.NewGuid(),

                UserId = user.UserId,

                FamilyId =
                    existingToken.FamilyId,

                TokenHash =
                    newTokenHash,

                ExpiresAt =
                    DateTime.UtcNow
                        .AddDays(7),

                CreatedByIp =
                    ipAddress,

                UserAgent =
                    userAgent
            };

        await _context.RefreshTokens
            .AddAsync(
                replacementToken,
                cancellationToken);

        // Generate new JWT

        var accessToken =
            _jwtTokenGenerator
                .GenerateToken(user);

        await _unitOfWork
            .SaveChangesAsync(
                cancellationToken);

        return new AuthResponse
        {
            AccessToken =
                accessToken,

            RefreshToken =
                newRefreshToken
        };
    }

    private async Task
        HandleReuseDetectionAsync(
            RefreshToken reusedToken,
            string? ipAddress,
            CancellationToken cancellationToken)
    {
        // Revoke ENTIRE family

        var familyTokens =
            await _context.RefreshTokens
                .Where(x =>
                    x.FamilyId ==
                        reusedToken.FamilyId)
                .ToListAsync(
                    cancellationToken);

        foreach (var token
            in familyTokens)
        {
            token.IsRevoked = true;

            token.RevokedAt =
                DateTime.UtcNow;

            token.RevokedReason =
                "ReuseDetected";
        }

        // Global logout:
        // revoke ALL user tokens

        var allUserTokens =
            await _context.RefreshTokens
                .Where(x =>
                    x.UserId ==
                        reusedToken.UserId)
                .ToListAsync(
                    cancellationToken);

        foreach (var token
            in allUserTokens)
        {
            token.IsRevoked = true;

            token.RevokedAt =
                DateTime.UtcNow;

            token.RevokedReason =
                "GlobalSecurityRevocation";
        }

        // TODO:
        // Queue security email

        // TODO:
        // Write AuditLog

        await _unitOfWork
            .SaveChangesAsync(
                cancellationToken);
    }
}