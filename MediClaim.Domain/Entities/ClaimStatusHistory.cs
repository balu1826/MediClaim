namespace MediClaim.Domain.Entities;

public class ClaimStatusHistory
{
    public Guid ClaimStatusHistoryId
    {
        get; set;
    }

    public Guid ClaimId
    {
        get; set;
    }

    public string Status
    {
        get; set;
    } = default!;

    public DateTime ChangedAt
    {
        get; set;
    }

    public string? Notes
    {
        get; set;
    }

    // Navigation

    public Claim Claim
    {
        get; set;
    } = default!;
}