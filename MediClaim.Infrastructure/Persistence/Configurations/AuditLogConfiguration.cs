using MediClaim.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediClaim.Infrastructure
    .Persistence.Configurations;

public class AuditLogConfiguration
    : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(
        EntityTypeBuilder<AuditLog> builder)
    {
        builder.HasKey(
            x => x.AuditId);

        builder.Property(
            x => x.AuditId)
                .ValueGeneratedOnAdd();

        builder.Property(
            x => x.EntityType)
                .HasMaxLength(50)
                .IsRequired();

        builder.Property(
            x => x.EntityId)
                .HasMaxLength(36)
                .IsRequired();

        builder.Property(
            x => x.Action)
                .HasMaxLength(50)
                .IsRequired();

        builder.Property(
            x => x.IpAddress)
                .HasMaxLength(45);

        builder.Property(
            x => x.CorrelationId)
                .HasMaxLength(36);

        builder.Property(
            x => x.Timestamp)
                .IsRequired();

        // Large JSON snapshots

        builder.Property(
            x => x.OldValues)
                .HasColumnType(
                    "nvarchar(max)");

        builder.Property(
            x => x.NewValues)
                .HasColumnType(
                    "nvarchar(max)");

        // Indexes

        builder.HasIndex(
            x => x.TenantId);

        builder.HasIndex(
            x => x.EntityId);

        builder.HasIndex(
            x => x.Timestamp);
    }
}