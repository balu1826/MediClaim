using FluentValidation;

namespace MediClaim.Application
    .Features.Claims.CreateDraftClaim;

public class CreateDraftClaimCommandValidator
    : AbstractValidator<
        CreateDraftClaimCommand>
{
    public CreateDraftClaimCommandValidator()
    {
        RuleFor(x => x.PolicyId)
            .NotEmpty();

        RuleFor(x => x.Amount)
            .GreaterThan(0);

        RuleFor(x => x.DiagnosisCode)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.TreatmentCategory)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(1000);
        RuleFor(x => x.ProviderId)
            .NotNull();
    }
}