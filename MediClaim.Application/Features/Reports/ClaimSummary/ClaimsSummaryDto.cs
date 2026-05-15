namespace MediClaim.Application.Features.Reports.ClaimSummary
{
    public class ClaimsSummaryDto
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public int TotalClaims { get; set; }

        public int ApprovedClaims { get; set; }

        public decimal ApprovalRate { get; set; }
    }
}
