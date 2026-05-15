namespace MediClaim.Application.Features.Reports.OfficerWorkLoad
{
    public class OfficerWorkloadDto
    {
        public Guid OfficerId { get; set; }
        public int TotalClaims { get; set; } = 0;
        public int ApprovedClaims { get; set; } = 0;
        public int RejectedClaims { get; set; } = 0;
    }
}
