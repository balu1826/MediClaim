using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Domain.Entities;
using MediClaim.Domain.Enums;
using MediClaim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Infrastructure
    .FraudDetection.Rules;

public class ExcessiveAmountRuleEvaluator
    : IFraudRuleEvaluator
{
    private readonly ApplicationDbContext
        _context;

    public ExcessiveAmountRuleEvaluator(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public string RuleName =>
        nameof(
            ExcessiveAmountRuleEvaluator);

    public async Task<int>
        EvaluateAsync(
            Claim claim)
    {
        var approvedAmounts =
            await _context.Claims
                .Where(x =>
                    x.TenantId ==
                        claim.TenantId
                    && x.DiagnosisCode ==
                        claim.DiagnosisCode
                    && x.Status ==
                        ClaimStatus.Approved
                    && x.ApprovedAmount != null)
                .Select(x =>
                    x.ApprovedAmount!.Value)
                .OrderBy(x => x)
                .ToListAsync();

        if (!approvedAmounts.Any())
        {
            return 0;
        }

        var median =
            approvedAmounts[
                approvedAmounts.Count / 2];

        return claim.Amount >
            (median * 1.5m)
                ? 20
                : 0;
    }
}