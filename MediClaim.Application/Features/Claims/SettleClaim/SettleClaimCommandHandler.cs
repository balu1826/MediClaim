using MediatR;
using MediClaim.Application
    .Common.Exceptions;
using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Application.Repositories;
using MediClaim.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Application
    .Features.Claims.SettleClaim;

public class SettleClaimCommandHandler
    : IRequestHandler<
        SettleClaimCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserRepository _currentUserService;
    private readonly IClaimSettlementService _settlementService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClaimRepository _claimRepository;

    public SettleClaimCommandHandler(
        IApplicationDbContext context,
        IUserRepository currentUserService,
        IClaimSettlementService settlementService,
        IUnitOfWork unitOfWork,
        IClaimRepository claimRepository)
    {
        _context = context;
        _currentUserService = currentUserService;
        _settlementService = settlementService;
        _unitOfWork = unitOfWork;
        _claimRepository = claimRepository;
    }

    public async Task Handle(
        SettleClaimCommand request,
        CancellationToken cancellationToken)
    {
        var tenantId =
            _currentUserService
                .TenantId;

        var claim =
            await _claimRepository.GetClaimByIdAsync(
                request.ClaimId,
                tenantId,
                cancellationToken);

        if (claim is null)
        {
            throw new NotFoundException(
                "Claim not found");
        }

        // Already settled (idempotent)

        if (claim.Status ==
            ClaimStatus.Settled)
        {
            return;
        }

        // Only approved claims

        if (claim.Status !=
            ClaimStatus.Approved)
        {
            throw new BadRequestException(
                "Only approved claims can be settled");
        }
        var approvedAmount =
            claim.ApprovedAmount
            ?? claim.Amount;
        var result =
            await _settlementService
                .SettleAsync(
                    claim.ClaimId,
                    approvedAmount,
                    claim.PolicyId,
                    cancellationToken);
        if (result.InsufficientBalance)
        {
            throw new ConflictException(
                "Policy balance insufficient");
        }
        // Sync aggregate state
        claim.Status = ClaimStatus.Settled;
        await _unitOfWork
            .SaveChangesAsync(
                cancellationToken);
    }
}