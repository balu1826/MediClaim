using MediClaim.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MediClaim.Application.Repositories;
using MediClaim.Domain.Entities;

namespace MediClaim.Infrastructure.Repositories
{
    public class PolicyRepository : IPolicyRepository
    {
        private readonly IApplicationDbContext _context;
        public PolicyRepository(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Policy?> GetPolicyByIdAsync
            (Guid policyId,
            CancellationToken cancellationToken)
        {
            return await _context.Policies
                .FirstOrDefaultAsync(x =>
                    x.PolicyId == policyId,
                    cancellationToken);
        }
        public async Task<PolicyType?> GetPolicyTypeByIdAsync
            (Guid policyTypeId,
            CancellationToken cancellationToken)
        {
            return await _context.PolicyTypes
                .FirstOrDefaultAsync(x =>
                    x.PolicyTypeId == policyTypeId,
                    cancellationToken);
        }
    }
}
