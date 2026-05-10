using MediClaim.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediClaim.Infrastructure
    .Persistence.Configurations;

public class PolicyCoverageCategoryConfiguration
    : IEntityTypeConfiguration<
        PolicyCoverageCategory>
{
    public void Configure(
        EntityTypeBuilder<
            PolicyCoverageCategory>
                builder)
    {
        builder.HasKey(
            x =>
                x.PolicyCoverageCategoryId);

        builder.Property(
            x => x.Name)
            .HasMaxLength(100);
    }
}