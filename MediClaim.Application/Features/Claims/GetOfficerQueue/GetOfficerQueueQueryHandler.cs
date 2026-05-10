using MediatR;
using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Application
    .Features.Claims.Common;
using MediClaim.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Application
    .Features.Claims.GetOfficerQueue;

public class GetOfficerQueueQueryHandler
    : IRequestHandler<
        GetOfficerQueueQuery,
        List<OfficerClaimQueueDto>>
{
    private readonly IApplicationDbContext
        _context;

    private readonly IUserRepository
        _currentUserService;

    public GetOfficerQueueQueryHandler(
        IApplicationDbContext context,
        IUserRepository currentUserService)
    {
        _context = context;

        _currentUserService =
            currentUserService;
    }

    public async Task<
        List<OfficerClaimQueueDto>>
            Handle(
                GetOfficerQueueQuery request,
                CancellationToken cancellationToken)
    {
        var tenantId =
            _currentUserService
                .TenantId;

        return await _context.Claims
            .Where(x =>
                x.TenantId == tenantId
                && x.Status ==
                    ClaimStatus.Submitted)
            .OrderByDescending(x =>
                x.RequiresFraudReview)
            .ThenByDescending(x =>
                x.FraudRiskScore)
            .ThenBy(x =>
                x.CreatedAt)
            .Select(x =>
                new OfficerClaimQueueDto
                {
                    ClaimId =
                        x.ClaimId,

                    PolicyNumber =
                        x.Policy
                            .PolicyNumber,

                    Amount =
                        x.Amount,

                    DiagnosisCode =
                        x.DiagnosisCode,

                    TreatmentCategory =
                        x.TreatmentCategory,

                    Status =
                        x.Status,

                    FraudRiskScore =
                        x.FraudRiskScore,

                    RequiresFraudReview =
                        x.RequiresFraudReview,

                    SubmittedAt =
                        x.UpdatedAt
                })
            .ToListAsync(
                cancellationToken);
    }
}