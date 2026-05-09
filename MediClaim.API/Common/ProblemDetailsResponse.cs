namespace MediClaim.API.Common;

public class ProblemDetailsResponse
{
    public string Title { get; set; }
        = default!;

    public int Status { get; set; }

    public string? Detail { get; set; }

    public string TraceId { get; set; }
        = default!;

    public List<string>? Errors { get; set; }
}