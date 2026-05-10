using MediClaim.Domain.Entities;

namespace MediClaim.Application
    .Common.Interfaces;

public interface IClaimAssignmentService
{
    Task AssignAsync(
        Claim claim,
        CancellationToken cancellationToken);
}