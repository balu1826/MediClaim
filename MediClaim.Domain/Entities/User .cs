using MediClaim.Domain.Common;
using MediClaim.Domain.Enums;

namespace MediClaim.Domain.Entities;

public class User : TenantEntity
{
    public Guid UserId { get; set; }

    public string Email { get; set; } = default!;

    public string PasswordHash { get; set; } = default!;

    public UserRole Role { get; set; }

    public decimal? ApprovalLimit { get; set; }

    public int FailedLoginCount { get; set; }

    public bool IsLocked { get; set; }

    public DateTime? LockedAt { get; set; }

    public bool IsFraudSpecialist { get; set; }

    public bool IsOnLeave { get; set; }
    public byte[] SsnEncrypted { get; set; } = default!;
}