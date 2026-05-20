using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Domain.Entities;
using MediClaim.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MediClaim.Infrastructure
    .BackgroundJobs;

public class FraudScoreRecalculationJob
{
    private readonly IApplicationDbContext _context;
    private readonly IFraudScoringService _fraudScoringService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FraudScoreRecalculationJob> _logger;
    public FraudScoreRecalculationJob(
        IApplicationDbContext context,
        IFraudScoringService fraudScoringService,
        IUnitOfWork unitOfWork,
        ILogger<FraudScoreRecalculationJob> logger)
    {
        _context = context;
        _fraudScoringService = fraudScoringService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var executionLog = new JobExecutionLog
        {
                JobName = nameof(FraudScoreRecalculationJob),
                StartedAt = DateTime.UtcNow,
                Status = "Running"
        };

        await _context.JobExecutionLogs.AddAsync(
                executionLog,
                cancellationToken);
        await _unitOfWork.SaveChangesAsync(
                cancellationToken);
        try
        {
            const int batchSize = 50;
            var totalProcessed = 0;
            var page = 0;
            while (true)
            {
                var claims =
                    await _context.Claims
                        .Where(
                            x => x.Status == ClaimStatus.Draft || x.Status == ClaimStatus.PendingDocuments)
                        .OrderBy(x => x.CreatedAt)
                        .Skip(page * batchSize)
                        .Take(batchSize)
                        .ToListAsync(cancellationToken);
                if (!claims.Any())
                {
                    break;
                }

                foreach (var claim in claims)
                {
                    var fraudResult = await _fraudScoringService
                            .EvaluateAsync(claim, cancellationToken);
                    claim.FraudRiskScore = fraudResult.Score;
                    claim.RequiresFraudReview = fraudResult.RequiresReview;
                }
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                totalProcessed += claims.Count;
                page++;
            }

            executionLog.Status = "Succeeded";
            executionLog.FinishedAt = DateTime.UtcNow;
            executionLog.RecordsAffected = totalProcessed;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Recalculated fraud scores for {Count} claims", totalProcessed);
        }
        catch (Exception ex)
        {
            executionLog.Status = "Failed";
            executionLog.ErrorMessage = ex.Message;
            executionLog.FinishedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            _logger.LogError(ex, "Fraud score recalculation job failed");
            throw;
        }
    }
}