using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Domain.Entities;
using MediClaim.Domain.Enums;
using MediClaim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Infrastructure
    .Workflow;

public class ClaimAssignmentService
    : IClaimAssignmentService
{
    private readonly ApplicationDbContext
        _context;

    public ClaimAssignmentService(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AssignAsync(
        Claim claim,
        CancellationToken cancellationToken)
    {
        await using var transaction =
            await _context.Database
                .BeginTransactionAsync(
                    cancellationToken);

        var requiresFraudSpecialist =
            claim.FraudRiskScore >= 60;

        // Eligible pool

        var eligibleOfficers =
            await _context.Users
                .FromSqlRaw(@"
SELECT *
FROM Users WITH (UPDLOCK, ROWLOCK)
WHERE TenantId = {0}
AND Role = {1}
AND IsActive = 1
AND IsOnLeave = 0",
                    claim.TenantId,
                    (int)UserRole.ClaimsOfficer)
                .Where(x =>
                    !requiresFraudSpecialist
                    || x.IsFraudSpecialist)
                .ToListAsync(
                    cancellationToken);

        if (!eligibleOfficers.Any())
        {
            claim.PendingAssignment =
                true;

            await _context
                .SaveChangesAsync(
                    cancellationToken);

            await transaction
                .CommitAsync(
                    cancellationToken);

            return;
        }

        // Workload balancing

        var officerWorkloads =
            await _context.Claims
                .Where(x =>
                    x.AssignedOfficerId != null
                    && (x.Status ==
                        ClaimStatus.UnderReview
                     || x.Status ==
                        ClaimStatus.PendingDocuments))
                .GroupBy(x => x.AssignedOfficerId)
                .Select(x =>
                    new
                    {
                        OfficerId = x.Key,
                        Count = x.Count()
                    })
                .ToListAsync(
                    cancellationToken);

        var selectedOfficer = eligibleOfficers
                                 .OrderBy(x => officerWorkloads.FirstOrDefault(w => w.OfficerId == x.UserId)?.Count ?? 0)
                                 .ThenBy(x => x.LastAssignedAt ?? DateTime.MinValue)
                                 .First();

        // Assign
        claim.AssignedOfficerId =
            selectedOfficer.UserId;

        claim.PendingAssignment =
            false;

        selectedOfficer.LastAssignedAt =
            DateTime.UtcNow;

        await _context
            .SaveChangesAsync(
                cancellationToken);

        await transaction
            .CommitAsync(
                cancellationToken);
    }
}