using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Domain.Entities;
using MediClaim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MediClaim.Application.Common.Exceptions;

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
    public string RuleName => GetType().Name;
    public async Task<int> EvaluateAsync(
            Claim claim,
            CancellationToken cancellationToken)
    {
        if (claim.ProviderId is null)
        {
            throw new BadRequestException("ProviderId is required.");
        }
        var provider =
            await _context.Providers
                .SingleAsync(
                    x =>
                        x.ProviderId ==
                            claim.ProviderId);
        var day = claim.TreatmentDate.DayOfWeek;
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