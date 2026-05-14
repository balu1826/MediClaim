using MediClaim.Application
    .Common.Interfaces;

using MediClaim.Domain.Entities;
using MediClaim.Domain.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MediClaim.Infrastructure
    .BackgroundJobs;

public class WeeklyClaimsSummaryJob
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<WeeklyClaimsSummaryJob> _logger;
    public WeeklyClaimsSummaryJob(
        IApplicationDbContext context,
        IUnitOfWork unitOfWork,
        ILogger<WeeklyClaimsSummaryJob> logger)
    {
        _context = context;
        _unitOfWork = unitOfWork;
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
                        WeeklyClaimsSummaryJob),

                StartedAt =
                    DateTime.UtcNow,

                Status =
                    "Running"
            };

        await _context
            .JobExecutionLogs
            .AddAsync(
                executionLog,
                cancellationToken);

        await _unitOfWork
            .SaveChangesAsync(
                cancellationToken);

        try
        {
            var tenants =
                await _context.Tenants
                    .ToListAsync(
                        cancellationToken);
            var reportsGenerated = 0;
            foreach (var tenant in tenants)
            {
                var claims =
                    await _context.Claims
                        .Where(x => x.TenantId == tenant.TenantId)
                        .ToListAsync(cancellationToken);
                if (!claims.Any())
                {
                    continue;
                }
                var totalClaims = claims.Count;
                var approvedClaims =
                    claims.Count(x =>
                        x.Status ==
                            ClaimStatus
                                .Approved);

                var rejectedClaims =
                    claims.Count(x =>
                        x.Status ==
                            ClaimStatus
                                .Rejected);

                var rejectionRate =
                    totalClaims == 0
                    ? 0
                    : (decimal)
                        rejectedClaims
                        / totalClaims;

                var processedClaims = claims.Where(x =>
                                                  x.Status == ClaimStatus.Settled
                                                  ||
                                                  x.Status == ClaimStatus.Rejected);                      
                decimal averageProcessingHours = 0;
                if (processedClaims.Any())
                {
                    averageProcessingHours =
                        (decimal)
                            processedClaims
                                .Average(x =>
                                    (
                                        x.UpdatedAt!
                                        - x.SubmittedAt)
                                    .TotalHours);
                }

                var reportContent =
                    $"""
                    Weekly Claims Summary

                    Tenant:
                    {tenant.Name}

                    Total Claims:
                    {totalClaims}

                    Approved Claims:
                    {approvedClaims}

                    Rejected Claims:
                    {rejectedClaims}

                    Rejection Rate:
                    {rejectionRate:P}

                    Average Processing Hours:
                    {averageProcessingHours:F2}
                    """;

                var report =
                    new Report
                    {
                        ReportId =
                            Guid.NewGuid(),

                        TenantId =
                            tenant.TenantId,

                        ReportType =
                            "Weekly",

                        TotalClaims =
                            totalClaims,

                        ApprovedClaims =
                            approvedClaims,

                        RejectionRate =
                            rejectionRate,

                        AverageProcessingTimeHours =
                            averageProcessingHours,

                        ReportContent =
                            reportContent,

                        GeneratedAt =
                            DateTime.UtcNow
                    };

                await _context
                    .Reports
                    .AddAsync(
                        report,
                        cancellationToken);

                // TODO:
                // Send email
                // to TenantAdmins

                reportsGenerated++;
            }

            executionLog.Status =
                "Succeeded";

            executionLog.FinishedAt =
                DateTime.UtcNow;

            executionLog.RecordsAffected =
                reportsGenerated;

            await _unitOfWork
                .SaveChangesAsync(
                    cancellationToken);

            _logger.LogInformation(
                "Weekly summary job completed");
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
                "Weekly summary job failed");
            throw;
        }
    }
}