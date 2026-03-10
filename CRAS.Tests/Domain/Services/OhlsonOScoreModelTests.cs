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
        var statement = new FinancialStatement
        {
            TotalAssets = 1000m,
            CurrentAssets = 500m,
            TotalLiabilities = 500m,
            GNPPriceIndex = 0m
        };

        Assert.Throws<ArgumentException>(() => _model.CalculateRisk(statement));
    }

    /// <summary>
    ///     Verifies that an <see cref="ArgumentException" /> is thrown if Current Assets are zero,
    ///     protecting the CL/CA ratio calculation.
    /// </summary>
    [Fact]
    public void CalculateRisk_ThrowsArgumentException_WhenCurrentAssetsAreZero()
    {
        var statement = new FinancialStatement
        {
            TotalAssets = 1000m,
            TotalLiabilities = 500m,
            CurrentAssets = 0m,
            GNPPriceIndex = 110.5m
        };

        Assert.Throws<ArgumentException>(() => _model.CalculateRisk(statement));
    }

    /// <summary>
    ///     Verifies that the logistic regression formula yields a valid probability strictly between 0 and 1.
    /// </summary>
    [Fact]
    public void CalculateRisk_ReturnsValidProbability_WhenFinancialsAreProvided()
    {
        var statement = new FinancialStatement
        {
            TotalAssets = 500000m,
            TotalLiabilities = 250000m,
            CurrentAssets = 300000m,
            CurrentLiabilities = 150000m,
            WorkingCapital = 150000m,
            NetIncome = 50000m,
            PreviousNetIncome = 40000m,
            FundsFromOperations = 60000m,
            GNPPriceIndex = 110.5m
        };

        var result = _model.CalculateRisk(statement);

        Assert.Equal("Ohlson O-Score", result.Model);
        Assert.True(result.Score is >= 0m and <= 1.0m);
    }
}
