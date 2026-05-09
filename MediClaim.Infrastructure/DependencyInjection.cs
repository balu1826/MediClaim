using MediClaim.Application.Common.Interfaces;
using MediClaim.Infrastructure.MultiTenancy;
using MediClaim.Infrastructure.Persistence;
using MediClaim.Infrastructure.Persistence.Interceptors;
using MediClaim.Infrastructure.Repositories;
using MediClaim.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MediClaim.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection
        AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<
            ICurrentTenantService,
            CurrentTenantService>();

        services.AddScoped<
            AuditableEntityInterceptor>();

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