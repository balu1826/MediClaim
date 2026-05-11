using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Application
    .Common.Models;
using MediClaim.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Infrastructure
    .Settlement;

public class ClaimSettlementService
    : IClaimSettlementService
{
    private readonly ApplicationDbContext
        _context;

    public ClaimSettlementService(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<
        SettlementResult>
            SettleAsync(
                Guid claimId,
                decimal approvedAmount,
                Guid policyId,
                CancellationToken cancellationToken)
    {
        var claimIdParam =
            new SqlParameter(
                "@ClaimId",
                claimId);

        var amountParam =
            new SqlParameter(
                "@ApprovedAmount",
                approvedAmount);

        var policyIdParam =
            new SqlParameter(
                "@PolicyId",
                policyId);

        var result =
            await _context.Database
                .SqlQueryRaw<int>(
                    @"
EXEC usp_SettleClaim
    @ClaimId,
    @ApprovedAmount,
    @PolicyId
",
                    claimIdParam,
                    amountParam,
                    policyIdParam)
                .FirstAsync(
                    cancellationToken);

        return result switch
        {
            0 => new SettlementResult
            {
                Success = true
            },

            1 => new SettlementResult
            {
                InsufficientBalance = true
            },

            2 => new SettlementResult
            {
                AlreadySettled = true,
                Success = true
            },

            _ => throw new InvalidOperationException(
                "Unknown settlement result")
        };
    }
}