using MediClaim.Application
    .Common.Exceptions;
using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Domain.Entities;
using MediClaim.Domain.Enums;
using MediClaim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace MediClaim.Infrastructure
    .Policies;

public class PolicyUpgradeService
    : IPolicyUpgradeService
{
    private readonly ApplicationDbContext
        _context;

    public PolicyUpgradeService(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task UpgradeAsync(
        Policy policy,
        PolicyType newPolicyType,
        CancellationToken cancellationToken)
    {
        var oldPolicyType =
            await _context.PolicyTypes
                .FirstAsync(
                    x =>
                        x.PolicyTypeId ==
                            policy.PolicyTypeId,
                    cancellationToken);

        // Remaining days

        var upgradeDate =
      DateTime.UtcNow;

        var remainingDays =
            policy.EndDate.DayNumber -
            DateOnly
                .FromDateTime(
                    upgradeDate)
                .DayNumber;

        if (remainingDays <= 0)
        {
            throw new BadRequestException(
                "Policy already expired");
        }

        // Formula inputs

        var oldLimit =
            oldPolicyType.AnnualCoverageLimit;

        var newLimit =
            newPolicyType.AnnualCoverageLimit;

        var delta =
            newLimit - oldLimit;

        var proratedDelta =
            delta *
            ((decimal)remainingDays / 365m);

        var oldRemaining =
            policy.RemainingLimit;

        var newRemaining =
            oldRemaining + proratedDelta;

        // Pending liability validation

        var pendingExposure =
            await _context.Claims
                .Where(x =>
                    x.PolicyId ==
                        policy.PolicyId
                    && (
                        x.Status ==
                            ClaimStatus.Submitted
                        || x.Status ==
                            ClaimStatus.UnderReview))
                .SumAsync(
                    x => x.Amount,
                    cancellationToken);

        if (newRemaining < pendingExposure)
        {
            throw new ConflictException(
                "Upgrade would reduce remaining coverage below pending liabilities");
        }

        // Upgrade

        policy.PolicyTypeId =
            newPolicyType.PolicyTypeId;

        policy.AnnualLimit =
            newLimit;

        policy.RemainingLimit =
            decimal.Round(
                newRemaining,
                2);

        // Regulatory traceability

        var metadata =
            new
            {
                OldLimit = oldLimit,
                NewLimit = newLimit,
                RemainingDays = remainingDays,
                Delta = delta,
                ProratedDelta = proratedDelta,
                OldRemaining = oldRemaining,
                NewRemaining = newRemaining
            };
    }
}