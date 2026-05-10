using MediClaim.Application.Common.Models;

namespace MediClaim.Application.Common.Interfaces
{
    public interface IFraudScoringService
    {
        Task<FraudEvaluationResult>
       EvaluateAsync(
           MediClaim.Domain.Entities.Claim claim);
    }
}
