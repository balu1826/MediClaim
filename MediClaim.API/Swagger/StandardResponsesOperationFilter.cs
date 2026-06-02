using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MediClaim.API.Swagger;

public class StandardResponsesOperationFilter
    : IOperationFilter
{
    public void Apply(
        OpenApiOperation operation,
        OperationFilterContext context)
    {
        operation.Responses.TryAdd(
            "500",
            new OpenApiResponse
            {
                Description = "Internal server error"
            });

        operation.Responses.TryAdd(
            "401",
            new OpenApiResponse
            {
                Description = "Unauthorized"
            });

        operation.Responses.TryAdd(
            "403",
            new OpenApiResponse
            {
                Description = "Forbidden"
            });
    }
}