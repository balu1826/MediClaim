using MediatR;
using MediClaim.Application.Common.Interfaces;
using MediClaim.Application.Repositories;
using System.Text;

namespace MediClaim.Application.Features.Reports.AuditExport;

public class ExportAuditLogsQueryHandler
    : IRequestHandler<
        ExportAuditLogsQuery,
        byte[]>
{
    private readonly IReportRepository _repository;
    public ExportAuditLogsQueryHandler(
        IReportRepository repository)
    {
        _repository = repository;
    }

    public async Task<byte[]> Handle(
        ExportAuditLogsQuery request,
        CancellationToken cancellationToken)
    {
        var logs =
            await _repository
                .GetAuditLogsAsync(
                    request.From,
                    request.To,
                    cancellationToken);

        var builder = new StringBuilder();
        builder.AppendLine("Timestamp,Action,EntityType,UserId");
        foreach (var log in logs)
        {
            builder.AppendLine(
                $"{log.Timestamp}," +
                $"{log.Action}," +
                $"{log.EntityType}," +
                $"{log.ChangedByUserId}");
        }

        return Encoding.UTF8.GetBytes(
            builder.ToString());
    }
}