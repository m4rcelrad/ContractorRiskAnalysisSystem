using CRAS.Domain.Enums;
using CRAS.Domain.Interfaces;
using CRAS.Domain.Models;

namespace CRAS.Domain.Strategies;

/// <summary>
///     Implements a majority vote strategy for aggregating financial risk assessments.
/// </summary>
/// <remarks>
///     This strategy determines the overall risk level by requiring at least two individual models
///     to agree on a <see cref="RiskLevel.Low" /> or <see cref="RiskLevel.Critical" /> classification.
///     If no clear majority is reached, or if the result set is empty, it securely defaults to
///     <see cref="RiskLevel.Moderate" />.
/// </remarks>
public class MajorityVoteAggregation : IRiskAggregationStrategy
{
    /// <summary>
    ///     Aggregates a collection of individual risk results using the majority vote rule.
    /// </summary>
    /// <param name="results">A read-only collection of <see cref="RiskResult" /> outcomes to be evaluated.</param>
    /// <returns>
    ///     An <see cref="AggregatedRiskResult" /> containing the overall determined risk level
    ///     and the underlying individual results.
    /// </returns>
    public AggregatedRiskResult Aggregate(IReadOnlyCollection<RiskResult> results) => new()
    {
        OverallRiskLevel = DetermineOverallRisk(results),
        IndividualResults = results
    };

    private static RiskLevel DetermineOverallRisk(IReadOnlyCollection<RiskResult> results)
    {
        if (results.Count == 0) return RiskLevel.Moderate;

        var distressCount = results.Count(r => r.RiskLevel == RiskLevel.Critical);
        var safeCount = results.Count(r => r.RiskLevel == RiskLevel.Low);

        if (distressCount >= 2) return RiskLevel.Critical;
        return safeCount >= 2 ? RiskLevel.Low : RiskLevel.Moderate;
    }
}
