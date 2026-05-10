using MediatR;
using MediClaim.Application
    .Features.Claims.Common;

namespace MediClaim.Application
    .Features.Claims.GetMyClaims;

public class GetMyClaimsQuery
    : IRequest<List<ClaimDto>>
{
}