using MediatR;

namespace MediClaim.Application
    .Features.Claims.SubmitClaim;

public class SubmitClaimCommand
    : IRequest
{
    public Guid ClaimId
    {
        get; set;
    }
}