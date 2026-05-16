using MediatR;
using MediClaim.Application
    .Common.Exceptions;
using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Application.Repositories;
using MediClaim.Domain.Entities;
using MediClaim.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Application
    .Features.Claims.CreateDraftClaim;

public class CreateDraftClaimCommandHandler
    : IRequestHandler<
        CreateDraftClaimCommand,
        Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserRepository _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPolicyRepository _policyRepository;
    public CreateDraftClaimCommandHandler(
        IApplicationDbContext context,
        IUserRepository currentUserService,
        IUnitOfWork unitOfWork,
        IPolicyRepository policyRepository)
    {
        _context = context;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _policyRepository = policyRepository;
    }

    public async Task<Guid> Handle(
        CreateDraftClaimCommand request,
        CancellationToken cancellationToken)
    {
        var userId =
            _currentUserService
                .UserId;

        var tenantId =
            _currentUserService
                .TenantId;

        // Validate active policy ownership
        var policy =await _policyRepository.GetPolicyByIdAsync(
            request.PolicyId,
            cancellationToken);
        if (policy is null||policy.StartDate > DateOnly.FromDateTime(DateTime.UtcNow)||
            policy.EndDate < DateOnly.FromDateTime(DateTime.UtcNow))
        {
            throw new BadRequestException(
                "Active policy not found");
        }

        // Validate coverage category
        var categoryCovered =
            policy.PolicyType
                .CoverageCategories
                .Any(x =>
                    x.Name ==
                    request
                        .TreatmentCategory);

        if (!categoryCovered)
        {
            throw new BadRequestException(
                "Treatment category not covered");
        }

        // Create claim

        var claim =
            new Claim
            {
                ClaimId = Guid.NewGuid(),
                TenantId = tenantId,
                UserId = userId,
                PolicyId = request.PolicyId,
                Amount = request.Amount,
                DiagnosisCode = request.DiagnosisCode,
                TreatmentCategory = request.TreatmentCategory,
                Description = request.Description,
                Status = ClaimStatus.Draft
            };

        await _context.Claims
            .AddAsync(
                claim,
                cancellationToken);

        await _unitOfWork
            .SaveChangesAsync(
                cancellationToken);

        return claim.ClaimId;
    }
}