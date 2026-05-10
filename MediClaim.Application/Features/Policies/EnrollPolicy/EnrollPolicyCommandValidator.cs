using FluentValidation;

namespace MediClaim.Application
    .Features.Policies.EnrollPolicy;

public class EnrollPolicyCommandValidator
    : AbstractValidator<
        EnrollPolicyCommand>
{
    public EnrollPolicyCommandValidator()
    {
        RuleFor(x => x.PatientId)
            .NotEmpty();

        RuleFor(x => x.PolicyTypeId)
            .NotEmpty();

        RuleFor(x => x.StartDate)
            .LessThan(x => x.EndDate);
    }
}