using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Domain.Entities;
using MediClaim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Infrastructure
    .FraudDetection.Rules;

public class FrequentClaimRuleEvaluator : IFraudRuleEvaluator

{
    private readonly ApplicationDbContext _context;
    public FrequentClaimRuleEvaluator(
        ApplicationDbContext context)
    {
        _context = context;
    }
    public string RuleName => GetType().Name;
    public async Task<int> EvaluateAsync(
            Claim claim, CancellationToken cancellationToken)
    {
        var since =
            DateTime.UtcNow
                .AddDays(-30);

        var claims = await _context.Claims
            .Where(x =>
                 x.UserId == claim.UserId &&
                 x.DiagnosisCode == claim.DiagnosisCode &&
                 x.CreatedAt >= since)
            .ToListAsync();
        var distinctProviders = claims
            .Select(x => x.ProviderId)
            .Distinct()
            .Count();
        return claims.Count > 3 && distinctProviders > 1
            ? 25
            : 0;
    }
}