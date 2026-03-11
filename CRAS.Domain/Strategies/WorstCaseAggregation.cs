using CRAS.Domain.Enums;
using CRAS.Domain.Interfaces;
using CRAS.Domain.Models;

namespace CRAS.Domain.Strategies;

/// <summary>
///     Implements a worst-case strategy for aggregating financial risk assessments.
/// </summary>
/// <remarks>
///     This highly conservative strategy prioritizes safety by determining the overall risk level as
///     <see cref="RiskLevel.Distress" /> if even a single individual model indicates distress.
///     It requires unanimous agreement among all models to classify the risk as <see cref="RiskLevel.Safe" />,
///     otherwise securely defaulting to <see cref="RiskLevel.Grey" />.
/// </remarks>
public class WorstCaseAggregation : IRiskAggregationStrategy
{
    /// <summary>
    ///     Aggregates a collection of individual risk results using the worst-case rule.
    /// </summary>
    /// <param name="results">A read-only collection of <see cref="RiskResult" /> outcomes to be evaluated.</param>
    /// <returns>
    ///     An <see cref="AggregatedRiskResult" /> containing the overall determined risk level
    ///     and the underlying individual results.
    /// </returns>
    public AggregatedRiskResult Aggregate(IReadOnlyCollection<RiskResult> results)
    {
        return new AggregatedRiskResult
        {
            OverallRiskLevel = DetermineOverallRisk(results),
            IndividualResults = results
        };
    }

    private static RiskLevel DetermineOverallRisk(IReadOnlyCollection<RiskResult> results)
    {
        if (results.Count == 0) return RiskLevel.Grey;

        if (results.Any(r => r.RiskLevel == RiskLevel.Distress)) return RiskLevel.Distress;

        return results.All(r => r.RiskLevel == RiskLevel.Safe) ? RiskLevel.Safe : RiskLevel.Grey;
    }
}