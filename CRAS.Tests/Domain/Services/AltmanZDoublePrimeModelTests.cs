using CRAS.Domain.Entities;
using CRAS.Domain.Enums;
using CRAS.Domain.RiskModels;
using CRAS.Domain.Services;

namespace CRAS.Tests.Domain.Services;

/// <summary>
///     Contains unit tests for the <see cref="AltmanZDoublePrimeModel" /> class.
/// </summary>
/// <remarks>
///     This suite ensures the variant model correctly assesses private, non-manufacturing entities.
///     It verifies the modified constants, the exclusion of the sales ratio, and the shifted risk thresholds.
/// </remarks>
public class AltmanZDoublePrimeModelTests
{
    private readonly AltmanZDoublePrimeModel _model = new();

    /// <summary>
    ///     Verifies that an <see cref="ArgumentException" /> is thrown when the total assets value is zero.
    /// </summary>
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
    ///     Verifies that the model correctly calculates a distress risk level and assigns the appropriate
    ///     score when provided with poor financial indicators.
    /// </summary>
    [Fact]
    public void CalculateRisk_ReturnsDistressRiskLevel_WhenFinancialsAreWeak()
    {
        var statement = CreateTestStatement(
            100000m,
            90000m,
            -10000m,
            -5000m,
            -2000m,
            10000m);

        var result = _model.CalculateRisk(statement);

        Assert.Equal("Altman Z''-Score", result.Model);
        Assert.Equal(RiskLevel.Critical, result.RiskLevel);
        Assert.True(result.Score < 1.10m);
    }

    /// <summary>
    ///     Verifies that the model accurately assigns a safe status when the company exhibits strong equity and earnings.
    /// </summary>
    [Fact]
    public void CalculateRisk_ReturnsSafeRiskLevel_WhenFinancialsAreStrong()
    {
        var statement = CreateTestStatement(
            100000m,
            20000m,
            60000m,
            40000m,
            25000m,
            80000m);

        var result = _model.CalculateRisk(statement);

        Assert.Equal(RiskLevel.Low, result.RiskLevel);
        Assert.True(result.Score > 2.60m);
    }

    /// <summary>
    ///     Creates a test financial statement with the specified values.
    /// </summary>
    /// <param name="totalAssets"></param>
    /// <param name="totalLiabilities"></param>
    /// <param name="workingCapital"></param>
    /// <param name="retainedEarnings"></param>
    /// <param name="ebit"></param>
    /// <param name="bookValueEquity"></param>
    /// <returns>
    ///     <see cref="FinancialStatement" />
    /// </returns>
    private static FinancialStatement CreateTestStatement(
        decimal totalAssets,
        decimal totalLiabilities,
        decimal workingCapital = 0m,
        decimal retainedEarnings = 0m,
        decimal ebit = 0m,
        decimal bookValueEquity = 0m) => new()
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
        MarketValueEquity = 0m,
        BookValueEquity = bookValueEquity,
        Sales = 0m,
        NetIncome = 0m,
        PreviousNetIncome = 0m,
        FundsFromOperations = 0m,
        GNPPriceIndex = 1m
    };
}
