using MediClaim.Domain.Entities;

namespace MediClaim.Application
    .Common.Interfaces;

public interface IFraudRuleEvaluator
{
    string RuleName
    {
        get;
    }
    Task<int> EvaluateAsync(
        Claim claim);
}