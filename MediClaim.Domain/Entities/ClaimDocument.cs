namespace MediClaim.Domain.Entities;

public class ClaimDocument
{
    public Guid ClaimDocumentId { get; set; }

    public Guid ClaimId { get; set; }

    public string FileName { get; set; } = default!;

    public string ContentType { get; set; } = default!;

    public string FilePath { get; set; } = default!;

    public DateTime UploadedAt { get; set; }

    public Claim Claim { get; set; } = default!;
}