using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Application
    .Common.Models;
using MediClaim.Domain.Entities;

namespace MediClaim.Infrastructure.BackgroundJobs;


public class FraudScoringService
    : IFraudScoringService
{
    private readonly IEnumerable<
        IFraudRuleEvaluator>
            _evaluators;

    public FraudScoringService(
        IEnumerable<
            IFraudRuleEvaluator>
                evaluators)
    {
        _evaluators = evaluators;
    }

    public async Task<
        FraudEvaluationResult>
            EvaluateAsync(
                Claim claim, CancellationToken cancellationToken)
    {
        var totalScore = 0;

        var triggeredRules =
            new List<string>();

        foreach (var evaluator
            in _evaluators)
        {
            var score =
                await evaluator
                    .EvaluateAsync(
                        claim, cancellationToken);

            totalScore += score;

            if (score > 0)
            {
                triggeredRules
                    .Add(
                        evaluator
                            .RuleName);
            }
        }

        return new FraudEvaluationResult
        {
            Score = totalScore,

            RequiresReview =
                totalScore >= 60,

            TriggeredRules =
                triggeredRules
        };
    }
}