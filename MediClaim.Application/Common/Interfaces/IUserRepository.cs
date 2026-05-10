using MediClaim.Domain.Entities;

namespace MediClaim.Application.Common.Interfaces;

public interface IUserRepository
    : IRepository<User>
{
    Guid UserId { get; }

    Guid TenantId { get; }

    string Email { get; }

    string Role { get; }
    Task<User?> EmailExistsAsync(
        string email);
}