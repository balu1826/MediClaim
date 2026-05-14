using MediatR;
using Microsoft.AspNetCore.Http;

namespace MediClaim.Application.Features.Claims.UploadDocument;

public class UploadClaimDocumentCommand
    : IRequest
{
    public Guid ClaimId { get; set; }
    public IFormFile File { get; set; } = default!;
}