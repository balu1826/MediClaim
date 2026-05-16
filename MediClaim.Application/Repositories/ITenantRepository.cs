using MediClaim.Domain.Entities;

namespace MediClaim.Application.Repositories
{
    public interface ITenantRepository
    {
        Task<Tenant?> GetTenantByIdAsync(Guid? id, CancellationToken cancellationToken);
        Task<bool> SlugExistsAsync(string slug);
        Task<Guid> AddAsync(Tenant tenant);

    }
}
