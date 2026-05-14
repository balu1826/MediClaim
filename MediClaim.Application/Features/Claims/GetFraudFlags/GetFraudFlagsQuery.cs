using MediatR;

namespace MediClaim.Application.Features.Claims.GetFraudFlags
{
    public class GetFraudFlagsQuery : IRequest<List<FraudFlagDto>>
    {
    }
}
