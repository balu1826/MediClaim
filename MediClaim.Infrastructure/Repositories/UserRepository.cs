using MediClaim.Application.Common.Interfaces;
using MediClaim.Domain.Entities;
using MediClaim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Infrastructure.Repositories;

public class UserRepository
    : Repository<User>,
      IUserRepository
{
    public UserRepository(
        ApplicationDbContext context)
        : base(context)
    {
    }

    public async Task<bool> EmailExistsAsync(
        string email)
    {
        return await _dbSet.AnyAsync(x =>
            x.Email == email);
    }
}