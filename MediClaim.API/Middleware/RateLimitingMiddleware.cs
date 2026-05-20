using Microsoft.Extensions.Caching.Memory;

namespace MediClaim.API
    .Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate
        _next;

    private readonly IMemoryCache
        _cache;

    private const int Limit = 10;

    private static readonly TimeSpan
        Window =
            TimeSpan.FromSeconds(60);

    public RateLimitingMiddleware(
        RequestDelegate next,
        IMemoryCache cache)
    {
        _next = next;

        _cache = cache;
    }

    public async Task InvokeAsync(
        HttpContext context)
    {
        var path =
            context.Request.Path
                .Value?
                .ToLower();

        // Apply ONLY to auth endpoints
        if (path is null
            ||
            !path.StartsWith(
                "/api/auth"))
        {
            await _next(context);

            return;
        }
        var ip = context.Connection
                .RemoteIpAddress
                ?.ToString()
            ?? "unknown";
        var key = $"{ip}:{path}";
        var now = DateTime.UtcNow;
        var requests =
            _cache.GetOrCreate(
                key,
                entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow =
                        Window;

                    return new List<DateTime>();
                })!;

        lock (requests)
        {
            // Remove expired timestamps

            requests.RemoveAll(x => now - x > Window);
            // Limit exceeded
            if (requests.Count >= Limit)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.Headers["Retry-After"] = "60";
                context.Response
                             .WriteAsJsonAsync(
                                 new
                                 {
                                     Error = "Rate limit exceeded"

                                 });
                return;
            }
            requests.Add(now);
        }

        await _next(context);
    }
}