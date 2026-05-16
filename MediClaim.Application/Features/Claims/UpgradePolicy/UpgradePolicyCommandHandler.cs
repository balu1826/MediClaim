using MediatR;
using MediClaim.Application
    .Common.Exceptions;
using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Application.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Application
    .Features.Policies
    .UpgradePolicy;

public class UpgradePolicyCommandHandler
    : IRequestHandler<
        UpgradePolicyCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IPolicyUpgradeService _upgradeService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClaimRepository _claimRepository;
    private readonly IPolicyRepository _policyRepository;

    public UpgradePolicyCommandHandler(
        IApplicationDbContext context,
        IPolicyUpgradeService upgradeService,
        IUnitOfWork unitOfWork,
        IClaimRepository claimRepository,
        IPolicyRepository policyRepository
        )
    {
        _context = context;
        _upgradeService = upgradeService;
        _unitOfWork = unitOfWork;
        _claimRepository = claimRepository;
        _policyRepository = policyRepository;
    }

    public async Task Handle(
        UpgradePolicyCommand request,
        CancellationToken cancellationToken)
    {
        var policy = await _policyRepository.GetPolicyByIdAsync(
                request.PolicyId,
                cancellationToken);
        if (policy is null)
        {
            throw new NotFoundException(
                "Policy not found");
        }

        var newPolicyType =
            await _policyRepository.GetPolicyTypeByIdAsync(
                request.NewPolicyTypeId,
                cancellationToken);
        if (newPolicyType is null)
        {
            throw new NotFoundException(
                "Policy type not found");
        }

        await _upgradeService
            .UpgradeAsync(
                policy,
                newPolicyType,
                cancellationToken);

        await _unitOfWork
            .SaveChangesAsync(
                cancellationToken);
    }
}