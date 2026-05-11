using MediatR;
using MediClaim.Application
    .Common.Exceptions;
using MediClaim.Application
    .Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Application
    .Features.Policies
    .UpgradePolicy;

public class UpgradePolicyCommandHandler
    : IRequestHandler<
        UpgradePolicyCommand>
{
    private readonly IApplicationDbContext
        _context;

    private readonly IPolicyUpgradeService
        _upgradeService;

    private readonly IUnitOfWork
        _unitOfWork;

    public UpgradePolicyCommandHandler(
        IApplicationDbContext context,
        IPolicyUpgradeService upgradeService,
        IUnitOfWork unitOfWork)
    {
        _context = context;

        _upgradeService =
            upgradeService;

        _unitOfWork = unitOfWork;
    }

    public async Task Handle(
        UpgradePolicyCommand request,
        CancellationToken cancellationToken)
    {
        var policy =
            await _context.Policies
                .FirstOrDefaultAsync(
                    x =>
                        x.PolicyId ==
                            request.PolicyId,
                    cancellationToken);

        if (policy is null)
        {
            throw new NotFoundException(
                "Policy not found");
        }

        var newPolicyType =
            await _context.PolicyTypes
                .FirstOrDefaultAsync(
                    x =>
                        x.PolicyTypeId ==
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