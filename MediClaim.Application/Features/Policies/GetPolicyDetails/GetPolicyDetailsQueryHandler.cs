using MediatR;
using MediClaim.Application.Common.Interfaces;
using MediClaim.Application.Common.Models;
using MediClaim.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using MediClaim.Application.Common.Exceptions;
namespace MediClaim.Application.Features.Policies.GetPolicyDetails;

public class GetPolicyDetailsQueryHandler : IRequestHandler<
                                        GetPolicyDetailsQuery,
                                        PolicyDetailsDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserRepository _userRepository;
    public GetPolicyDetailsQueryHandler(
        IApplicationDbContext context,
        IUserRepository userRepository)
    {
        _context = context;
        _userRepository = userRepository;
    }

    public async Task<PolicyDetailsDto> Handle(
        GetPolicyDetailsQuery request,
        CancellationToken cancellationToken)
    {
        var policy = await _context.Policies
                .SingleAsync(
                    x => x.PolicyId == request.PolicyId,
                    cancellationToken);
        if (policy is null)
        {
            throw new NotFoundException("Policy Not  Found");
        }
        if ((_userRepository.Role == nameof(UserRole.Patient)
            && policy.PatientId != _userRepository.UserId) ||
            (_userRepository.TenantId == _userRepository.UserId))
        {
            throw new ForbiddenAccessException("Forbidden Access");
        }

        var settledAmount = await _context.Claims
                .Where(x => x.PolicyId == policy.PolicyId && x.Status == ClaimStatus.Settled)
                .SumAsync(x => x.Amount, cancellationToken);
        var claimsQuery = _context.Claims
                        .Where(x => x.PolicyId == policy.PolicyId);
        if (request.Cursor.HasValue)
        {
            claimsQuery = claimsQuery
                          .Where(x => x.ClaimId > request.Cursor.Value);
        }
        var claims =
            await claimsQuery
                .OrderBy(x => x.ClaimId)
                .Take(request.PageSize + 1)
                .Select(x =>
                    new ClaimHistoryDto
                    {
                        ClaimId = x.ClaimId,
                        Amount = x.Amount,
                        Status = x.Status.ToString(),
                        CreatedAt = x.CreatedAt
                    })
                .ToListAsync(cancellationToken);
        var hasMore = claims.Count > request.PageSize;
        if (hasMore)
        {
            claims.RemoveAt(claims.Count - 1);
        }
        return new PolicyDetailsDto
        {
            PolicyId = policy.PolicyId,
            CoverageLimit = policy.AnnualLimit,
            RemainingBalance = policy.AnnualLimit - settledAmount,
            StartDate = policy.StartDate,
            EndDate = policy.EndDate,
            ClaimHistory =
                new CursorPage<
                    ClaimHistoryDto>
                {
                    Items = claims,
                    HasMore = hasMore,
                    NextCursor = claims.LastOrDefault()?.ClaimId
                }
        };
    }
}