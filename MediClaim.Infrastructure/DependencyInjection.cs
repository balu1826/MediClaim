using MediClaim.Application.Common.Interfaces;
using MediClaim.Infrastructure.MultiTenancy;
using MediClaim.Infrastructure.Persistence;
using MediClaim.Infrastructure.Persistence.Interceptors;
using MediClaim.Infrastructure.Repositories;
using MediClaim.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MediClaim.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection
        AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        var jwtSettings =
    configuration
        .GetSection("Jwt")
        .Get<JwtSettings>();

        services
            .AddAuthentication(
                JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,

                        ValidateAudience = true,

                        ValidateLifetime = true,

                        ValidateIssuerSigningKey = true,

                        ValidIssuer =
                            jwtSettings!.Issuer,

                        ValidAudience =
                            jwtSettings.Audience,

                        IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(
                                    jwtSettings.Key))
                    };
            });
        services.AddAuthorization();
        services.AddScoped<
            ICurrentTenantService,
            CurrentTenantService>();

        services.AddScoped<
            AuditableEntityInterceptor>();
        services.Configure<JwtSettings>(
            configuration.GetSection("Jwt"));
        services.AddScoped<
            IJwtTokenGenerator,
            JwtTokenGenerator>();

        services.AddDbContext<ApplicationDbContext>(
            (sp, options) =>
            {
                var interceptor =
                    sp.GetRequiredService<
                        AuditableEntityInterceptor>();

                options.UseSqlServer(
                    configuration.GetConnectionString(
                        "DefaultConnection"));

                options.AddInterceptors(interceptor);
            });
        services.AddScoped(typeof(IRepository<>),typeof(Repository<>));
        services.AddScoped<IUnitOfWork,UnitOfWork>();
        services.AddScoped<IUserRepository,UserRepository>();
        services.AddScoped<ITenantRepository,TenantRepository>();
        services.AddScoped<IEncryptionService,EncryptionService>();
        return services;
    }
}