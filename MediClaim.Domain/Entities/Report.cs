namespace MediClaim.Domain.Entities;

public class Report
{
    public Guid ReportId
    {
        get;
        set;
    }

    public Guid TenantId
    {
        get;
        set;
    }

    public string ReportType
    {
        get;
        set;
    } = default!;

    public int TotalClaims
    {
        get;
        set;
    }

    public int ApprovedClaims
    {
        get;
        set;
    }

    public decimal RejectionRate
    {
        get;
        set;
    }

    public decimal AverageProcessingTimeHours
    {
        get;
        set;
    }

    public string ReportContent
    {
        get;
        set;
    } = default!;

    public DateTime GeneratedAt
    {
        get;
        set;
    }

    // Navigation

    public Tenant Tenant
    {
        get;
        set;
    } = default!;
}