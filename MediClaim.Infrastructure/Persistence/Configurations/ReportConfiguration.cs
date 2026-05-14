using MediClaim.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MediClaim.Infrastructure
    .Persistence.Configurations;

public class SummaryReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<
            Report> builder)
    {
        builder.HasKey(
            x => x.ReportId);
        builder.Property(
            x => x.ReportType)
            .HasMaxLength(100);
        builder.Property(
            x => x.RejectionRate)
            .HasPrecision(18, 2);
        builder.Property(
            x =>
                x.AverageProcessingTimeHours)
            .HasPrecision(18, 2);

        builder.Property(
            x => x.ReportContent)
            .HasMaxLength(4000);

        builder.HasOne(
            x => x.Tenant)
            .WithMany()
            .HasForeignKey(
             x => x.TenantId);
    }
}