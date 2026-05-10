using FluentValidation;
using MediClaim.Domain.Enums;

namespace MediClaim.Application
    .Features.Users.Commands.CreateUser;

public class CreateUserCommandValidator
    : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .MinimumLength(8);

        RuleFor(x => x.Ssn)
            .NotEmpty();

        RuleFor(x => x.Role)
            .Must(x =>
                x == UserRole.Patient
                || x == UserRole.ClaimsOfficer)
            .WithMessage(
                "Only Patient or ClaimsOfficer can be created");

        RuleFor(x => x.ApprovalLimit)
            .NotNull()
            .GreaterThan(0)
            .When(x =>
                x.Role ==
                UserRole.ClaimsOfficer);
    }
}