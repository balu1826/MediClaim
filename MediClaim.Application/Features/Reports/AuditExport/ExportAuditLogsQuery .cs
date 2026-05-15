using MediatR;
namespace MediClaim.Application.Features.Reports.AuditExport
{
    public class ExportAuditLogsQuery:IRequest<byte[]>
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
