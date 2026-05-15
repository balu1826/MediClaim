namespace MediClaim.Application.Features.Policies.GetPolicyTypes;

public class PolicyTypeDto
{
    public Guid PolicyTypeId { get; set; }

    public string Name { get; set; } = default!;

    public decimal CoverageLimit { get; set; }
}