using MediatR;

namespace MediClaim.Application
    .Features.Policies
    .UpgradePolicy;

public class UpgradePolicyCommand
    : IRequest
{
    public Guid PolicyId
    {
        get; set;
    }

    public Guid NewPolicyTypeId
    {
        get; set;
    }
}