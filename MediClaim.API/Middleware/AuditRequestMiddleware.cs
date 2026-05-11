using System.Text;
using MediClaim.Application
    .Common.Models;

namespace MediClaim.API
    .Middleware;

public class AuditRequestMiddleware
{
    private readonly RequestDelegate
        _next;

    public AuditRequestMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context)
    {
        var method =
            context.Request.Method;

        // Skip GET requests

        if (HttpMethods.IsGet(method))
        {
            await _next(context);

            return;
        }

        // BUFFER REQUEST

        context.Request.EnableBuffering();

        string requestBody;

        using (var reader =
            new StreamReader(
                context.Request.Body,
                Encoding.UTF8,
                leaveOpen: true))
        {
            requestBody =
                await reader
                    .ReadToEndAsync();

            context.Request.Body.Position = 0;
        }

        // BUFFER RESPONSE

        var originalResponseBody =
            context.Response.Body;

        using var responseBodyStream =
            new MemoryStream();

        context.Response.Body =
            responseBodyStream;

        await _next(context);

        // READ RESPONSE BODY

        context.Response.Body.Seek(
            0,
            SeekOrigin.Begin);

        string responseBody;

        using (var reader =
            new StreamReader(
                context.Response.Body,
                Encoding.UTF8,
                leaveOpen: true))
        {
            responseBody =
                await reader
                    .ReadToEndAsync();
        }

        context.Response.Body.Seek(
            0,
            SeekOrigin.Begin);

        // STORE CONTEXT

        context.Items[
            nameof(RequestAuditContext)] =
                new RequestAuditContext
                {
                    RequestBody =
                        requestBody,

                    ResponseBody =
                        responseBody
                };

        // COPY RESPONSE BACK

        await responseBodyStream
            .CopyToAsync(
                originalResponseBody);

        context.Response.Body =
            originalResponseBody;
    }
}