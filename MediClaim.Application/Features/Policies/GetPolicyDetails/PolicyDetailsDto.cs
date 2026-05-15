using MediClaim.Application.Common.Models;

namespace MediClaim.Application.Features.Policies.GetPolicyDetails;

public class PolicyDetailsDto
{
    public Guid PolicyId { get; set; }

    public decimal CoverageLimit { get; set; }

    public decimal RemainingBalance { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public CursorPage<ClaimHistoryDto> ClaimHistory
    {
        get;
        set;
    } = default!;
}