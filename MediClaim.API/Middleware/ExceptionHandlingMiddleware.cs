using System.Net;
using System.Text.Json;
using FluentValidation;
using MediClaim.Application.Common.Exceptions;
using MediClaim.API.Common;

namespace MediClaim.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<
        ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;

        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(
                context,
                exception);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        var response = context.Response;

        response.ContentType =
            "application/json";

        var problemDetails =
            new ProblemDetailsResponse
            {
                TraceId = context.TraceIdentifier
            };

        switch (exception)
        {
            case ValidationException validationException:

                response.StatusCode =
                    (int)HttpStatusCode.BadRequest;

                problemDetails.Title =
                    "Validation Error";

                problemDetails.Status =
                    response.StatusCode;

                problemDetails.Errors =
                    validationException.Errors
                        .Select(x => x.ErrorMessage)
                        .ToList();

                break;

            case ConflictException conflictException:

                response.StatusCode =
                    (int)HttpStatusCode.Conflict;

                problemDetails.Title =
                    "Conflict";

                problemDetails.Status =
                    response.StatusCode;

                problemDetails.Detail =
                    conflictException.Message;

                break;
            case UnprocessableEntityException UnprocessableEntityException:
                response.StatusCode =
                   (int)HttpStatusCode.UnprocessableEntity;

                problemDetails.Title =
                    "UnprocessableEntityException";

                problemDetails.Status =
                    response.StatusCode;

                problemDetails.Detail =
                    UnprocessableEntityException.Message;

                break;

            default:

                response.StatusCode =
                    (int)HttpStatusCode.InternalServerError;

                problemDetails.Title =
                    "Server Error";

                problemDetails.Status =
                    response.StatusCode;

                problemDetails.Detail =
                    "An unexpected error occurred.";

                _logger.LogError(
                    exception,
                    exception.Message);

                break;
        }

        var json =
            JsonSerializer.Serialize(problemDetails);

        await response.WriteAsync(json);
    }
}