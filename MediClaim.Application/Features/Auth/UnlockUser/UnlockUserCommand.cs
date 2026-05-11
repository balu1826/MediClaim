using MediatR;

namespace MediClaim.Application
    .Features.Auth
    .UnlockUser;

public class UnlockUserCommand
    : IRequest
{
    public Guid UserId
    {
        get; set;
    }
}