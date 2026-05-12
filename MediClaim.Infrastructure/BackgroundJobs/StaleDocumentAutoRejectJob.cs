using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Domain.Entities;
using MediClaim.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MediClaim.Infrastructure
    .BackgroundJobs;

public class StaleDocumentAutoRejectJob
{
    private readonly IApplicationDbContext
        _context;

    private readonly IUnitOfWork
        _unitOfWork;

    private readonly ILogger<
        StaleDocumentAutoRejectJob>
            _logger;

    public StaleDocumentAutoRejectJob(
        IApplicationDbContext context,
        IUnitOfWork unitOfWork,
        ILogger<
            StaleDocumentAutoRejectJob>
                logger)
    {
        _context = context;

        _unitOfWork =
            unitOfWork;

        _logger = logger;
    }

    public async Task ExecuteAsync(
        CancellationToken cancellationToken)
    {
        var executionLog =
            new JobExecutionLog
            {
                JobName =
                    nameof(
                        StaleDocumentAutoRejectJob),

                StartedAt =
                    DateTime.UtcNow,

                Status =
                    "Running"
            };

        await _context.JobExecutionLogs
            .AddAsync(
                executionLog,
                cancellationToken);

        await _unitOfWork
            .SaveChangesAsync(
                cancellationToken);

        try
        {
            var cutoffDate =
                DateTime.UtcNow
                    .AddDays(-7);

            var staleClaims =
                await _context.Claims
                    .Where(
                        x =>
                            x.Status ==
                                ClaimStatus
                                    .PendingDocuments
                            &&
                            x.UpdatedAt <
                                cutoffDate)
                    .ToListAsync(
                        cancellationToken);

            foreach (var claim
                in staleClaims)
            {
                // IDEMPOTENCY CHECK

                if (claim.Status !=
                    ClaimStatus
                        .PendingDocuments)
                {
                    continue;
                }

                claim.Status =
                    ClaimStatus
                        .Rejected;

                claim.RejectionReason =
                    "STALE_DOCUMENTS";

                claim.UpdatedAt =
                    DateTime.UtcNow;

                await _context
                    .ClaimStatusHistories
                    .AddAsync(
                        new ClaimStatusHistory
                        {
                            ClaimStatusHistoryId =
                                Guid.NewGuid(),

                            ClaimId =
                                claim.ClaimId,

                            Status =
                                ClaimStatus
                                    .Rejected
                                    .ToString(),

                            ChangedAt =
                                DateTime.UtcNow,

                            Notes =
                                "Auto-rejected due to stale documents"
                        },
                        cancellationToken);

                // TODO:
                // notify patient
            }

            await _unitOfWork
                .SaveChangesAsync(
                    cancellationToken);

            executionLog.Status =
                "Succeeded";

            executionLog.FinishedAt =
                DateTime.UtcNow;

            executionLog.RecordsAffected =
                staleClaims.Count;

            await _unitOfWork
                .SaveChangesAsync(
                    cancellationToken);

            _logger.LogInformation(
                "Processed {Count} stale claims",
                staleClaims.Count);
        }
        catch (Exception ex)
        {
            executionLog.Status =
                "Failed";

            executionLog.ErrorMessage =
                ex.Message;

            executionLog.FinishedAt =
                DateTime.UtcNow;

            await _unitOfWork
                .SaveChangesAsync(
                    cancellationToken);

            _logger.LogError(
                ex,
                "Stale document job failed");

            throw;
        }
    }
}