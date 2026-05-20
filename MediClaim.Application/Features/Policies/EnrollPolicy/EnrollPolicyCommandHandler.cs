using MediatR;
using MediClaim.Application
    .Common.Exceptions;
using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Domain.Entities;
using MediClaim.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Application
    .Features.Policies.EnrollPolicy;

public class EnrollPolicyCommandHandler
    : IRequestHandler<
        EnrollPolicyCommand,
        Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserRepository _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    public EnrollPolicyCommandHandler(
        IApplicationDbContext context,
        IUserRepository currentUserService,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }
    public async Task<Guid> Handle(
        EnrollPolicyCommand request,
        CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService
                .TenantId;
        // Validate patient
        var patient =
            await _context.Users
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId ==
                            request.PatientId
                        && x.TenantId ==
                            tenantId
                        && x.Role ==
                            UserRole.Patient,
                    cancellationToken);
        if (patient is null)
        {
            throw new NotFoundException(
                "Patient not found");
        }
        // Validate policy type
        var policyType =
            await _context.PolicyTypes
                .FirstOrDefaultAsync(
                    x =>
                        x.PolicyTypeId ==
                            request.PolicyTypeId
                        && x.TenantId ==
                            tenantId
                        && x.IsActive,
                    cancellationToken);

        if (policyType is null)
        {
            throw new NotFoundException(
                "Policy type not found");
        }
        // Generate policy number
        var policyNumber =
            $"POL-{DateTime.UtcNow:yyyy}-" +
            $"{Guid.NewGuid()
                .ToString()[..8]
                .ToUpper()}";
        // Create policy
        var policy =
            new Policy
            {
                PolicyId =Guid.NewGuid(),
                TenantId =tenantId,
                PatientId =request.PatientId,
                PolicyTypeId =request.PolicyTypeId,
                PolicyNumber =policyNumber,
                AnnualLimit =policyType.AnnualCoverageLimit,
                UsedAmount = 0,
                StartDate =request.StartDate,
                EndDate = request.EndDate,
                RemainingLimit = policyType.AnnualCoverageLimit
            };
        _context.Policies.Add(policy);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return policy.PolicyId;
    }
}