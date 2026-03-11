using CRAS.Domain.Enums;
using CRAS.Domain.Models;
using CRAS.Domain.Strategies;

namespace CRAS.Tests.Domain.Strategies;

/// <summary>
///     Contains unit tests for the <see cref="WorstCaseAggregation" /> strategy.
/// </summary>
/// <remarks>
///     These tests ensure that the strategy adheres to the most conservative approach,
///     prioritizing the detection of any potential distress signal.
/// </remarks>
public class WorstCaseAggregationTests
{
    private readonly WorstCaseAggregation _strategy = new();

    /// <summary>
    ///     Verifies that the strategy returns <see cref="RiskLevel.Distress" /> if at least one model indicates distress,
    ///     regardless of other positive results.
    /// </summary>
    [Fact]
    public void Aggregate_ReturnsDistress_WhenSingleModelIndicatesDistress()
    {
        var results = new List<RiskResult>
        {
            new() { Model = "Altman", RiskLevel = RiskLevel.Distress },
            new() { Model = "Ohlson", RiskLevel = RiskLevel.Safe }
        };

        var aggregated = _strategy.Aggregate(results);

        Assert.Equal(RiskLevel.Distress, aggregated.OverallRiskLevel);
    }

    /// <summary>
    ///     Verifies that the strategy returns <see cref="RiskLevel.Safe" /> only when all models unanimously agree.
    /// </summary>
    [Fact]
    public void Aggregate_ReturnsSafe_WhenAllModelsAreSafe()
    {
        var results = new List<RiskResult>
        {
            new() { Model = "Altman", RiskLevel = RiskLevel.Safe },
            new() { Model = "Ohlson", RiskLevel = RiskLevel.Safe }
        };

        var aggregated = _strategy.Aggregate(results);

        Assert.Equal(RiskLevel.Safe, aggregated.OverallRiskLevel);
    }
}