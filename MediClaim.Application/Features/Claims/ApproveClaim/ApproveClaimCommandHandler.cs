using MediatR;
using MediClaim.Application
    .Common.Exceptions;
using MediClaim.Application
    .Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using MediClaim.Application.Repositories;

namespace MediClaim.Application
    .Features.Claims.ApproveClaim;

public class ApproveClaimCommandHandler
    : IRequestHandler<ApproveClaimCommand>

{
    private readonly IApplicationDbContext _context;
    private readonly IUserRepository _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClaimRepository _claimRepository;
    public ApproveClaimCommandHandler(
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
        ApproveClaimCommand request,
        CancellationToken cancellationToken)
    {
        var officerId = _currentUserService.UserId;
        var tenantId = _currentUserService.TenantId;
        var claim = await _claimRepository.GetClaimByIdAsync(
                request.ClaimId,
                tenantId,
                cancellationToken);



        if (claim is null)
        {
            throw new NotFoundException("Claim not found");
        }

        if (claim.AssignedOfficerId
            != officerId)
        {
            throw new ForbiddenAccessException(
                "Claim not assigned");
        }
        // Fraud restriction
        if (claim.RequiresFraudReview)
        {
            throw new BadRequestException(
                "Fraud review claims require admin override");
        }

        // Approval limit validation
        var limit =
            claim.AssignedOfficer
                ?.ApprovalLimit;

        if (limit is not null
            && claim.Amount > limit)
        {
            throw new BadRequestException(
                "Officer approval limit exceeded");
        }
        claim.Approve();
        await _unitOfWork
            .SaveChangesAsync(
                cancellationToken);
    }
}