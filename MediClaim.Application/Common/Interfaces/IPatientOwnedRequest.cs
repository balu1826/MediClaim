namespace MediClaim.Application.Common.Interfaces
{
    public interface IPatientOwnedRequest
    {
        Guid PatientId
        {
            get;
        }
    }
}
