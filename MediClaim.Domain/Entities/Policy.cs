using MediClaim.Domain.Common;

namespace MediClaim.Domain.Entities;

public class Policy
    : BaseEntity
{
    public Guid PolicyId
    {
        get; set;
    }

    public Guid TenantId
    {
        get; set;
    }

    public Guid PatientId
    {
        get; set;
    }

    public Guid PolicyTypeId
    {
        get; set;
    }

    public string PolicyNumber
    {
        get; set;
    } = default!;

    public decimal AnnualLimit
    {
        get; set;
    }

    public decimal UsedAmount
    {
        get; set;
    }

    public DateOnly StartDate
    {
        get; set;
    }

    public DateOnly EndDate
    {
        get; set;
    }

    public byte[] RowVersion
    {
        get; set;
    } = default!;

    public User Patient
    {
        get; set;
    } = default!;

    public PolicyType PolicyType
    {
        get; set;
    } = default!;

    public Tenant Tenant
    {
        get; set;
    } = default!;
    public decimal RemainingLimit
    {
        get; set;
    }
    public bool IsArchived { get; set; }
}