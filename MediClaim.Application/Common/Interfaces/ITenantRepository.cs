using MediClaim.Domain.Entities;

namespace MediClaim.Application.Common.Interfaces;

public interface ITenantRepository
    : IRepository<Tenant>
{
    Task<bool> SlugExistsAsync(
        string slug);
}