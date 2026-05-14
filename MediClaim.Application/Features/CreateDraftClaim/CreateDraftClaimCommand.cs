using MediatR;

namespace MediClaim.Application
    .Features.Claims.CreateDraftClaim;

public class CreateDraftClaimCommand
    : IRequest<Guid>
{
    public Guid PolicyId
    {
        get; set;
    }
    public Guid ProviderId
    {
        get; set;
    }

    public decimal Amount
    {
        get; set;
    }

    public string DiagnosisCode
    {
        get; set;
    } = default!;

    public string TreatmentCategory
    {
        get; set;
    } = default!;

    public string Description
    {
        get; set;
    } = default!;
}