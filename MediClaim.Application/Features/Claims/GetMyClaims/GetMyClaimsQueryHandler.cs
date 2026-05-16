using MediatR;
using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Application
    .Features.Claims.Common;
using MediClaim.Application.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Application
    .Features.Claims.GetMyClaims;

public class GetMyClaimsQueryHandler
    : IRequestHandler<
        GetMyClaimsQuery,
        List<ClaimDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserRepository _currentUserService;
    private readonly IClaimRepository _claimRepository;

    public GetMyClaimsQueryHandler(
        IApplicationDbContext context,
        IUserRepository currentUserService,
        IClaimRepository claimRepository)
    {
        _context = context;
        _currentUserService = currentUserService;
        _claimRepository = claimRepository;
    }

    public async Task<List<ClaimDto>> Handle(
            GetMyClaimsQuery request,
            CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var tenantId = _currentUserService.TenantId;
        return await _claimRepository.GetMyClaimsAsync(
            userId,
            tenantId,
            cancellationToken);
    }
}