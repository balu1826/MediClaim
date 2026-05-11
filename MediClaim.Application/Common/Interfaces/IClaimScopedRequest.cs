namespace MediClaim.Application.Common.Interfaces
{
    public interface IClaimScopedRequest
    {
        Guid ClaimId
        {
            get;
        }
    }
}
