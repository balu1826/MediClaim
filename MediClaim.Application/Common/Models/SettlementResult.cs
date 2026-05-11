namespace MediClaim.Application
    .Common.Models;

public class SettlementResult
{
    public bool Success
    {
        get; set;
    }

    public bool AlreadySettled
    {
        get; set;
    }

    public bool InsufficientBalance
    {
        get; set;
    }
}