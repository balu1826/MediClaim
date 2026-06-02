using FluentValidation;
using MediatR;
using MediClaim.Application.Common.Behaviours;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace MediClaim.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
            this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                Assembly.GetExecutingAssembly());
        });

        //services.AddValidatorsFromAssembly(
        //    Assembly.GetExecutingAssembly());
        //services.AddTransient(
        //    typeof(IPipelineBehavior<,>),
        //    typeof(ValidationBehaviour<,>));
        //services.AddTransient(
        //    typeof(IPipelineBehavior<,>),
        //    typeof(AuthorizationBehaviour<,>));
        //services.AddTransient(
        //    typeof(IPipelineBehavior<,>),
        //    typeof(PerformanceBehaviour<,>));
        //services.AddTransient(
        //    typeof(IPipelineBehavior<,>),
        //    typeof(TenantValidationBehaviour<,>));
        //services.AddTransient(
        //    typeof(IPipelineBehavior<,>),
        //    typeof(AuditBehaviour<,>));

        return services;
    }
}