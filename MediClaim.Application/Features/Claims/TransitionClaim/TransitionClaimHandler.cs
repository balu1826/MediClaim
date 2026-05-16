using MediatR;
using MediClaim.Application.Common.Interfaces;
using MediClaim.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using MediClaim.Application.Common.Exceptions;
using MediClaim.Application.Repositories;


namespace MediClaim.Application.Features.Claims.TransitionClaim;

public class TransitionClaimHandler:IRequestHandler<TransitionClaimCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserRepository _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClaimRepository _claimRepository;
    public TransitionClaimHandler(IApplicationDbContext context,
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
        TransitionClaimCommand request,
        CancellationToken cancellationToken)
    {
        var claim = await _claimRepository.GetClaimByIdAsync(
                request.ClaimId,
                _currentUserService.TenantId,
                cancellationToken);
        if(claim == null)
        {
            throw new NotFoundException("Claim not found");
        }
        switch (request.NewStatus)
        {
            case ClaimStatus.UnderReview:

                if (claim.Status !=
                    ClaimStatus.Submitted)
                {
                    throw new UnprocessableEntityException("Invalid transition");
                }
                claim.Status = ClaimStatus.UnderReview;
                break;
            case ClaimStatus.Approved:

                if (claim.Status !=
                    ClaimStatus.UnderReview)
                {
                    throw new UnprocessableEntityException("Invalid transition");
                }
                claim.Status = ClaimStatus.Approved;
                break;
            case ClaimStatus.Rejected:
                if (claim.Status !=
                    ClaimStatus.UnderReview)
                {
                    throw new UnprocessableEntityException("Invalid transition");
                }

                claim.Status = ClaimStatus.Rejected;
                claim.RejectionReason = request.RejectionReason;
                break;
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
