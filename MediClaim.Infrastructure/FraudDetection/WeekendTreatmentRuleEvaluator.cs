using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Domain.Entities;
using MediClaim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Infrastructure
    .FraudDetection.Rules;

public class WeekendTreatmentRuleEvaluator
    : IFraudRuleEvaluator
{
    private readonly ApplicationDbContext
        _context;

    public WeekendTreatmentRuleEvaluator(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public string RuleName =>
        nameof(
            WeekendTreatmentRuleEvaluator);

    public async Task<int>
        EvaluateAsync(
            Claim claim, CancellationToken cancellationToken)
    {
        if (claim.ProviderId is null)
        {
            return 0;
        }

        var provider =
            await _context.Providers
                .FirstOrDefaultAsync(
                    x =>
                        x.ProviderId ==
                            claim.ProviderId);

        if (provider is null)
        {
            return 0;
        }

        var day =
            claim.TreatmentDate
                .DayOfWeek;

        var weekend =
            day == DayOfWeek.Saturday
            || day == DayOfWeek.Sunday;

        var nonEmergency =
            provider.Specialty !=
                "Emergency";

        return weekend && nonEmergency
            ? 20
            : 0;
    }
}