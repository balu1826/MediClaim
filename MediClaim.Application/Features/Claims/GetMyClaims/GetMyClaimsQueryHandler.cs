using MediatR;
using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Application
    .Features.Claims.Common;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Application
    .Features.Claims.GetMyClaims;

public class GetMyClaimsQueryHandler
    : IRequestHandler<
        GetMyClaimsQuery,
        List<ClaimDto>>
{
    private readonly IApplicationDbContext
        _context;

    private readonly IUserRepository
        _currentUserService;

    public GetMyClaimsQueryHandler(
        IApplicationDbContext context,
        IUserRepository currentUserService)
    {
        _context = context;

        _currentUserService =
            currentUserService;
    }

    public async Task<List<ClaimDto>>
        Handle(
            GetMyClaimsQuery request,
            CancellationToken cancellationToken)
    {
        var userId =
            _currentUserService
                .UserId;

        var tenantId =
            _currentUserService
                .TenantId;

        return await _context.Claims
            .Where(x =>
                x.UserId == userId
                && x.TenantId == tenantId)
            .OrderByDescending(
                x => x.CreatedAt)
            .Select(x =>
                new ClaimDto
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

                    CreatedAt =
                        x.CreatedAt
                })
            .ToListAsync(
                cancellationToken);
    }
}