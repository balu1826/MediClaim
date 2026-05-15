using MediatR;

namespace MediClaim.Application.Features.Policies.RenewPolicy;

public class RenewPolicyCommand
    : IRequest<Guid>
{
    public Guid PolicyId { get; set; }
}