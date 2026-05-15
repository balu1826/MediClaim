namespace MediClaim.Application.Features.Policies.GetPolicyDetails;

public class ClaimHistoryDto
{
    public Guid ClaimId { get; set; }

    public decimal Amount { get; set; }

    public string Status { get; set; } = default!;

    public DateTime CreatedAt { get; set; }
}