namespace MediClaim.Domain.Entities;

public class JobExecutionLog
{
    public long JobExecutionLogId
    {
        get; set;
    }

    public string JobName
    {
        get; set;
    } = default!;

    public DateTime StartedAt
    {
        get; set;
    }

    public DateTime? FinishedAt
    {
        get; set;
    }

    public string Status
    {
        get; set;
    } = default!;

    public int RecordsAffected
    {
        get; set;
    }

    public string? ErrorMessage
    {
        get; set;
    }
}