using MediClaim.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediClaim.Infrastructure.Persistence.Configurations;

public class ClaimDocumentConfiguration
    : IEntityTypeConfiguration<ClaimDocument>
{
    public void Configure(
        EntityTypeBuilder<ClaimDocument> builder)
    {
        builder.HasKey(x => x.ClaimDocumentId);

        builder.Property(x => x.FileName)
            .HasMaxLength(255);

        builder.Property(x => x.ContentType)
            .HasMaxLength(100);

        builder.Property(x => x.FilePath)
            .HasMaxLength(500);

        builder.HasOne(x => x.Claim)
            .WithMany()
            .HasForeignKey(x => x.ClaimId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}