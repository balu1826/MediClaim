using MediClaim.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediClaim.Infrastructure.Persistence.Configurations;

public class TenantConfiguration
    : IEntityTypeConfiguration<Tenant>
{
    public void Configure(
        EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(x => x.TenantId);

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Slug)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(x => x.Slug)
            .IsUnique();
    }
}