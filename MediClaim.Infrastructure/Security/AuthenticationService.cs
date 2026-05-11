using BCrypt.Net;
using MediClaim.Application.Common.Exceptions;
using MediClaim.Application.Common.Interfaces;
using MediClaim.Application.Features.Auth.Common;
using MediClaim.Domain.Entities;
using MediClaim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Infrastructure.Security;

public class AuthenticationService
    : IAuthenticationService
{
    private readonly ApplicationDbContext
        _context;

    private readonly IJwtTokenGenerator
        _jwtTokenGenerator;


    private readonly IUnitOfWork
        _unitOfWork;

    public AuthenticationService(
        ApplicationDbContext context,
        IJwtTokenGenerator jwtTokenGenerator,
        IUnitOfWork unitOfWork)
    {
        _context = context;

        _jwtTokenGenerator =
            jwtTokenGenerator;

        _unitOfWork =
            unitOfWork;
    }

    public async Task<AuthResponseDto>
        LoginAsync(
            string email,
            string password,
            string? ipAddress,
            string? userAgent,
            CancellationToken cancellationToken)
    {
        var user =
            await _context.Users
                .FirstOrDefaultAsync(
                    x =>
                        x.Email == email,
                    cancellationToken);

        if (user is null)
        {
            throw new ConflictException(
                "Invalid credentials");
        }

        var passwordValid =
            BCrypt.Net.BCrypt.Verify(
                password,
                user.PasswordHash);

        if (!passwordValid)
        {
            throw new ConflictException(
                "Invalid credentials");
        }

        // Generate access token

        var accessToken =
            _jwtTokenGenerator
                .GenerateToken(user);

        // Create refresh token family

        var familyId =
            Guid.NewGuid();

        // Raw token returned to client

        var rawRefreshToken =
            Guid.NewGuid()
                .ToString();

        // Hash stored in DB

        var refreshTokenHash =
            RefreshTokenHasher
                .Hash(rawRefreshToken);

        var refreshToken =
            new RefreshToken
            {
                TokenId =
                    Guid.NewGuid(),

                UserId =
                    user.UserId,

                FamilyId =
                    familyId,

                TokenHash =
                    refreshTokenHash,

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
                refreshToken,
                cancellationToken);

        await _unitOfWork
            .SaveChangesAsync(
                cancellationToken);

        return new AuthResponseDto
        {
            AccessToken =
                accessToken,

            RefreshToken =
                rawRefreshToken
        };
    }
}