using MediatR;
using MediClaim.Application.Common.Interfaces;
using MediClaim.Application.Common.Models;
using MediClaim.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using MediClaim.Application.Repositories;

namespace MediClaim.Application.Features.Claims.GetFraudFlags;

public class GetFraudFlagsQueryHandler  : IRequestHandler<
        GetFraudFlagsQuery,
        List<FraudFlagDto>>
  
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentTenantService _tenantService;
    private readonly ITenantRepository _tenantRepository;
    private readonly IClaimRepository _claimRepository;
    public GetFraudFlagsQueryHandler(
        IApplicationDbContext context,
        ICurrentTenantService tenantService,
        ITenantRepository tenantRepository,
        IClaimRepository claimRepository)
    {
        _context = context;
        _tenantService = tenantService;
        _tenantRepository = tenantRepository;
        _claimRepository = claimRepository;
    }

    public async Task<List<FraudFlagDto>> Handle(
        GetFraudFlagsQuery request,
        CancellationToken cancellationToken)
    {
       
        var tenant =
            await _tenantRepository.GetTenantByIdAsync(
                _tenantService.TenantId,
                cancellationToken);
        if(tenant == null)
        {
            throw new UnauthorizedAccessException("Tenant not found");
        }

        var settings = JsonSerializer.Deserialize<TenantSettings>(tenant.SettingsJson!);
        var threshold = settings!.FraudThreshold;
        return await _claimRepository.GetFraudFlagsAsync(tenant.TenantId, threshold, cancellationToken);
    }
}
