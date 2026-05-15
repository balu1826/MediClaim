using MediClaim.Application.Common.Interfaces;
using MediClaim.Application.Features.Reports.ClaimSummary;
using MediClaim.Application.Features.Reports.OfficerWorkLoad;
using MediClaim.Application.Repositories;
using MediClaim.Domain.Entities;
using MediClaim.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Infrastructure.Persistence;

public class ReportRepository
    : IReportRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IUserRepository _userRepository;
    public ReportRepository(ApplicationDbContext context, IUserRepository userRepository)
    {
        _context = context;
        _userRepository = userRepository;
    }
    public async Task<List<ClaimsSummaryDto>> GetClaimsSummaryAsync(
                Guid tenantId,
                CancellationToken cancellationToken)
    {
        return await _context.Claims
            .Where(x => x.TenantId == tenantId)
            .GroupBy(x =>
                new
                {
                    x.CreatedAt.Year,
                    x.CreatedAt.Month
                })
            .Select(g =>
                new ClaimsSummaryDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalClaims = g.Count(),
                    ApprovedClaims = g.Count(x =>
                            x.Status == ClaimStatus.Approved),
                    ApprovalRate =
                        g.Count() == 0
                        ? 0
                        : (decimal)
                            g.Count(x =>
                                x.Status ==
                                    ClaimStatus.Approved)
                            / g.Count()
                })
            .ToListAsync(cancellationToken);
    }

    public async Task<List<OfficerWorkloadDto>> GetOfficerWorkloadAsync(
                CancellationToken cancellationToken)
    {
        return await _context.Claims
       .Where(x => x.AssignedOfficerId != null)
       .GroupBy(x => x.AssignedOfficerId)
       .Select(g =>
           new OfficerWorkloadDto
           {
               OfficerId = g.Key!.Value,
               TotalClaims = g.Count(),
               ApprovedClaims = g.Count(x =>
                       x.Status == ClaimStatus.Approved),
               RejectedClaims = g.Count(x =>
                       x.Status == ClaimStatus.Rejected)
           })
       .ToListAsync(cancellationToken);
    }

    public async Task<List<AuditLog>> GetAuditLogsAsync(
                DateTime from,
                DateTime to,
                CancellationToken cancellationToken)
    {
        return await _context.AuditLogs
            .Where(x =>
                x.Timestamp >= from
                &&
                x.Timestamp <= to)
            .Where(x=>x.TenantId==_userRepository.TenantId)
            .ToListAsync(cancellationToken);

    }
}