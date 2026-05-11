using MediatR;

namespace MediClaim.Application
    .Features.Claims.SettleClaim;

public class SettleClaimCommand
    : IRequest
{
    public Guid ClaimId
    {
        get; set;
    }
}