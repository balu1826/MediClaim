using MediatR;

namespace MediClaim.Application.Features.Reports.OfficerWorkLoad
{
    public class GetOfficerWorkloadQuery :
        IRequest<List<OfficerWorkloadDto>>
    {
    }
}
