using System.Security.Claims;
using MediClaim.Application.Common.Exceptions;
using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.API
    .Middleware;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    public TenantResolutionMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ICurrentTenantService currentTenantService,
        IApplicationDbContext dbContext)
    {
        var path = context.Request.Path
                .Value?
                .ToLower();
        // Skip auth endpoints
        if (path is not null
            &&
            (
                path.Contains(
                    "/api/auth/login")
                ||
                path.Contains(
                    "/api/auth/register")
            ))
        {
            await _next(context);
            return;
        }
        // Skip unauthenticated users
        if (!(context.User
            ?.Identity
            ?.IsAuthenticated ?? false))
        {
            await _next(context);
            return;
        }

        // Extract tenant claim
        var tenantClaim =
            context.User.FindFirst(
                "tenant_id")
                    ?.Value;
        if (string.IsNullOrWhiteSpace(tenantClaim))
        {
            throw new UnauthorizedAccessException("Tenant not found");
        }
        var tenantId = Guid.Parse(tenantClaim);
        // Load tenant
        var tenant =
            await dbContext.Tenants
                .FirstOrDefaultAsync(
                    x =>
                        x.TenantId ==
                            tenantId);

        if (tenant is null)
        {
            throw new UnauthorizedAccessException("Tenant not found");
        }

        // Suspended tenant

        if (tenant.Status == TenantStatus.Suspended)
        {
            throw new ForbiddenAccessException("Tenant Suspended");

        }
        await _next(context);
    }
}