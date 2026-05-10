using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MediClaim.Application.Common.Interfaces;
using MediClaim.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
namespace MediClaim.Infrastructure.Security;

public class JwtTokenGenerator
    : IJwtTokenGenerator
{
    private readonly JwtSettings
        _jwtSettings;

    public JwtTokenGenerator(
        IOptions<JwtSettings> options)
    {
        _jwtSettings = options.Value;
    }

    public string GenerateToken(User user)
    {
        var claims = new List<System.Security.Claims.Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                user.UserId.ToString()),

            new(
                JwtRegisteredClaimNames.Email,
                user.Email),

            new(
                "tenant_id",
                user.TenantId.ToString()),

            new(
                ClaimTypes.Role,
                user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _jwtSettings.Key));

        var credentials =
            new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,

            audience: _jwtSettings.Audience,

            claims: claims,

            expires: DateTime.UtcNow.AddMinutes(
                _jwtSettings
                    .AccessTokenExpiryMinutes),

            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}