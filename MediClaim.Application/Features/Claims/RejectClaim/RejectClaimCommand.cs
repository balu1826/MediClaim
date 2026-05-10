using MediatR;

namespace MediClaim.Application
    .Features.Claims.RejectClaim;

public class RejectClaimCommand
    : IRequest
{
    public Guid ClaimId
    {
        get; set;
    }

    public string Reason
    {
        get; set;
    } = default!;
}