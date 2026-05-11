using Microsoft.Extensions.Logging;
using System.Diagnostics;
using MediatR;
using MediClaim.Application
    .Common.Interfaces;


namespace MediClaim.Application
    .Common.Behaviours;

public class PerformanceBehaviour<
    TRequest,
    TResponse>
    : IPipelineBehavior<
        TRequest,
        TResponse>

    where TRequest : notnull
{
    private readonly ICurrentTenantService
        _currentTenantService;
    private readonly ILogger<
      PerformanceBehaviour<
          TRequest,
          TResponse>>
              _logger;

    public PerformanceBehaviour(
        ICurrentTenantService currentTenantService, ILogger<PerformanceBehaviour<TRequest, TResponse>> logger   )
    {
        _currentTenantService =
            currentTenantService;
        _logger = logger;
    }

    public async Task<TResponse>
        Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
    {
        var stopwatch =
            Stopwatch.StartNew();

        var response =
            await next();

        stopwatch.Stop();

        var elapsedMs =
            stopwatch.ElapsedMilliseconds;

        if (elapsedMs > 500)
        {
            _logger.LogWarning(
                "Slow handler execution detected. " +
                "Handler: {Handler}, " +
                "TenantId: {TenantId}, " +
                "ElapsedMs: {ElapsedMs}",
                typeof(TRequest).Name,
                _currentTenantService
                    .TenantId,
                elapsedMs);
        }

        return response;
    }
}