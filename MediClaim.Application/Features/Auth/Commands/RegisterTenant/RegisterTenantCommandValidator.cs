using FluentValidation;

namespace MediClaim.Application
    .Features.Auth.Commands.RegisterTenant;

public class RegisterTenantCommandValidator
    : AbstractValidator<RegisterTenantCommand>
{
    public RegisterTenantCommandValidator()
    {
        RuleFor(x => x.TenantName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.AdminEmail)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]")
            .WithMessage(
                "Password must contain uppercase letter")
            .Matches("[0-9]")
            .WithMessage(
                "Password must contain digit")
            .Matches("[^a-zA-Z0-9]")
            .WithMessage(
                "Password must contain special character");
        RuleFor(x => x.Ssn)
            .NotEmpty()
            .Length(9);
    }
}