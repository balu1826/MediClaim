using MediClaim.Application.Common.Interfaces;
using MediClaim.Domain.Entities;
using MediClaim.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MediClaim.Infrastructure.Repositories;

public class UserRepository
    : Repository<User>,
      IUserRepository
{
    private readonly IHttpContextAccessor
        _httpContextAccessor;
    public UserRepository(
        ApplicationDbContext context,IHttpContextAccessor httpContextAccessor)
        : base(context)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    public Guid UserId =>
       Guid.Parse(
           _httpContextAccessor
               .HttpContext!
               .User
               .FindFirstValue(
                   ClaimTypes.NameIdentifier)!);

    public Guid TenantId =>
        Guid.Parse(
            _httpContextAccessor
                .HttpContext!
                .User
                .FindFirstValue(
                    "tenant_id")!);

    public string Email =>
        _httpContextAccessor
            .HttpContext!
            .User
            .FindFirstValue(
                ClaimTypes.Email)!;

    public string Role =>
        _httpContextAccessor
            .HttpContext!
            .User
            .FindFirstValue(
                ClaimTypes.Role)!;
    public async Task<User?> EmailExistsAsync(
        string email)
    {
        return await _dbSet
        .FirstOrDefaultAsync(x =>
            x.Email == email);
    }
}