using MediatR;

namespace MediClaim.Application
    .Features.Policies.CreatePolicyType;

public class CreatePolicyTypeCommand
    : IRequest<Guid>
{
    public string Name { get; set; }
        = default!;

    public decimal AnnualCoverageLimit
    {
        get; set;
    }

    public decimal DeductibleAmount
    {
        get; set;
    }
    public List<string>
    CoverageCategories
    { get; set; }
    = [];
}