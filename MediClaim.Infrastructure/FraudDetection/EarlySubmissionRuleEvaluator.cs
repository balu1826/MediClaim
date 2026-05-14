using MediClaim.Application
    .Common.Interfaces;
using MediClaim.Domain.Entities;
using MediClaim.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MediClaim.Infrastructure
    .FraudDetection.Rules;

public class EarlySubmissionRuleEvaluator : IFraudRuleEvaluator
{
    private readonly ApplicationDbContext _context;
    public EarlySubmissionRuleEvaluator(ApplicationDbContext context)
    {
        _context = context;
    }
    public string RuleName => GetType().Name;
    public async Task<int>EvaluateAsync(Claim claim, CancellationToken cancellationToken)
    {
        var policy = await _context.Policies.SingleAsync(x =>x.PolicyId ==claim.PolicyId);
        var hours =(claim.CreatedAt -policy.CreatedAt).TotalHours;
        return hours < 24? 10: 0;
    }
}