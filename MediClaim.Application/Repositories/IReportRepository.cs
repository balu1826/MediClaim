using MediClaim.Domain.Entities;
using MediClaim.Application.Features.Reports.ClaimSummary;
using MediClaim.Application.Features.Reports.OfficerWorkLoad;


namespace MediClaim.Application.Repositories
{
    public interface IReportRepository
    {
        Task<List<ClaimsSummaryDto>> GetClaimsSummaryAsync(
            Guid tenantId,
            CancellationToken cancellationToken);
        Task<List<OfficerWorkloadDto>> GetOfficerWorkloadAsync(
                CancellationToken cancellationToken);
        Task<List<AuditLog>> GetAuditLogsAsync(
                DateTime from,
                DateTime to,
                CancellationToken cancellationToken);

    }
}
