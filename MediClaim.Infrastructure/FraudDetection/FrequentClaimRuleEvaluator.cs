using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Domain.Entities;
using MediClaim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Infrastructure
    .FraudDetection.Rules;

public class FrequentClaimRuleEvaluator
    : IFraudRuleEvaluator
{
    private readonly ApplicationDbContext
        _context;

    public FrequentClaimRuleEvaluator(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public string RuleName =>
        nameof(
            FrequentClaimRuleEvaluator);

    public async Task<int>
        EvaluateAsync(
            Claim claim)
    {
        var since =
            DateTime.UtcNow
                .AddDays(-30);

        var claims =
            await _context.Claims
                .Where(x =>
                    x.UserId ==
                        claim.UserId
                    && x.DiagnosisCode ==
                        claim.DiagnosisCode
                    && x.CreatedAt >= since
                    && x.ProviderId !=
                        claim.ProviderId)
                .Select(x =>
                    x.ProviderId)
                .Distinct()
                .CountAsync();

        return claims > 3
            ? 25
            : 0;
    }
}