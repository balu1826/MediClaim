namespace MediClaim.Domain.Enums;

public enum ClaimStatus
{
    Draft = 1,
    Submitted = 2,
    UnderReview = 3,
    PendingDocuments = 4,
    Escalated = 5,
    Approved = 6,
    Rejected = 7,
    Settled = 8,
    Withdrawn = 9
}