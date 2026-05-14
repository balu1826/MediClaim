using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Domain.Entities;
using MediClaim.Domain.Enums;
using MediClaim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Infrastructure
    .FraudDetection.Rules;

public class PriorRejectionRuleEvaluator : IFraudRuleEvaluator
{
    private readonly ApplicationDbContext _context;
    public PriorRejectionRuleEvaluator(ApplicationDbContext context)
    {
        _context = context;
    }
    public string RuleName => GetType().Name;
    public async Task<int> EvaluateAsync(
            Claim claim, CancellationToken cancellationToken)
    {
        var since = DateTime.UtcNow.AddDays(-90);
        var exists =
            await _context.Claims
                .AnyAsync(x =>
                    x.UserId ==
                        claim.UserId
                    && x.DiagnosisCode ==
                        claim.DiagnosisCode
                    && x.Status ==
                        ClaimStatus.Rejected
                    && x.CreatedAt >= since);

        return exists ? 15 : 0;
    }
}