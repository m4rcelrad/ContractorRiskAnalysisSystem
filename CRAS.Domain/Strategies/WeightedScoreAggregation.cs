using CRAS.Domain.Enums;
using CRAS.Domain.Interfaces;
using CRAS.Domain.Models;

namespace CRAS.Domain.Strategies;

/// <summary>
///     Implements a weighted score strategy for aggregating financial risk assessments.
/// </summary>
/// <remarks>
///     This strategy assigns configurable importance (weights) to individual risk models.
///     It calculates a weighted average of the categorized risk levels to determine the overall consensus.
///     If a model's weight is not explicitly configured, it securely defaults to a neutral weight of 1.0.
/// </remarks>
/// <param name="modelWeights">A read-only dictionary mapping model names to their respective decimal weights.</param>
public class WeightedScoreAggregation(IReadOnlyDictionary<string, decimal> modelWeights) : IRiskAggregationStrategy
{
    /// <summary>
    ///     Aggregates a collection of individual risk results by calculating a weighted average of their risk levels.
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

    private RiskLevel DetermineOverallRisk(IReadOnlyCollection<RiskResult> results)
    {
        if (results.Count == 0) return RiskLevel.Grey;

        var totalWeight = 0m;
        var weightedScoreSum = 0m;

        foreach (var result in results)
        {
            var weight = modelWeights.GetValueOrDefault(result.Model, 1.0m);

            var levelScore = result.RiskLevel switch
            {
                RiskLevel.Safe => 1m,
                RiskLevel.Grey => 2m,
                RiskLevel.Distress => 3m,
                _ => 2m
            };

            weightedScoreSum += levelScore * weight;
            totalWeight += weight;
        }

        if (totalWeight == 0m) return RiskLevel.Grey;

        var finalScore = weightedScoreSum / totalWeight;

        return finalScore switch
        {
            < 1.5m => RiskLevel.Safe,
            >= 1.5m and < 2.5m => RiskLevel.Grey,
            _ => RiskLevel.Distress
        };
    }
}