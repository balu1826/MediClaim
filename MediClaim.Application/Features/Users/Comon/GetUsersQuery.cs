using MediatR;
using MediClaim.Application
    .Features.Users.Common;

namespace MediClaim.Application
    .Features.Users.Queries.GetUsers;

public class GetUsersQuery
    : IRequest<List<UserDto>>
{
}