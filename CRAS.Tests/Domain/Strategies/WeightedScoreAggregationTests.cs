using System.Collections.ObjectModel;
using CRAS.Domain.Enums;
using CRAS.Domain.Models;
using CRAS.Domain.Strategies;

namespace CRAS.Tests.Domain.Strategies;

/// <summary>
///     Contains unit tests for the <see cref="WeightedScoreAggregation" /> strategy.
/// </summary>
/// <remarks>
///     These tests verify that the overall risk level is calculated based on model importance,
///     ensuring that models with higher weights have a greater impact on the final decision.
/// </remarks>
public class WeightedScoreAggregationTests
{
    /// <summary>
    ///     Verifies that a model with a dominant weight dictates the overall risk level.
    /// </summary>
    [Fact]
    public void Aggregate_FavorsHeavierWeightModel_WhenWeightsAreUneven()
    {
        var weights = new ReadOnlyDictionary<string, decimal>(new Dictionary<string, decimal>
        {
            ["ImportantModel"] = 10.0m,
            ["MinorModel"] = 1.0m
        });

        var strategy = new WeightedScoreAggregation(weights);

        var aggregated = strategy.Aggregate(
        [
            new RiskResult { Model = "ImportantModel", RiskLevel = RiskLevel.Distress },
            new RiskResult { Model = "MinorModel", RiskLevel = RiskLevel.Safe }
        ]);

        Assert.Equal(RiskLevel.Distress, aggregated.OverallRiskLevel);
    }

    /// <summary>
    ///     Verifies that the strategy uses the default weight of 1.0 when a model name is not found in the dictionary.
    /// </summary>
    [Fact]
    public void Aggregate_UsesDefaultWeight_WhenModelIsMissingFromDictionary()
    {
        var strategy =
            new WeightedScoreAggregation(new ReadOnlyDictionary<string, decimal>(new Dictionary<string, decimal>()));

        var aggregated = strategy.Aggregate(
        [
            new RiskResult { Model = "UnknownModel", RiskLevel = RiskLevel.Safe }
        ]);

        Assert.Equal(RiskLevel.Safe, aggregated.OverallRiskLevel);
    }
}