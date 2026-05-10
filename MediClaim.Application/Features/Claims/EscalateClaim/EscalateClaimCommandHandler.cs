using MediatR;
using MediClaim.Application
    .Common.Exceptions;
using MediClaim.Application
    .Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Application
    .Features.Claims.EscalateClaim;

public class EscalateClaimCommandHandler
    : IRequestHandler<
        EscalateClaimCommand>
{
    private readonly IApplicationDbContext
        _context;

    private readonly IUserRepository
        _currentUserService;

    private readonly IUnitOfWork
        _unitOfWork;

    public EscalateClaimCommandHandler(
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
        EscalateClaimCommand request,
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

        claim.Escalate();

        await _unitOfWork
            .SaveChangesAsync(
                cancellationToken);
    }
}