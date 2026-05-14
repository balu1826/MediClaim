namespace MediClaim.Application.Features.Claims.GetFraudFlags;

public class FraudFlagDto
{
    public Guid ClaimId { get; set; }

    public int FraudScore { get; set; }

    public decimal Amount { get; set; }

    public string DiagnosisCode { get; set; } = default!;
}