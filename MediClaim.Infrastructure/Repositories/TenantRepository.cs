using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using MediClaim.Application.Common.Interfaces;
using MediClaim.Application.Repositories;
using MediClaim.Domain.Entities;
using MediClaim.Domain.Enums;
using Microsoft.EntityFrameworkCore;



namespace MediClaim.Infrastructure.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly IApplicationDbContext _context;
        public TenantRepository(IApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Tenant?> GetTenantByIdAsync(Guid? id, CancellationToken cancellationToken)
        {
            if (id == null || id == Guid.Empty)
                return null;

            return await _context.Tenants.FindAsync(new object[] { id }, cancellationToken);
        }
        public async Task<bool> SlugExistsAsync(string slug)
        {
            return await _context.Tenants.AnyAsync(t => t.Slug == slug);
        }
        public async Task<Guid> AddAsync(Tenant tenant)
        {
            tenant.TenantId = Guid.NewGuid();
            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync(CancellationToken.None);
            return tenant.TenantId;
        }
    }
}
