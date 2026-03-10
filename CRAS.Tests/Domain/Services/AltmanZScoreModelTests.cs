using CRAS.Domain.Entities;
using CRAS.Domain.Enums;
using CRAS.Domain.Services;

namespace CRAS.Tests.Domain.Services;

/// <summary>
///     Contains unit tests for the <see cref="AltmanZScoreModel" /> class.
/// </summary>
/// <remarks>
///     This test suite verifies the accurate calculation of risk scores for public manufacturing companies.
///     It covers mathematical edge cases (like division by zero) and validates the classification
///     thresholds for Safe, Grey, and Distress zones.
/// </remarks>
public class AltmanZScoreModelTests
{
    private readonly AltmanZScoreModel _model = new();

    /// <summary>
    ///     Verifies that an <see cref="ArgumentException" /> is thrown when the total assets value is zero.
    /// </summary>
    /// <remarks>
    ///     This is a critical guard clause test to prevent <see cref="DivideByZeroException" />
    ///     when calculating asset-based ratios like Working Capital to Total Assets.
    /// </remarks>
    [Fact]
    public void CalculateRisk_ThrowsArgumentException_WhenTotalAssetsIsZero()
    {
        var statement = new FinancialStatement
        {
            TotalAssets = 0m,
            TotalLiabilities = 100000m
        };

        Assert.Throws<ArgumentException>(() => _model.CalculateRisk(statement));
    }

    /// <summary>
    ///     Verifies that an <see cref="ArgumentException" /> is thrown when the total liabilities value is zero.
    /// </summary>
    [Fact]
    public void CalculateRisk_ThrowsArgumentException_WhenTotalLiabilitiesIsZero()
    {
        var statement = new FinancialStatement
        {
            TotalAssets = 100000m,
            TotalLiabilities = 0m
        };

        Assert.Throws<ArgumentException>(() => _model.CalculateRisk(statement));
    }

    /// <summary>
    ///     Verifies that the model correctly calculates a safe risk level when provided with strong financials.
    /// </summary>
    [Fact]
    public void CalculateRisk_ReturnsSafeRiskLevel_WhenFinancialsAreStrong()
    {
        var statement = new FinancialStatement
        {
            TotalAssets = 100000m,
            TotalLiabilities = 40000m,
            WorkingCapital = 50000m,
            RetainedEarnings = 30000m,
            EBIT = 20000m,
            MarketValueEquity = 60000m,
            Sales = 120000m
        };

        var result = _model.CalculateRisk(statement);

        Assert.Equal("Altman Z-Score", result.Model);
        Assert.Equal(RiskLevel.Safe, result.RiskLevel);
        Assert.True(result.Score > 2.99m);
    }

    /// <summary>
    ///     Verifies that the model correctly assigns a distress risk level when financial ratios fall below the critical
    ///     threshold.
    /// </summary>
    [Fact]
    public void CalculateRisk_ReturnsDistressRiskLevel_WhenFinancialsAreWeak()
    {
        var statement = new FinancialStatement
        {
            TotalAssets = 100000m,
            TotalLiabilities = 150000m,
            WorkingCapital = -20000m,
            RetainedEarnings = -10000m,
            EBIT = -5000m,
            MarketValueEquity = 10000m,
            Sales = 50000m
        };

        var result = _model.CalculateRisk(statement);

        Assert.Equal(RiskLevel.Distress, result.RiskLevel);
        Assert.True(result.Score < 1.81m);
    }
}
