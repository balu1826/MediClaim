using MediClaim.Domain.Entities;

namespace MediClaim.Application
    .Common.Interfaces;

public interface IPolicyUpgradeService
{
    Task UpgradeAsync(
        Policy policy,
        PolicyType newPolicyType,
        CancellationToken cancellationToken);
}
