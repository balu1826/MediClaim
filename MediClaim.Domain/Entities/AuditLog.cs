namespace MediClaim.Domain.Entities;

public class AuditLog
{
    public long AuditId
    {
        get; set;
    }

    public Guid TenantId
    {
        get; set;
    }

    public string EntityType
    {
        get; set;
    } = default!;

    public string EntityId
    {
        get; set;
    } = default!;

    public string Action
    {
        get; set;
    } = default!;

    public string? OldValues
    {
        get; set;
    }

    public string? NewValues
    {
        get; set;
    }

    public Guid ChangedByUserId
    {
        get; set;
    }

    public string? IpAddress
    {
        get; set;
    }

    public string? CorrelationId
    {
        get; set;
    }

    public DateTime Timestamp
    {
        get; set;
    }
}