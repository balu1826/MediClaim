using MediatR;
using MediClaim.Application.Common.Interfaces;
using MediClaim.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Application.Features.Claims.UploadDocument;

public class UploadClaimDocumentCommandHandler
    : IRequestHandler<UploadClaimDocumentCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _environment;
    public UploadClaimDocumentCommandHandler(
        IApplicationDbContext context,
        IUnitOfWork unitOfWork,
        IWebHostEnvironment environment)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _environment = environment;
    }

    public async Task Handle(
        UploadClaimDocumentCommand request,
        CancellationToken cancellationToken)
    {
        var claim =
            await _context.Claims
                .SingleAsync(
                    x => x.ClaimId == request.ClaimId,
                    cancellationToken);

        var uploadsFolder =
            Path.Combine(
                _environment.ContentRootPath,
                "Uploads");

        Directory.CreateDirectory(uploadsFolder);

        var fileName =
            $"{Guid.NewGuid()}_{request.File.FileName}";

        var filePath =
            Path.Combine(
                uploadsFolder,
                fileName);

        using var stream =
            new FileStream(
                filePath,
                FileMode.Create);

        await request.File.CopyToAsync(
            stream,
            cancellationToken);

        var document =
            new ClaimDocument
            {
                ClaimDocumentId = Guid.NewGuid(),
                ClaimId = claim.ClaimId,
                FileName = request.File.FileName,
                ContentType = request.File.ContentType,
                FilePath = filePath,
                UploadedAt = DateTime.UtcNow
            };

        await _context.ClaimDocuments
            .AddAsync(document, cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);
    }
}