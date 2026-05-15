using MediatR;

namespace MediClaim.Application.Features.Reports.ClaimSummary
{
    public class GetClaimsSummaryQuery
      : IRequest<List<ClaimsSummaryDto>>
    {
    }
}
