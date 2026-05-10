using MediatR;
using MediClaim.Application
    .Features.Claims.Common;

namespace MediClaim.Application
    .Features.Claims.GetOfficerQueue;

public class GetOfficerQueueQuery
    : IRequest<
        List<OfficerClaimQueueDto>>
{
}