using MediClaim.Application.Common.Interfaces;
using MediClaim.Application.Features.Claims.Common;
using MediClaim.Application.Features.Claims.GetFraudFlags;
using MediClaim.Application.Repositories;
using MediClaim.Domain.Entities;
using MediClaim.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediClaim.Infrastructure.Persistence
{
    public class ClaimRepository : IClaimRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserRepository _userRepository;
        public ClaimRepository(ApplicationDbContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }
        public async Task<Domain.Entities.Claim?> GetClaimByIdAsync(
            Guid claimId,
            Guid tenantId,
            CancellationToken cancellationToken)
        {
            return await _context.Claims
                .FirstOrDefaultAsync(x =>
                    x.ClaimId == claimId
                    && x.TenantId == tenantId,
                    cancellationToken);
        }
        public async Task AddClaimAsync(
            Domain.Entities.Claim claim,
            CancellationToken cancellationToken)
        {
            await _context.Claims.AddAsync(claim, cancellationToken);
        }
        public async Task<List<ClaimDto>> GetMyClaimsAsync(
         Guid userId,
         Guid tenantId,
         CancellationToken cancellationToken)
        {
            return await _context.Claims
                .Where(x => x.UserId == userId
                    &&
                    x.TenantId == tenantId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x =>
                    new ClaimDto
                    {
                        ClaimId = x.ClaimId,
                        PolicyNumber = x.Policy.PolicyNumber,
                        Amount = x.Amount,
                        DiagnosisCode = x.DiagnosisCode,
                        TreatmentCategory = x.TreatmentCategory,
                        Status = x.Status,
                        CreatedAt = x.CreatedAt
                    })
                .ToListAsync(cancellationToken);
        }
        public async Task<List<OfficerClaimQueueDto>> GetOfficerQueueAsync(
            Guid tenantId,
            Guid officerId,
            CancellationToken cancellationToken)
        {
            return await _context.Claims
                .Where(x => x.TenantId == tenantId
                    &&
                    x.AssignedOfficerId == officerId
                    &&
                    x.Status == ClaimStatus.Submitted)
                .OrderByDescending(x => x.RequiresFraudReview)
                .ThenByDescending(x => x.FraudRiskScore)
                .ThenBy(x => x.CreatedAt)
                .Select(x =>
                    new OfficerClaimQueueDto
                    {
                        ClaimId = x.ClaimId,
                        PolicyNumber = x.Policy.PolicyNumber,
                        Amount = x.Amount,
                        DiagnosisCode = x.DiagnosisCode,
                        TreatmentCategory = x.TreatmentCategory,
                        Status = x.Status,
                        FraudRiskScore = x.FraudRiskScore,
                        RequiresFraudReview = x.RequiresFraudReview,
                        SubmittedAt = x.UpdatedAt
                    })
                .ToListAsync(cancellationToken);
        }
        public async Task<List<FraudFlagDto>> GetFraudFlagsAsync(
            Guid tenantId,
            int threshold,
            CancellationToken cancellationToken)
        {
            return await _context.Claims
                .Where(x => x.TenantId == tenantId
                    &&
                    x.FraudRiskScore >= threshold)
                .Select(x =>
                    new FraudFlagDto
                    {
                        ClaimId = x.ClaimId,
                        FraudScore = x.FraudRiskScore,
                        Amount = x.Amount,
                        DiagnosisCode = x.DiagnosisCode
                    })
                .ToListAsync(cancellationToken);
        }

    }
}
