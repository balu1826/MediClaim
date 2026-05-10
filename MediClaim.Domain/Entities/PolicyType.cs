using MediClaim.Domain.Common;

namespace MediClaim.Domain.Entities;

public class PolicyType
    : BaseEntity
{
    public Guid PolicyTypeId
    {
        get; set;
    }

    public Guid TenantId
    {
        get; set;
    }

    public string Name
    {
        get; set;
    } = default!;

    public decimal AnnualCoverageLimit
    {
        get; set;
    }

    public decimal DeductibleAmount
    {
        get; set;
    }

    public bool IsActive
    {
        get; set;
    }

    public Tenant Tenant
    {
        get; set;
    } = default!;
    public ICollection<
    PolicyCoverageCategory>
    CoverageCategories
    { get; set; }
    = new List<
        PolicyCoverageCategory>();
}