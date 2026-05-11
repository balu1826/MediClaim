using MediClaim.Application.Common.Interfaces;
using Serilog;
using System.Diagnostics;

namespace MediClaim.API
    .Middleware;

public class RequestTimingMiddleware
{
    private readonly RequestDelegate
        _next;

    public RequestTimingMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ICurrentTenantService currentTenantService)
    {
        var stopwatch =
            Stopwatch.StartNew();

        context.Response.OnStarting(() =>
        {
            stopwatch.Stop();

            var elapsedMs =
                stopwatch
                    .ElapsedMilliseconds;

            context.Response.Headers[
                "X-Response-Time-Ms"] =
                    elapsedMs.ToString();

            if (elapsedMs > 1000)
            {
                Log.Warning(
                    "Slow request detected. " +
                    "Method: {Method}, " +
                    "Route: {Route}, " +
                    "StatusCode: {StatusCode}, " +
                    "TenantId: {TenantId}, " +
                    "ElapsedMs: {ElapsedMs}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    currentTenantService.TenantId,
                    elapsedMs);
            }

            return Task.CompletedTask;
        });

        await _next(context);
    }
}