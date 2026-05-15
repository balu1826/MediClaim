using System.Net;
using MediClaim.API.Common;
using Microsoft.AspNetCore
    .Authentication.JwtBearer;

namespace MediClaim.API.Common;

public static class JwtBearerEventExtensions
{
    public static JwtBearerEvents
        GetJwtBearerEvents()
    {
        return new JwtBearerEvents
        {
            OnForbidden = async context =>
            {
                context.Response.ContentType =
                    "application/json";

                context.Response.StatusCode =
                    (int)HttpStatusCode.Forbidden;

                var response =
                    new ProblemDetailsResponse
                    {
                        Title = "Forbidden",
                        Status =
                            context.Response.StatusCode,

                        Detail =
                            "You do not have permission to access this resource.",

                        TraceId =
                            context.HttpContext
                                .TraceIdentifier
                    };

                await context.Response
                    .WriteAsJsonAsync(response);
            },

            OnChallenge = async context =>
            {
                context.HandleResponse();

                context.Response.ContentType =
                    "application/json";

                context.Response.StatusCode =
                    (int)HttpStatusCode.Unauthorized;

                var response =
                    new ProblemDetailsResponse
                    {
                        Title = "Unauthorized",
                        Status =
                            context.Response.StatusCode,

                        Detail =
                            "Authentication is required to access this resource.",

                        TraceId =
                            context.HttpContext
                                .TraceIdentifier
                    };

                await context.Response
                    .WriteAsJsonAsync(response);
            }
        };
    }
}