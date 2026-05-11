using MediClaim.Domain.Common;

namespace MediClaim.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid TokenId { get; set; }

    public Guid UserId { get; set; }

    public Guid FamilyId { get; set; }

    public string TokenHash { get; set; } = default!;

    public bool IsUsed { get; set; }

    public bool IsRevoked { get; set; }

    public DateTime ExpiresAt { get; set; }

    public string? CreatedByIp { get; set; }

    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByTokenHash
    {
        get; set;
    }

    public string? RevokedReason
    {
        get; set;
    }

    public string? UserAgent
    {
        get; set;
    }
}