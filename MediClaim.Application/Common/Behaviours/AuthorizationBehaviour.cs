using MediatR;
using MediClaim.Application
    .Common.Exceptions;
using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Application
    .Common.Behaviours;

public class AuthorizationBehaviour<
    TRequest,
    TResponse>
    : IPipelineBehavior<
        TRequest,
        TResponse>

    where TRequest : notnull
{
    private readonly IUserRepository
        _currentUserService;

    private readonly IApplicationDbContext
        _context;

    public AuthorizationBehaviour(
        IUserRepository currentUserService,
        IApplicationDbContext context)
    {
        _currentUserService =
            currentUserService;

        _context = context;
    }

    public async Task<TResponse>
        Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
    {
        // Apply only to claim-scoped requests

        if (request is IClaimScopedRequest claimRequest)
        {
            var claim =
                await _context.Claims
                    .AsNoTracking()
                    .FirstOrDefaultAsync(
                        x => x.ClaimId == claimRequest.ClaimId,
                        cancellationToken);

            if (claim is null)
            {
                throw new NotFoundException(
                    "Claim not found");
            }

            // Patients can only access their own claims

            if (_currentUserService.Role == UserRole.Patient.ToString()
                && claim.UserId != _currentUserService.UserId)
            {
                throw new ForbiddenAccessException(
                    "Access denied");
            }
        }

        return await next();
    }
}