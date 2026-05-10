namespace MediClaim.Application
    .Common.Models;

public class FraudEvaluationResult
{
    public int Score
    {
        get; set;
    }

    public bool RequiresReview
    {
        get; set;
    }
    public List<string>
      TriggeredRules
    { get; set; }
      = [];
}