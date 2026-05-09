using MediClaim.Domain.Common;
using MediClaim.Domain.Enums;

namespace MediClaim.Domain.Entities;

public class Tenant : BaseEntity
{
    public Guid TenantId { get; set; }

    public string Name { get; set; } = default!;

    public string Slug { get; set; } = default!;

    public TenantStatus Status { get; set; }

    public string? SettingsJson { get; set; }
}