using MediClaim.Domain.Enums;

namespace MediClaim.Application
    .Features.Users.Common;

public class UserDto
{
    public Guid UserId { get; set; }

    public string Email { get; set; }
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

    public bool IsLocked
    {
        get; set;
    }
}