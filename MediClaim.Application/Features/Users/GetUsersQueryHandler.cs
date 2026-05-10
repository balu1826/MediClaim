using MediatR;
using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Application
    .Features.Users.Common;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Application
    .Features.Users.Queries.GetUsers;

public class GetUsersQueryHandler
    : IRequestHandler<
        GetUsersQuery,
        List<UserDto>>
{
    private readonly IApplicationDbContext
        _context;

    private readonly IUserRepository
        _currentUserService;

    public GetUsersQueryHandler(
        IApplicationDbContext context,
        IUserRepository currentUserService)
    {
        _context = context;

        _currentUserService =
            currentUserService;
    }

    public async Task<List<UserDto>>
        Handle(
            GetUsersQuery request,
            CancellationToken cancellationToken)
    {
        return await _context.Users
            .Where(x =>
                x.TenantId ==
                _currentUserService
                    .TenantId)
            .Select(x =>
                new UserDto
                {
                    UserId = x.UserId,

                    Email = x.Email,

                    Role = x.Role,

                    ApprovalLimit =
                        x.ApprovalLimit,

                    IsFraudSpecialist =
                        x.IsFraudSpecialist,

                    IsLocked =
                        x.IsLocked
                })
            .ToListAsync(
                cancellationToken);
    }
}