using MediatR;
using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Domain.Entities;

namespace MediClaim.Application
    .Features.Policies.CreatePolicyType;

public class CreatePolicyTypeCommandHandler
    : IRequestHandler<
        CreatePolicyTypeCommand,
        Guid>
{
    private readonly IRepository<PolicyType>
        _repository;

    private readonly IUnitOfWork
        _unitOfWork;

    private readonly IUserRepository
        _currentUserService;

    public CreatePolicyTypeCommandHandler(
        IRepository<PolicyType> repository,
        IUnitOfWork unitOfWork,
        IUserRepository currentUserService)
    {
        _repository = repository;

        _unitOfWork = unitOfWork;

        _currentUserService =
            currentUserService;
    }

    public async Task<Guid> Handle(
        CreatePolicyTypeCommand request,
        CancellationToken cancellationToken)
    {
        var policyType =
            new PolicyType
            {
                PolicyTypeId =
                    Guid.NewGuid(),

                TenantId =
                    _currentUserService
                        .TenantId,

                Name = request.Name,

                AnnualCoverageLimit =
                    request
                        .AnnualCoverageLimit,

                DeductibleAmount =
                    request
                        .DeductibleAmount,

                IsActive = true,
                CoverageCategories =
    request
        .CoverageCategories
        .Select(x =>
            new PolicyCoverageCategory
            {
                PolicyCoverageCategoryId =
                    Guid.NewGuid(),

                Name = x
            })
        .ToList()
            };

        await _repository
            .AddAsync(policyType);

        await _unitOfWork
            .SaveChangesAsync(
                cancellationToken);

        return policyType
            .PolicyTypeId;
    }
}