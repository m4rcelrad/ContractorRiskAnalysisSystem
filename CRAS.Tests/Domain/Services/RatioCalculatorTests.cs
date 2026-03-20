using CRAS.Domain.Entities;
using CRAS.Domain.Services;

namespace CRAS.Tests.Domain.Services;

/// <summary>
/// Unit tests for the <see cref="RatioCalculator"/> service.
/// </summary>
public class RatioCalculatorTests
{
    private readonly RatioCalculator _calculator = new();

    /// <summary>
    /// Verifies that <see cref="RatioCalculator.CalculateFor"/> returns correct
    /// ratios based on valid financial statement data.
    /// </summary>
    [Fact]
    public void CalculateFor_WithValidData_ReturnsExpectedRatios()
    {
        var statement = new FinancialStatement
        {
            ContractorId = Guid.NewGuid(),
            Year = 2025,
            TotalAssets = 1000m,
            TotalLiabilities = 500m,
            CurrentAssets = 400m,
            CurrentLiabilities = 200m,
            WorkingCapital = 200m,
            RetainedEarnings = 100m,
            EBIT = 150m,
            MarketValueEquity = 1200m,
            BookValueEquity = 500m,
            Sales = 2000m,
            NetIncome = 100m,
            PreviousNetIncome = 80m,
            FundsFromOperations = 120m,
            GNPPriceIndex = 1.0m
        };

        var result = _calculator.CalculateFor(statement);

        Assert.Equal(2.0m, result.CurrentRatio);
        Assert.Equal(0.5m, result.DebtRatio);
        Assert.Equal(0.2m, result.ReturnOnEquity);
        Assert.Equal(0.075m, result.EbitMargin);
    }

    /// <summary>
    /// Verifies that the calculator handles zero denominators gracefully by
    /// returning zero instead of throwing exceptions.
    /// </summary>
    [Fact]
    public void CalculateFor_WithZeroDenominators_ReturnsZeroRatios()
    {
        var statement = new FinancialStatement
        {
            ContractorId = Guid.NewGuid(),
            Year = 2025,
            TotalAssets = 0,
            TotalLiabilities = 100,
            CurrentAssets = 50,
            CurrentLiabilities = 0,
            WorkingCapital = 50,
            RetainedEarnings = 10,
            EBIT = 5,
            MarketValueEquity = 50,
            BookValueEquity = 0,
            Sales = 0,
            NetIncome = 0,
            PreviousNetIncome = 0,
            FundsFromOperations = 0,
            GNPPriceIndex = 1
        };

        var result = _calculator.CalculateFor(statement);

        Assert.Equal(0m, result.CurrentRatio);
        Assert.Equal(0m, result.DebtRatio);
        Assert.Equal(0m, result.ReturnOnEquity);
    }
}
