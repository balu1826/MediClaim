using MediatR;
using MediClaim.Application.Common.Interfaces;
using MediClaim.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Application.Features.Policies.GetPolicyTypes;

public class GetPolicyTypesQueryHandler : IRequestHandler<
        GetPolicyTypesQuery,
        CursorPage<PolicyTypeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentTenantService _tenantService;
    public GetPolicyTypesQueryHandler(
        IApplicationDbContext context,
        ICurrentTenantService tenantService)
    {
        _context = context;
        _tenantService = tenantService;
    }

    public async Task<CursorPage<PolicyTypeDto>> Handle(
                GetPolicyTypesQuery request,
                CancellationToken cancellationToken)
    {
        var query = _context.PolicyTypes
                .Where(x => x.TenantId == _tenantService.TenantId);
        if (request.Cursor.HasValue)
        {
            query = query.Where(x => x.PolicyTypeId > request.Cursor.Value);
        }
        var items =
            await query
                .OrderBy(x => x.PolicyTypeId)
                .Take(request.PageSize + 1)
                .Select(x =>
                    new PolicyTypeDto
                    {
                        PolicyTypeId = x.PolicyTypeId,
                        Name = x.Name,
                        CoverageLimit = x.AnnualCoverageLimit
                    })
                .ToListAsync(cancellationToken);
        var hasMore = items.Count > request.PageSize;
        if (hasMore)
        {
            items.RemoveAt(items.Count - 1);

        }
        return new CursorPage<PolicyTypeDto>
        {
            Items = items,
            HasMore = hasMore,
            NextCursor = items.LastOrDefault()?.PolicyTypeId
        };
    }
}