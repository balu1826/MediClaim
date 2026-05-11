namespace MediClaim.Application.Common.Interfaces
{
    public interface ICurrentTenantService
    {
        Guid? TenantId { get; set; }

        bool IsSuperAdmin { get; }
    }
}
