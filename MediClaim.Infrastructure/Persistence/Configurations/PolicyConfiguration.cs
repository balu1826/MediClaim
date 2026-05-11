using MediClaim.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediClaim.Infrastructure
    .Persistence.Configurations;

public class PolicyConfiguration
    : IEntityTypeConfiguration<Policy>
{
    public void Configure(
        EntityTypeBuilder<Policy> builder)
    {
        builder.HasKey(
            x => x.PolicyId);

        builder.Property(
            x => x.PolicyNumber)
            .HasMaxLength(30);

        builder.HasIndex(
            x => x.PolicyNumber)
            .IsUnique();

        builder.Property(
            x => x.AnnualLimit)
            .HasColumnType(
                "decimal(18,2)");

        builder.Property(
            x => x.UsedAmount)
            .HasColumnType(
                "decimal(18,2)");

        builder.Property(
            x => x.RowVersion)
            .IsRowVersion();

        builder.HasOne(
            x => x.Patient)
            .WithMany()
            .HasForeignKey(
                x => x.PatientId);

        builder.HasOne(
            x => x.PolicyType)
            .WithMany()
            .HasForeignKey(
                x => x.PolicyTypeId);

        builder.HasOne(
      x => x.Tenant)
      .WithMany()
      .HasForeignKey(
          x => x.TenantId)
      .OnDelete(DeleteBehavior.Restrict);
        builder.Property(
    x => x.RemainingLimit)
    .HasColumnType(
        "decimal(18,2)");
    }
}