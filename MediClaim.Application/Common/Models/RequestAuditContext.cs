namespace MediClaim.Application
    .Common.Models;

public class RequestAuditContext
{
    public string? RequestBody
    {
        get; set;
    }

    public string? ResponseBody
    {
        get; set;
    }
}