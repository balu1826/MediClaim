using MediClaim.Application.Common.Interfaces;
using MediClaim.Domain.Common;
using MediClaim.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using MediClaim.Application.Repositories;
using MediClaim.Infrastructure.Persistence;


namespace MediClaim.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentTenantService _currentTenantService;
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options, ICurrentTenantService currentTenantService)
        : base(options)
    {
        _currentTenantService = currentTenantService;
    }

   
    
   
    public DbSet<Tenant> Tenants => Set<Tenant>();

    public DbSet<User> Users => Set<User>();
    public DbSet<Claim> Claims=> Set<Claim>();
    public DbSet<PolicyType>PolicyTypes=> Set<PolicyType>();
    public DbSet<Policy> Policies=> Set<Policy>();
    public DbSet<Provider> Providers => Set<Provider>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<ClaimDocument> ClaimDocuments { get; set; }
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<JobExecutionLog>

    JobExecutionLogs => Set<JobExecutionLog>();


    public DbSet<ClaimStatusHistory>
    ClaimStatusHistories => Set<ClaimStatusHistory>();


    protected override void OnModelCreating(
    ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        modelBuilder.Entity<User>()
     .HasQueryFilter(x =>
         !x.IsDeleted &&
         (_currentTenantService.TenantId == null
          || x.TenantId ==
             _currentTenantService.TenantId));

        modelBuilder.Entity<RefreshToken>()
            .HasQueryFilter(x =>
                !x.IsDeleted);

        base.OnModelCreating(modelBuilder);
        
    }
    
}