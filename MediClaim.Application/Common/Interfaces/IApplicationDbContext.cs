using MediClaim.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Application
    .Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }

    DbSet<Tenant> Tenants { get; }

    DbSet<Claim> Claims { get; }
    DbSet<PolicyType>  PolicyTypes  { get; }
    DbSet<Policy> Policies { get; }
    DbSet<Provider> Providers { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken);
}