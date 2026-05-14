using MediClaim.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediClaim.Infrastructure
    .Persistence.Configurations;

public class ClaimConfiguration
    : IEntityTypeConfiguration<Claim>
{
    public void Configure(
        EntityTypeBuilder<Claim> builder)
    {
        builder.HasKey(x => x.ClaimId);

        builder.Property(x => x.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.DiagnosisCode)
            .HasMaxLength(50);

        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);

        builder.HasOne(x => x.Tenant)
            .WithMany()
            .HasForeignKey(x => x.TenantId);
        builder.Property(x => x.TreatmentCategory)
            .HasMaxLength(100);
                builder.HasOne(
                    x => x.Policy)
                    .WithMany()
                    .HasForeignKey(
                        x => x.PolicyId)
                    .OnDelete(DeleteBehavior.Restrict);
        builder.Property(
    x => x.ProviderId)
    .IsRequired();
        builder.HasOne(
            x => x.Provider)
            .WithMany()
            .HasForeignKey(
                x => x.ProviderId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(
    x => x.AssignedOfficer)
    .WithMany()
    .HasForeignKey(
        x => x.AssignedOfficerId)
    .OnDelete(
        DeleteBehavior.Restrict);
    }
}