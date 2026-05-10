using MediatR;

namespace MediClaim.Application
    .Features.Policies.EnrollPolicy;

public class EnrollPolicyCommand
    : IRequest<Guid>
{
    public Guid PatientId
    {
        get; set;
    }

    public Guid PolicyTypeId
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
}