using MediClaim.Domain.Enums;

namespace MediClaim.Application
    .Features.Claims.Common;

public class OfficerClaimQueueDto
{
    public Guid ClaimId
    {
        get; set;
    }

    public string PolicyNumber
    {
        get; set;
    } = default!;

    public decimal Amount
    {
        get; set;
    }

    public string DiagnosisCode
    {
        get; set;
    } = default!;

    public string TreatmentCategory
    {
        get; set;
    } = default!;

    public ClaimStatus Status
    {
        get; set;
    }

    public int FraudRiskScore
    {
        get; set;
    }

    public bool RequiresFraudReview
    {
        get; set;
    }

    public DateTime SubmittedAt
    {
        get; set;
    }
}