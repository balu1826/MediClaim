using MediatR;
using MediClaim.Application.Common.Interfaces;
using MediClaim.Application.Common.Models;
using MediClaim.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MediClaim.Application.Features.Claims.GetFraudFlags;

public class GetFraudFlagsQueryHandler  : IRequestHandler<
        GetFraudFlagsQuery,
        List<FraudFlagDto>>
  
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentTenantService _tenantService;
    public GetFraudFlagsQueryHandler(
        IApplicationDbContext context,
        ICurrentTenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task<List<FraudFlagDto>> Handle(
        GetFraudFlagsQuery request,
        CancellationToken cancellationToken)
    {
        var tenant =
            await _context.Tenants
                .SingleAsync(
                    x =>
                        x.TenantId ==
                            _tenantService.TenantId,
                    cancellationToken);

        var settings = JsonSerializer.Deserialize<TenantSettings>(tenant.SettingsJson!);
        var threshold = settings!.FraudThreshold;
        return await _context.Claims
            .Where(x =>
                x.TenantId ==
                    tenant.TenantId
                &&
                x.FraudRiskScore >= threshold)
            .Select(x =>
                new FraudFlagDto
                {
                    ClaimId = x.ClaimId,
                    FraudScore = x.FraudRiskScore,
                    Amount = x.Amount,
                    DiagnosisCode = x.DiagnosisCode
                })
            .ToListAsync(cancellationToken);
    }
}