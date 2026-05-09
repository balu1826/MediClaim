using MediClaim.Domain.Entities;

namespace MediClaim.Application.Common.Interfaces;

public interface IUserRepository
    : IRepository<User>
{
    Task<bool> EmailExistsAsync(
        string email);
}