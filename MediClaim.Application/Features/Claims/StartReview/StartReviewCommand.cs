using MediatR;

namespace MediClaim.Application
    .Features.Claims.StartReview;

public class StartReviewCommand
    : IRequest
{
    public Guid ClaimId
    {
        get; set;
    }
}