using System.Security.Claims;
using MediClaim.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace MediClaim.Infrastructure.MultiTenancy;

public class CurrentTenantService
    : ICurrentTenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentTenantService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? TenantId
    {
        get
        {
            var tenantId =
                _httpContextAccessor
                    .HttpContext?
                    .User?
                    .FindFirst("tenant_id")
                    ?.Value;

            return Guid.TryParse(
                tenantId,
                out var parsedTenantId)
                ? parsedTenantId
                : null;
        }
    }

    public bool IsSuperAdmin =>
        _httpContextAccessor
            .HttpContext?
            .User?
            .IsInRole("SuperAdmin")
        ?? false;
}