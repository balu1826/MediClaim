using MediClaim.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediClaim.Infrastructure.Persistence.Configurations;

public class UserConfiguration
    : IEntityTypeConfiguration<User>
{
    public void Configure(
        EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.UserId);

        builder.Property(x => x.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(x => x.PasswordHash)
            .HasMaxLength(512)
            .IsRequired();

        builder.Property(x => x.ApprovalLimit)
            .HasPrecision(18, 2);

        builder.Property(x => x.SsnEncrypted)
            .HasColumnType("VARBINARY(256)");

        builder.HasIndex(x =>
            new { x.TenantId, x.Email })
            .IsUnique();
    }
}