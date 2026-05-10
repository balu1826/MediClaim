using MediatR;
using MediClaim.Application
    .Common.Exceptions;
using MediClaim.Application
    .Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Application
    .Features.Claims.RejectClaim;

public class RejectClaimCommandHandler
    : IRequestHandler<
        RejectClaimCommand>
{
    private readonly IApplicationDbContext
        _context;

    private readonly IUserRepository
        _currentUserService;

    private readonly IUnitOfWork
        _unitOfWork;

    public RejectClaimCommandHandler(
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
        RejectClaimCommand request,
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

        if (claim.AssignedOfficerId
            != officerId)
        {
            throw new ForbiddenAccessException(
                "Claim not assigned");
        }

        claim.Reject(
            request.Reason);

        await _unitOfWork
            .SaveChangesAsync(
                cancellationToken);
    }
}