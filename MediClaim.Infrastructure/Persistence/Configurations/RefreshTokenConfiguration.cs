using MediClaim.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediClaim.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration
    : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(
        EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(x => x.TokenId);

        builder.Property(x => x.TokenHash)
            .IsRequired();

        builder.HasIndex(x => x.UserId);

        builder.HasIndex(x => x.FamilyId);
    }
}