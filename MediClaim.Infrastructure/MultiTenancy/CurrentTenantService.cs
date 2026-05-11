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

            return tenantId is null
                ? null
                : Guid.Parse(tenantId);
        }
        set
        {
            TenantId = value;
        }
    }

    public bool IsSuperAdmin =>
        _httpContextAccessor
            .HttpContext?
            .User?
            .IsInRole("SuperAdmin")
        ?? false;
}