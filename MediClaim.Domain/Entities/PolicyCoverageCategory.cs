using MediClaim.Domain.Common;

namespace MediClaim.Domain.Entities;

public class PolicyCoverageCategory
    : BaseEntity
{
    public Guid PolicyCoverageCategoryId
    {
        get; set;
    }

    public Guid PolicyTypeId
    {
        get; set;
    }

    public string Name
    {
        get; set;
    } = default!;

    public PolicyType PolicyType
    {
        get; set;
    } = default!;
}