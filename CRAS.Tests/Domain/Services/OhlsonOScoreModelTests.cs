using CRAS.Domain.Entities;
using CRAS.Domain.Services;

namespace CRAS.Tests.Domain.Services;

/// <summary>
///     Contains unit tests for the <see cref="OhlsonOScoreModel" /> class.
/// </summary>
/// <remarks>
///     This suite verifies the logistic regression calculations used to predict the probability
///     of bankruptcy. It thoroughly tests multiple denominator validations required by the nine-factor model.
/// </remarks>
public class OhlsonOScoreModelTests
{
    private readonly OhlsonOScoreModel _model = new();

    /// <summary>
    ///     Verifies that an <see cref="ArgumentException" /> is thrown when the GNP price index is zero.
    /// </summary>
    /// <remarks>
    ///     The Ohlson model uses the GNP Price Index to scale Total Assets. A zero value would
    ///     cause a division by zero error prior to the logarithmic transformation.
    /// </remarks>
    [Fact]
    public void CalculateRisk_ThrowsArgumentException_WhenGnpPriceIndexIsZero()
    {
        var statement = CreateTestStatement(
            1000m,
            500m,
            500m,
            gnpPriceIndex: 0m);

        Assert.Throws<ArgumentException>(() => _model.CalculateRisk(statement));
    }

    /// <summary>
    ///     Verifies that an <see cref="ArgumentException" /> is thrown if Current Assets are zero,
    ///     protecting the CL/CA ratio calculation.
    /// </summary>
    [Fact]
    public void CalculateRisk_ThrowsArgumentException_WhenCurrentAssetsAreZero()
    {
        var statement = CreateTestStatement(
            1000m,
            0m,
            500m,
            gnpPriceIndex: 110.5m);

        Assert.Throws<ArgumentException>(() => _model.CalculateRisk(statement));
    }

    /// <summary>
    ///     Verifies that the logistic regression formula yields a valid probability strictly between 0 and 1.
    /// </summary>
    [Fact]
    public void CalculateRisk_ReturnsValidProbability_WhenFinancialsAreProvided()
    {
        var statement = CreateTestStatement(
            500000m,
            300000m,
            250000m,
            150000m,
            150000m,
            50000m,
            40000m,
            60000m,
            110.5m);

        var result = _model.CalculateRisk(statement);

        Assert.Equal("Ohlson O-Score", result.Model);
        Assert.True(result.Score is >= 0m and <= 1.0m);
    }

    /// <summary>
    ///     Creates a test financial statement with the specified values.
    /// </summary>
    /// <param name="totalAssets"></param>
    /// <param name="currentAssets"></param>
    /// <param name="totalLiabilities"></param>
    /// <param name="workingCapital"></param>
    /// <param name="currentLiabilities"></param>
    /// <param name="netIncome"></param>
    /// <param name="previousNetIncome"></param>
    /// <param name="fundsFromOperations"></param>
    /// <param name="gnpPriceIndex"></param>
    /// <returns>
    ///     <see cref="FinancialStatement" />
    /// </returns>
    private static FinancialStatement CreateTestStatement(
        decimal totalAssets = 0m,
        decimal currentAssets = 0m,
        decimal totalLiabilities = 0m,
        decimal currentLiabilities = 0m,
        decimal workingCapital = 0m,
        decimal netIncome = 0m,
        decimal previousNetIncome = 0m,
        decimal fundsFromOperations = 0m,
        decimal gnpPriceIndex = 1m) => new()
    {
        ContractorId = Guid.NewGuid(),
        Year = DateTime.UtcNow.Year,
        TotalAssets = totalAssets,
        TotalLiabilities = totalLiabilities,
        CurrentAssets = currentAssets,
        CurrentLiabilities = currentLiabilities,
        WorkingCapital = workingCapital,
        RetainedEarnings = 0m,
        EBIT = 0m,
        MarketValueEquity = 0m,
        BookValueEquity = 0m,
        Sales = 0m,
        NetIncome = netIncome,
        PreviousNetIncome = previousNetIncome,
        FundsFromOperations = fundsFromOperations,
        GNPPriceIndex = gnpPriceIndex
    };
}
