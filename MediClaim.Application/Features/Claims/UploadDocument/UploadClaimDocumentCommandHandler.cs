using MediatR;
using MediClaim.Application.Common.Interfaces;
using MediClaim.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using MediClaim.Application.Repositories;
using MediClaim.Application.Common.Exceptions;

namespace MediClaim.Application.Features.Claims.UploadDocument;

public class UploadClaimDocumentCommandHandler
    : IRequestHandler<UploadClaimDocumentCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _environment;
    private readonly IClaimRepository _claimRepository;
    private readonly IUserRepository _currentUserService;
    public UploadClaimDocumentCommandHandler(
        IApplicationDbContext context,
        IUnitOfWork unitOfWork,
        IWebHostEnvironment environment,
        IClaimRepository claimRepository,
        IUserRepository currentUserService)
    {
        _context = context;
        _unitOfWork = unitOfWork;
        _environment = environment;
        _claimRepository = claimRepository;
        _currentUserService = currentUserService;
    }

    public async Task Handle(
        UploadClaimDocumentCommand request,
        CancellationToken cancellationToken)
    {
        var userId =  _currentUserService.UserId;
          
        var claim =
            await _claimRepository.GetClaimByIdAsync(
                userId,
                request.ClaimId,
                cancellationToken);
        if(claim  == null) {
            throw new NotFoundException("Claim not found");
        }

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