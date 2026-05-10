using MediClaim.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediClaim.Infrastructure
    .Persistence.Configurations;

public class ProviderConfiguration
    : IEntityTypeConfiguration<Provider>
{
    public void Configure(
        EntityTypeBuilder<Provider> builder)
    {
        builder.HasKey(
            x => x.ProviderId);

        builder.Property(
            x => x.Name)
            .HasMaxLength(200);

        builder.Property(
            x => x.Specialty)
            .HasMaxLength(100);

        builder.HasOne(
            x => x.Tenant)
            .WithMany()
            .HasForeignKey(
                x => x.TenantId)
            .OnDelete(
                DeleteBehavior.Restrict);
    }
}