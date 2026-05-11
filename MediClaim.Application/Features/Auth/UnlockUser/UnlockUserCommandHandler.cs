using MediatR;
using MediClaim.Application
    .Common.Exceptions;
using MediClaim.Application
    .Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Application
    .Features.Auth
    .UnlockUser;

public class UnlockUserCommandHandler
    : IRequestHandler<
        UnlockUserCommand>
{
    private readonly IApplicationDbContext
        _context;

    private readonly IUnitOfWork
        _unitOfWork;

    public UnlockUserCommandHandler(
        IApplicationDbContext context,
        IUnitOfWork unitOfWork)
    {
        _context = context;

        _unitOfWork =
            unitOfWork;
    }

    public async Task Handle(
        UnlockUserCommand request,
        CancellationToken cancellationToken)
    {
        var user =
            await _context.Users
                .FirstOrDefaultAsync(
                    x =>
                        x.UserId ==
                            request.UserId,
                    cancellationToken);

        if (user is null)
        {
            throw new NotFoundException(
                "User not found");
        }

        user.IsLocked = false;

        user.LockedAt = null;

        user.FailedLoginCount = 0;

        await _unitOfWork
            .SaveChangesAsync(
                cancellationToken);
    }
}