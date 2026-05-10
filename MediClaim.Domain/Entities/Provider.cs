using MediClaim.Domain.Common;

namespace MediClaim.Domain.Entities;

public class Provider
    : BaseEntity
{
    public Guid ProviderId
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

    public string Specialty
    {
        get; set;
    } = default!;

    public Tenant Tenant
    {
        get; set;
    } = default!;
}