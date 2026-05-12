using MediClaim.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediClaim.Infrastructure
    .Persistence.Configurations;

public class JobExecutionLogConfiguration
    : IEntityTypeConfiguration<
        JobExecutionLog>
{
    public void Configure(
        EntityTypeBuilder<
            JobExecutionLog> builder)
    {
        builder.HasKey(
            x => x.JobExecutionLogId);

        builder.Property(
            x => x.JobName)
                .HasMaxLength(200)
                .IsRequired();

        builder.Property(
            x => x.Status)
                .HasMaxLength(50)
                .IsRequired();

        builder.Property(
            x => x.ErrorMessage)
                .HasColumnType(
                    "nvarchar(max)");
    }
}