using MediatR;

namespace MediClaim.Application
    .Features.Auth.Commands.RegisterTenant;

public class RegisterTenantCommand
    : IRequest<Guid>
{
    public string TenantName { get; set; }
        = default!;

    public string Slug { get; set; }
        = default!;

    public string AdminEmail { get; set; }
        = default!;

    public string Password { get; set; }
        = default!;
    public string Ssn { get; set; }
    = default!;
    public int FraudThreshold
    {
        get;
        set;
    }
}