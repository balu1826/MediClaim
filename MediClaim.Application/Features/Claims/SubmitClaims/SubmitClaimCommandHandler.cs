using MediatR;
using MediClaim.Application
    .Common.Exceptions;
using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Application.Repositories;
using MediClaim.Domain.Enums;
using Microsoft.EntityFrameworkCore;


namespace MediClaim.Application
    .Features.Claims.SubmitClaim;

public class SubmitClaimCommandHandler
    : IRequestHandler<
        SubmitClaimCommand>
{
    private readonly IApplicationDbContext
        _context;

    private readonly IUserRepository
        _currentUserService;

    private readonly IUnitOfWork
        _unitOfWork;
    private readonly IFraudScoringService
    _fraudScoringService;
    private readonly IClaimAssignmentService
    _claimAssignmentService;
    private readonly IClaimRepository
        _claimRepository;

    public SubmitClaimCommandHandler(
        IApplicationDbContext context,
        IUserRepository currentUserService,
        IUnitOfWork unitOfWork,
        IFraudScoringService fraudScoringService,
        IClaimAssignmentService claimAssignmentService,
        IClaimRepository claimRepository)
    {
        _context = context;
        _currentUserService = currentUserService;
        _fraudScoringService = fraudScoringService;
        _unitOfWork = unitOfWork;
        _claimAssignmentService = claimAssignmentService;
        _claimRepository = claimRepository;
    }

    public async Task Handle(
        SubmitClaimCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var tenantId = _currentUserService.TenantId;
        var claim =await _claimRepository.GetClaimByIdAsync(
                request.ClaimId,
                tenantId,
                cancellationToken);
        if (claim is null)
        {
            throw new NotFoundException(
                "Claim not found");
        }

        // Domain transition

       
        if (claim.Status != ClaimStatus.Draft)
        {
            throw new UnprocessableEntityException("Claim Already Submitted");
        }
        claim.Submit();
        // Fraud evaluation


        var fraudResult =
            await _fraudScoringService
                .EvaluateAsync(claim, cancellationToken);

        claim.FraudRiskScore = fraudResult.Score;
        claim.RequiresFraudReview = fraudResult.RequiresReview;
        claim.SubmittedAt = DateTime.UtcNow;
        // Officer assignment
        await _claimAssignmentService.AssignAsync(
                claim,
                cancellationToken);
        await _unitOfWork.SaveChangesAsync(
                cancellationToken);
    }
}