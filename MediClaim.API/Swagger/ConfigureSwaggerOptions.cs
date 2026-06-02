using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace MediClaim.API.Swagger;

public static class ConfigureSwaggerOptions
{
    public static IServiceCollection AddSwaggerDocumentation(
        this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            var xmlFile =
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";

            var xmlPath =
                Path.Combine(AppContext.BaseDirectory, xmlFile);

            c.IncludeXmlComments(xmlPath);

            c.CustomOperationIds(apiDesc =>
            {
                var action = apiDesc.ActionDescriptor.RouteValues["action"];

                return char.ToLowerInvariant(action[0]) +
                       action.Substring(1);
            });
            //c.OperationFilter<CorrelationIdOperationFilter>();
            c.OperationFilter<StandardResponsesOperationFilter>();
         
        });

        return services;
    }
}