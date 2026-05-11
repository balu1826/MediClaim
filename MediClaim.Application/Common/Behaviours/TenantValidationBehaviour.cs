using MediatR;
using MediClaim.Application
    .Common.Exceptions;
using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Application
    .Common.Behaviours;

public class TenantValidationBehaviour<
    TRequest,
    TResponse>
    : IPipelineBehavior<
        TRequest,
        TResponse>

    where TRequest : notnull
{
    private readonly ICurrentTenantService
        _currentTenantService;

    private readonly IApplicationDbContext
        _context;

    public TenantValidationBehaviour(
        ICurrentTenantService currentTenantService,
        IApplicationDbContext context)
    {
        _currentTenantService =
            currentTenantService;

        _context = context;
    }

    public async Task<TResponse>
        Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
    {
        // Skip if tenant unavailable

        if (_currentTenantService
            .TenantId is null)
        {
            return await next();
        }

        var tenant =
            await _context.Tenants
                .FirstOrDefaultAsync(
                    x =>
                        x.TenantId ==
                            _currentTenantService
                                .TenantId,
                    cancellationToken);

        if (tenant is null)
        {
            throw new UnauthorizedAccessException(
                "Tenant not found");
        }

        if (tenant.Status ==
            TenantStatus.Suspended)
        {
            throw new ForbiddenAccessException(
                "Tenant suspended");
        }

        return await next();
    }
}