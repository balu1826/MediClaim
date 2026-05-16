using MediatR;
using MediClaim.Application
    .Common.Exceptions;
using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Application.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Application
    .Features.Claims.RejectClaim;

public class RejectClaimCommandHandler
    : IRequestHandler<
        RejectClaimCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserRepository _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClaimRepository _claimRepository;
    public RejectClaimCommandHandler(
        IApplicationDbContext context,
        IUserRepository currentUserService,
        IUnitOfWork unitOfWork,
        IClaimRepository claimRepository)
    {
        _context = context;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _claimRepository = claimRepository;
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
            await _claimRepository.GetClaimByIdAsync(
                request.ClaimId,
                tenantId, cancellationToken);

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