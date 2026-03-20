using CRAS.Domain.Enums;
using CRAS.Domain.Strategies;
using CRAS.Domain.ValueObjects;

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
    ///     Verifies that the strategy returns <see cref="RiskLevel.Critical" /> if at least one model indicates distress,
    ///     regardless of other positive results.
    /// </summary>
    [Fact]
    public void Aggregate_ReturnsDistress_WhenSingleModelIndicatesDistress()
    {
        var results = new List<RiskResult>
        {
            new() { Model = "Altman", RiskLevel = RiskLevel.Critical },
            new() { Model = "Ohlson", RiskLevel = RiskLevel.Low }
        };

        var aggregated = _strategy.Aggregate(results);

        Assert.Equal(RiskLevel.Critical, aggregated.OverallRiskLevel);
    }

    /// <summary>
    ///     Verifies that the strategy returns <see cref="RiskLevel.Low" /> only when all models unanimously agree.
    /// </summary>
    [Fact]
    public void Aggregate_ReturnsSafe_WhenAllModelsAreSafe()
    {
        var results = new List<RiskResult>
        {
            new() { Model = "Altman", RiskLevel = RiskLevel.Low },
            new() { Model = "Ohlson", RiskLevel = RiskLevel.Low }
        };

        var aggregated = _strategy.Aggregate(results);

        Assert.Equal(RiskLevel.Low, aggregated.OverallRiskLevel);
    }
}
