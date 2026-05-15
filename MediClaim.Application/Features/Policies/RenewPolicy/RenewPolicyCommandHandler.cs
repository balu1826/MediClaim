using MediatR;
using MediClaim.Application.Common.Interfaces;
using MediClaim.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Application.Features.Policies.RenewPolicy;

public class RenewPolicyCommandHandler
    : IRequestHandler<
        RenewPolicyCommand,
        Guid>
{
    private readonly IApplicationDbContext _context;

    private readonly IUnitOfWork _unitOfWork;

    public RenewPolicyCommandHandler(
        IApplicationDbContext context,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> Handle(
        RenewPolicyCommand request,
        CancellationToken cancellationToken)
    {
        var existingPolicy =
            await _context.Policies
                .SingleAsync(
                    x =>
                        x.PolicyId == request.PolicyId&&!x.IsArchived,
                    cancellationToken);
        existingPolicy.IsArchived = true;
        var duration = existingPolicy.EndDate.ToDateTime(TimeOnly.MinValue)
                     - existingPolicy.StartDate.ToDateTime(TimeOnly.MinValue);

        var policyNumber = $"POL-{DateTime.UtcNow:yyyy}-" + $"{Guid.NewGuid()
                .ToString()[..8]
                .ToUpper()}";
        var renewedPolicy =
            new Policy
            {
                PolicyId = Guid.NewGuid(),
                PolicyNumber=policyNumber,
                PatientId = existingPolicy.PatientId,
                TenantId = existingPolicy.TenantId,
                PolicyTypeId = existingPolicy.PolicyTypeId,
                AnnualLimit = existingPolicy.AnnualLimit,
                RemainingLimit = existingPolicy.RemainingLimit,
                StartDate = existingPolicy.EndDate.AddDays(1),
                EndDate = existingPolicy.StartDate.AddDays(duration.Days),
                IsArchived = false
            };

        await _context.Policies
            .AddAsync(
                renewedPolicy,
                cancellationToken);

        await _unitOfWork
            .SaveChangesAsync(
                cancellationToken);

        return renewedPolicy.PolicyId;
    }
}