using MediClaim.Application.Features.Claims.Common;
using MediClaim.Application.Features.Claims.GetFraudFlags;

namespace MediClaim.Application.Repositories
{
    public interface IClaimRepository
    {
        Task<Domain.Entities.Claim?> GetClaimByIdAsync(
            Guid claimId,
            Guid tenantId,
            CancellationToken cancellationToken);
        Task AddClaimAsync(
            Domain.Entities.Claim claim,
            CancellationToken cancellationToken);
        Task<List<ClaimDto>> GetMyClaimsAsync(
            Guid userId,
            Guid tenantId,
            CancellationToken cancellationToken);
       Task<List<OfficerClaimQueueDto>>  GetOfficerQueueAsync(
        Guid tenantId,
        Guid officerId,
        CancellationToken cancellationToken);
       Task<List<FraudFlagDto>> GetFraudFlagsAsync(
        Guid tenantId,
        int threshold,
        CancellationToken cancellationToken);
    }
}
