using MediatR;
using MediClaim.Domain.Enums;

namespace MediClaim.Application
    .Features.Claims.TransitionClaim;

public class TransitionClaimCommand : IRequest
{
    public Guid ClaimId
    {
        get;
        set;
    }

    public ClaimStatus NewStatus
    {
        get;
        set;
    }

    public string? RejectionReason
    {
        get;
        set;
    }
}