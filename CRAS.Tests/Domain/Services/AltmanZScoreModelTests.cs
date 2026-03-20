using CRAS.Domain.Entities;
using CRAS.Domain.Enums;
using CRAS.Domain.RiskModels;
using CRAS.Domain.Services;

namespace CRAS.Tests.Domain.Services;

/// <summary>
///     Contains unit tests for the <see cref="AltmanZScoreModel" /> class.
/// </summary>
/// <remarks>
///     This test suite verifies the accurate calculation of risk scores for public manufacturing companies.
///     It covers mathematical edge cases (like division by zero) and validates the classification
///     thresholds for Low, Moderate, and Critical zones.
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
        var statement = CreateTestStatement(0m, 100000m);

        Assert.Throws<ArgumentException>(() => _model.CalculateRisk(statement));
    }

    /// <summary>
    ///     Verifies that an <see cref="ArgumentException" /> is thrown when the total liabilities value is zero.
    /// </summary>
    [Fact]
    public void CalculateRisk_ThrowsArgumentException_WhenTotalLiabilitiesIsZero()
    {
        var statement = CreateTestStatement(100000m, 0m);

        Assert.Throws<ArgumentException>(() => _model.CalculateRisk(statement));
    }

    /// <summary>
    ///     Verifies that the model correctly calculates a safe risk level when provided with strong financials.
    /// </summary>
    [Fact]
    public void CalculateRisk_ReturnsSafeRiskLevel_WhenFinancialsAreStrong()
    {
        var statement = CreateTestStatement(
            100000m,
            40000m,
            50000m,
            30000m,
            20000m,
            60000m,
            120000m);

        var result = _model.CalculateRisk(statement);

        Assert.Equal("Altman Z-Score", result.Model);
        Assert.Equal(RiskLevel.Low, result.RiskLevel);
        Assert.True(result.Score > 2.99m);
    }

    /// <summary>
    ///     Verifies that the model correctly assigns a distress risk level when financial ratios fall below the critical
    ///     threshold.
    /// </summary>
    [Fact]
    public void CalculateRisk_ReturnsDistressRiskLevel_WhenFinancialsAreWeak()
    {
        var statement = CreateTestStatement(
            100000m,
            150000m,
            -20000m,
            -10000m,
            -5000m,
            10000m,
            50000m);

        var result = _model.CalculateRisk(statement);

        Assert.Equal(RiskLevel.Critical, result.RiskLevel);
        Assert.True(result.Score < 1.81m);
    }

    /// <summary>
    ///     Creates a test financial statement with the specified values.
    /// </summary>
    /// <param name="totalAssets"></param>
    /// <param name="totalLiabilities"></param>
    /// <param name="workingCapital"></param>
    /// <param name="retainedEarnings"></param>
    /// <param name="ebit"></param>
    /// <param name="marketValueEquity"></param>
    /// <param name="sales"></param>
    /// <returns>
    ///     <see cref="FinancialStatement" />
    /// </returns>
    private static FinancialStatement CreateTestStatement(
        decimal totalAssets,
        decimal totalLiabilities,
        decimal workingCapital = 0m,
        decimal retainedEarnings = 0m,
        decimal ebit = 0m,
        decimal marketValueEquity = 0m,
        decimal sales = 0m) => new()
    {
        ContractorId = Guid.NewGuid(),
        Year = DateTime.UtcNow.Year,
        TotalAssets = totalAssets,
        TotalLiabilities = totalLiabilities,
        CurrentAssets = 0m,
        CurrentLiabilities = 0m,
        WorkingCapital = workingCapital,
        RetainedEarnings = retainedEarnings,
        EBIT = ebit,
        MarketValueEquity = marketValueEquity,
        BookValueEquity = 0m,
        Sales = sales,
        NetIncome = 0m,
        PreviousNetIncome = 0m,
        FundsFromOperations = 0m,
        GNPPriceIndex = 1m
    };
}
