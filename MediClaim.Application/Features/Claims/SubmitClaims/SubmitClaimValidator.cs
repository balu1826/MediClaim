using FluentValidation;
using MediClaim.Application.Features.Claims.SubmitClaim;

namespace MediClaim.Application.Features.Claims.SubmitClaims
{
    public class SubmitClaimValidator
     : AbstractValidator<SubmitClaimCommand>
    {
        public SubmitClaimValidator()
        {
            RuleFor(x => x.ClaimId)
                .NotEmpty();
        }
    }
}
