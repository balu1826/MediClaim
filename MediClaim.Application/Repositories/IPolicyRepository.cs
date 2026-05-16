using MediClaim.Domain.Entities;

namespace MediClaim.Application.Repositories
{
    public interface IPolicyRepository
    {
        Task<Policy?> GetPolicyByIdAsync(
          Guid policyId,
          CancellationToken cancellationToken);
        Task<PolicyType?> GetPolicyTypeByIdAsync(
          Guid policyTypeId,
          CancellationToken cancellationToken);
    }
}
