using MediatR;
using MediClaim.Application.Common.Interfaces;
using MediClaim.Application.Features.Reports.ClaimSummary;
using MediClaim.Application.Repositories;

namespace MediClaim.Application.Features.Reports.ClaimsSummary;

public class GetClaimsSummaryQueryHandler
    : IRequestHandler<
        GetClaimsSummaryQuery,
        List<ClaimsSummaryDto>>
{
    private readonly IReportRepository _reportRepository;
    private readonly ICurrentTenantService _tenantService;
    public GetClaimsSummaryQueryHandler(
        IReportRepository repository,
        ICurrentTenantService tenantService)
    {
        _reportRepository = repository;
        _tenantService = tenantService;
    }

    public async Task<List<ClaimsSummaryDto>>
        Handle(
            GetClaimsSummaryQuery request,
            CancellationToken cancellationToken)
    {
        return await _reportRepository
            .GetClaimsSummaryAsync(
                _tenantService.TenantId!.Value,
                cancellationToken);
    }
}