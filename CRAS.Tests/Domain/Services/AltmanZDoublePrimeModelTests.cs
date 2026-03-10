using CRAS.Domain.Entities;
using CRAS.Domain.Enums;
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
    ///     Verifies that the model correctly calculates a distress risk level and assigns the appropriate
    ///     score when provided with poor financial indicators.
    /// </summary>
    [Fact]
    public void CalculateRisk_ReturnsDistressRiskLevel_WhenFinancialsAreWeak()
    {
        var statement = new FinancialStatement
        {
            TotalAssets = 100000m,
            TotalLiabilities = 90000m,
            WorkingCapital = -10000m,
            RetainedEarnings = -5000m,
            EBIT = -2000m,
            BookValueEquity = 10000m
        };

        var result = _model.CalculateRisk(statement);

        Assert.Equal("Altman Z''-Score", result.Model);
        Assert.Equal(RiskLevel.Distress, result.RiskLevel);
        Assert.True(result.Score < 1.10m);
    }

    /// <summary>
    ///     Verifies that the model accurately assigns a safe status when the company exhibits strong equity and earnings.
    /// </summary>
    [Fact]
    public void CalculateRisk_ReturnsSafeRiskLevel_WhenFinancialsAreStrong()
    {
        var statement = new FinancialStatement
        {
            TotalAssets = 100000m,
            TotalLiabilities = 20000m,
            WorkingCapital = 60000m,
            RetainedEarnings = 40000m,
            EBIT = 25000m,
            BookValueEquity = 80000m
        };

        var result = _model.CalculateRisk(statement);

        Assert.Equal(RiskLevel.Safe, result.RiskLevel);
        Assert.True(result.Score > 2.60m);
    }
}
