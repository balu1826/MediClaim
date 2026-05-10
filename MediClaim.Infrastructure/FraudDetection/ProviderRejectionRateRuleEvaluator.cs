using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Domain.Entities;
using MediClaim.Domain.Enums;
using MediClaim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Infrastructure
    .FraudDetection.Rules;

public class ProviderRejectionRateRuleEvaluator
    : IFraudRuleEvaluator
{
    private readonly ApplicationDbContext
        _context;

    public ProviderRejectionRateRuleEvaluator(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public string RuleName =>
        nameof(
            ProviderRejectionRateRuleEvaluator);

    public async Task<int>
        EvaluateAsync(
            Claim claim)
    {
        if (claim.ProviderId is null)
        {
            return 0;
        }

        var since =
            DateTime.UtcNow
                .AddMonths(-6);

        var claims =
            await _context.Claims
                .Where(x =>
                    x.ProviderId ==
                        claim.ProviderId
                    && x.TenantId ==
                        claim.TenantId
                    && x.CreatedAt >= since)
                .ToListAsync();

        if (!claims.Any())
        {
            return 0;
        }

        var rejected =
            claims.Count(x =>
                x.Status ==
                    ClaimStatus.Rejected);

        var rate =
            (decimal)rejected /
            claims.Count;

        return rate > 0.4m
            ? 10
            : 0;
    }
}