using MediClaim.Application.Common.Interfaces;
using MediClaim.Domain.Entities;
using MediClaim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Infrastructure.Repositories;

public class TenantRepository
    : Repository<Tenant>,
      ITenantRepository
{
    public TenantRepository(
        ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<bool> SlugExistsAsync(
        string slug)
    {
        return await _dbSet.AnyAsync(x =>
            x.Slug == slug);
    }
}