using MediatR;
using MediClaim.Application.Common.Interfaces;

namespace MediClaim.Application.Features.Policies.GetPolicyDetails;

public class GetPolicyDetailsQuery
    : IRequest<PolicyDetailsDto>
{
    public Guid PolicyId { get; set; }

    public Guid? Cursor { get; set; }

    public int PageSize { get; set; } = 10;
}