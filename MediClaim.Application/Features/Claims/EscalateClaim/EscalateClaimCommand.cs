using MediatR;

namespace MediClaim.Application
    .Features.Claims.EscalateClaim;

public class EscalateClaimCommand
    : IRequest
{
    public Guid ClaimId
    {
        get; set;
    }
}