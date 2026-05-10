using MediatR;
using MediClaim.Domain.Enums;

namespace MediClaim.Application
    .Features.Users.Commands.CreateUser;

public class CreateUserCommand
    : IRequest<Guid>
{
    public string Email { get; set; }
        = default!;

    public string Password { get; set; }
        = default!;

    public UserRole Role { get; set; }

    public decimal? ApprovalLimit
    {
        get; set;
    }

    public bool IsFraudSpecialist
    {
        get; set;
    }

    public string Ssn { get; set; }
        = default!;
}