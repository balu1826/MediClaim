using MediatR;
using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Application
    .Features.Claims.Common;
using MediClaim.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using MediClaim.Application.Repositories;

namespace MediClaim.Application
    .Features.Claims.GetOfficerQueue;

public class GetOfficerQueueQueryHandler
    : IRequestHandler<
        GetOfficerQueueQuery,
        List<OfficerClaimQueueDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IUserRepository _currentUserService;
    private readonly IClaimRepository _claimRepository;
    public GetOfficerQueueQueryHandler(
        IApplicationDbContext context,
        IUserRepository currentUserService,
        IClaimRepository claimRepository)
    {
        _context = context;
        _currentUserService = currentUserService;
        _claimRepository = claimRepository; 
    }

    public async Task<List<OfficerClaimQueueDto>> Handle(
                GetOfficerQueueQuery request,
                CancellationToken cancellationToken)
    {
        var tenantId = _currentUserService.TenantId;
        var userId = _currentUserService.UserId;
        return await _claimRepository.GetOfficerQueueAsync
            (tenantId, userId, cancellationToken);
    }
}