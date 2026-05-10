using MediatR;

namespace MediClaim.Application
    .Features.Claims.ApproveClaim;

public class ApproveClaimCommand
    : IRequest
{
    public Guid ClaimId
    {
        get; set;
    }
}