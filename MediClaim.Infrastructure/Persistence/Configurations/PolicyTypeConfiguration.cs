using MediClaim.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediClaim.Infrastructure
    .Persistence.Configurations;

public class PolicyTypeConfiguration
    : IEntityTypeConfiguration<PolicyType>
{
    public void Configure(
        EntityTypeBuilder<PolicyType> builder)
    {
        builder.HasKey(
            x => x.PolicyTypeId);

        builder.Property(
            x => x.Name)
            .HasMaxLength(200);

        builder.Property(
            x => x.AnnualCoverageLimit)
            .HasColumnType(
                "decimal(18,2)");

        builder.Property(
            x => x.DeductibleAmount)
            .HasColumnType(
                "decimal(18,2)");

        builder.HasOne(
            x => x.Tenant)
            .WithMany()
            .HasForeignKey(
                x => x.TenantId);
        builder.HasMany(
            x => x.CoverageCategories)
        .WithOne(
            x => x.PolicyType)
        .HasForeignKey(
            x => x.PolicyTypeId);
    }
}