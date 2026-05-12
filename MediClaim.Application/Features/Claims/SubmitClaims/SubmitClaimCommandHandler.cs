using MediatR;
using MediClaim.Application
    .Common.Exceptions;
using MediClaim.Application
    .Common.Interfaces;
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

    public SubmitClaimCommandHandler(
        IApplicationDbContext context,
        IUserRepository currentUserService,
        IUnitOfWork unitOfWork,
        IFraudScoringService fraudScoringService,IClaimAssignmentService claimAssignmentService)
    {
        _context = context;

        _currentUserService =
            currentUserService;
        _fraudScoringService = fraudScoringService;

        _unitOfWork = unitOfWork;
        _claimAssignmentService = claimAssignmentService;
    }

    public async Task Handle(
        SubmitClaimCommand request,
        CancellationToken cancellationToken)
    {
        var userId =
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
                        && x.UserId ==
                            userId
                        && x.TenantId ==
                            tenantId,
                    cancellationToken);

        if (claim is null)
        {
            throw new NotFoundException(
                "Claim not found");
        }

        // Domain transition

        claim.Submit();
        // Fraud evaluation

        var fraudResult =
            await _fraudScoringService
                .EvaluateAsync(claim, cancellationToken);

        claim.FraudRiskScore =
            fraudResult.Score;

        claim.RequiresFraudReview =
            fraudResult.RequiresReview;
        // Officer assignment

        await _claimAssignmentService
            .AssignAsync(
                claim,
                cancellationToken);

        await _unitOfWork
            .SaveChangesAsync(
                cancellationToken);
    }
}