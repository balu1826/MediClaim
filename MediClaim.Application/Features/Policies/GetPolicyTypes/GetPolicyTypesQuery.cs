using MediatR;
using MediClaim.Application.Common.Models;

namespace MediClaim.Application.Features.Policies.GetPolicyTypes;

public class GetPolicyTypesQuery
    : IRequest<CursorPage<PolicyTypeDto>>
{
    public Guid? Cursor { get; set; }

    public int PageSize { get; set; } = 10;
}