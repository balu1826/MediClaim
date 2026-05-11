using MediClaim.Application
    .Common.Models;

namespace MediClaim.Application
    .Common.Interfaces;

public interface IClaimSettlementService
{
    Task<SettlementResult>
        SettleAsync(
            Guid claimId,
            decimal approvedAmount,
            Guid policyId,
            CancellationToken cancellationToken);
}