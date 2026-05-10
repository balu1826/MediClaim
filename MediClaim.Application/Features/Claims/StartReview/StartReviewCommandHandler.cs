using MediatR;
using MediClaim.Application
    .Common.Exceptions;
using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Application
    .Features.Claims.StartReview;

public class StartReviewCommandHandler
    : IRequestHandler<
        StartReviewCommand>
{
    private readonly IApplicationDbContext
        _context;

    private readonly IUserRepository
        _currentUserService;

    private readonly IUnitOfWork
        _unitOfWork;

    public StartReviewCommandHandler(
        IApplicationDbContext context,
        IUserRepository currentUserService,
        IUnitOfWork unitOfWork)
    {
        _context = context;

        _currentUserService =
            currentUserService;

        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        StartReviewCommand request,
        CancellationToken cancellationToken)
    {
        var officerId =
            _currentUserService
                .UserId;

        var tenantId =
            _currentUserService
                .TenantId;

        var claim =
            await _context.Claims
                .FirstOrDefaultAsync(
                    x =>
                        x.ClaimId ==
                            request.ClaimId
                        && x.TenantId ==
                            tenantId,
                    cancellationToken);

        if (claim is null)
        {
            throw new NotFoundException(
                "Claim not found");
        }

        // Authorization

        if (claim.AssignedOfficerId
            != officerId)
        {
            throw new ForbiddenAccessException(
                "Claim not assigned to officer");
        }

        // Domain transition

        claim.StartReview();

        await _unitOfWork
            .SaveChangesAsync(
                cancellationToken);
    }
}