using CRAS.Domain.Enums;
using CRAS.Domain.Strategies;
using CRAS.Domain.ValueObjects;

namespace CRAS.Tests.Domain.Strategies;

/// <summary>
///     Contains unit tests for the <see cref="MajorityVoteAggregation" /> strategy.
/// </summary>
/// <remarks>
///     These tests verify the core voting logic, ensuring that a majority (at least 2)
///     of agreeing models is required to upgrade or downgrade the overall risk status.
/// </remarks>
/// <summary>
///     Contains unit tests for the <see cref="MajorityVoteAggregation" /> strategy.
/// </summary>
/// <remarks>
///     These tests verify the core voting logic, ensuring that a majority (at least 2)
///     of agreeing models is required to upgrade or downgrade the overall risk status.
/// </remarks>
public class MajorityVoteAggregationTests
{
    private readonly MajorityVoteAggregation _strategy = new();

    /// <summary>
    ///     Verifies that the strategy returns <see cref="RiskLevel.Critical" /> when at least two models indicate distress.
    /// </summary>
    [Fact]
    public void Aggregate_ReturnsDistress_WhenAtLeastTwoModelsAreDistress()
    {
        RiskResult[] results =
        [
            new() { Model = "M1", RiskLevel = RiskLevel.Critical },
            new() { Model = "M2", RiskLevel = RiskLevel.Critical },
            new() { Model = "M3", RiskLevel = RiskLevel.Low }
        ];

        var aggregated = _strategy.Aggregate(results);

        Assert.Equal(RiskLevel.Critical, aggregated.OverallRiskLevel);
        Assert.Equal(3, aggregated.IndividualResults.Count);
    }

    /// <summary>
    ///     Verifies that the strategy returns <see cref="RiskLevel.Low" /> when at least two models indicate safe status.
    /// </summary>
    [Fact]
    public void Aggregate_ReturnsSafe_WhenAtLeastTwoModelsAreSafe()
    {
        RiskResult[] results =
        [
            new() { Model = "M1", RiskLevel = RiskLevel.Low },
            new() { Model = "M2", RiskLevel = RiskLevel.Low },
            new() { Model = "M3", RiskLevel = RiskLevel.Moderate }
        ];

        var aggregated = _strategy.Aggregate(results);

        Assert.Equal(RiskLevel.Low, aggregated.OverallRiskLevel);
    }

    /// <summary>
    ///     Verifies that the strategy returns <see cref="RiskLevel.Moderate" /> when there is no clear majority
    ///     between safe and distress outcomes.
    /// </summary>
    [Fact]
    public void Aggregate_ReturnsGrey_WhenNoClearMajorityExists()
    {
        RiskResult[] results =
        [
            new() { Model = "M1", RiskLevel = RiskLevel.Low },
            new() { Model = "M2", RiskLevel = RiskLevel.Critical },
            new() { Model = "M3", RiskLevel = RiskLevel.Moderate }
        ];

        var aggregated = _strategy.Aggregate(results);

        Assert.Equal(RiskLevel.Moderate, aggregated.OverallRiskLevel);
    }

    /// <summary>
    ///     Verifies that the strategy defaults to <see cref="RiskLevel.Moderate" /> when the input collection is empty.
    /// </summary>
    [Fact]
    public void Aggregate_ReturnsGrey_WhenResultsAreEmpty()
    {
        RiskResult[] results = [ ];

        var aggregated = _strategy.Aggregate(results);

        Assert.Equal(RiskLevel.Moderate, aggregated.OverallRiskLevel);
    }
}
