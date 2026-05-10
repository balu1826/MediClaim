using FluentValidation;

namespace MediClaim.Application
    .Features.Policies.CreatePolicyType;

public class CreatePolicyTypeCommandValidator
    : AbstractValidator<
        CreatePolicyTypeCommand>
{
    public CreatePolicyTypeCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x =>
            x.AnnualCoverageLimit)
            .GreaterThan(0);

        RuleFor(x =>
            x.DeductibleAmount)
            .GreaterThanOrEqualTo(0);
        RuleFor(x =>
            x.CoverageCategories)
        .NotEmpty();
        RuleForEach(x =>
                x.CoverageCategories)
            .NotEmpty()
            .MaximumLength(100);
    }
}