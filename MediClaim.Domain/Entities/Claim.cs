using MediClaim.Domain.Common;
using MediClaim.Domain.Enums;


namespace MediClaim.Domain.Entities;

public class Claim
    : BaseEntity
{
    public Guid ClaimId { get; set; }

    public Guid TenantId { get; set; }

    public Guid UserId { get; set; }

    public decimal Amount { get; set; }

    public string DiagnosisCode { get; set; }
        = default!;

    public string Description { get; set; }
        = default!;

    public ClaimStatus Status { get; set; }

    public User User { get; set; }
        = default!;

    public Tenant Tenant { get; set; }
        = default!;
    public Guid PolicyId
    {
        get; set;
    }

    public string TreatmentCategory
    {
        get; set;
    } = default!;

    public Policy Policy
    {
        get; set;
    } = default!;

    public int FraudRiskScore
    {
        get; set;
    }

    public bool RequiresFraudReview
    {
        get; set;
    }
    public Guid? ProviderId
    {
        get; set;
    }

    public DateOnly TreatmentDate
    {
        get; set;
    }

    public decimal? ApprovedAmount
    {
        get; set;
    }
    public Guid? AssignedOfficerId
    {
        get; set;
    }

    public User? AssignedOfficer
    {
        get; set;
    }

    public bool PendingAssignment
    {
        get; set;
    }
    public string? RejectionReason
    {
        get; set;
    }

    public DateTime? ReviewedAt
    {
        get; set;
    }
    public DateTime SubmittedAt { get; set; }
    public Provider Provider
    {
        get; set;
    } = default!;
    public void Submit()
    {
        if (Status != ClaimStatus.Draft)
        {
            throw new UnprocessableEntityException(
                "Only draft claims can be submitted");
        }

        Status = ClaimStatus.Submitted;
    }
    public void StartReview()
    {
        if (Status != ClaimStatus.Submitted)
        {
            throw new InvalidOperationException(
                "Only submitted claims can be reviewed");
        }

        Status = ClaimStatus.UnderReview;
    }

    public void Approve()
    {
        if (Status != ClaimStatus.UnderReview)
        {
            throw new InvalidOperationException(
                "Only claims under review can be approved");
        }

        Status = ClaimStatus.Approved;
    }

    public void Reject(string reason)
    {
        if (Status != ClaimStatus.UnderReview)
        {
            throw new InvalidOperationException(
                "Only claims under review can be rejected");
        }
        RejectionReason = reason;
        Status = ClaimStatus.Rejected;
        ReviewedAt = DateTime.UtcNow;
    }

    public void Escalate()
    {
        if (Status != ClaimStatus.UnderReview)
        {
            throw new InvalidOperationException(
                "Only claims under review can be escalated");
        }

        Status = ClaimStatus.Escalated;
    }

    public void Withdraw()
    {
        if (Status == ClaimStatus.Settled)
        {
            throw new InvalidOperationException(
                "Settled claims cannot be withdrawn");
        }

        Status = ClaimStatus.Withdrawn;
    }
}