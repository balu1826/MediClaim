using MediClaim.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediClaim.Infrastructure
    .Persistence.Configurations;

public class ClaimStatusHistoryConfiguration
    : IEntityTypeConfiguration<
        ClaimStatusHistory>
{
    public void Configure(
        EntityTypeBuilder<
            ClaimStatusHistory> builder)
    {
        builder.HasKey(
            x => x.ClaimStatusHistoryId);

        builder.Property(
            x => x.Status)
                .HasMaxLength(50)
                .IsRequired();

        builder.Property(
            x => x.Notes)
                .HasMaxLength(1000);

        builder.HasOne(
            x => x.Claim)
                .WithMany()
                .HasForeignKey(
                    x => x.ClaimId)
                .OnDelete(
                    DeleteBehavior.Cascade);
    }
}