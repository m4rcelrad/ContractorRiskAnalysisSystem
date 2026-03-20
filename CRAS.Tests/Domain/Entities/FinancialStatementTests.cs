using CRAS.Domain.Entities;

namespace CRAS.Tests.Domain.Entities;

/// <summary>
///     Unit tests for the <see cref="FinancialStatement" /> class.
/// </summary>
/// <remarks>
///     This class verifies the initialization and property assignment of financial statement entities,
///     ensuring that default values and required data points are correctly handled [cite: 138-140].
/// </remarks>
public class FinancialStatementTests
{
    /// <summary>
    ///     Verifies that a new <see cref="FinancialStatement" /> instance generates a unique identifier by default.
    /// </summary>
    [Fact]
    public void Constructor_ShouldInitializeId()
    {
        var statement = new FinancialStatement
        {
            ContractorId = Guid.NewGuid(),
            Year = 2025,
            TotalAssets = 100000m,
            TotalLiabilities = 50000m,
            CurrentAssets = 40000m,
            CurrentLiabilities = 20000m,
            WorkingCapital = 20000m,
            RetainedEarnings = 10000m,
            EBIT = 15000m,
            MarketValueEquity = 120000m,
            BookValueEquity = 50000m,
            Sales = 200000m,
            NetIncome = 25000m,
            PreviousNetIncome = 20000m,
            FundsFromOperations = 30000m,
            GNPPriceIndex = 1.0m
        };

        Assert.NotEqual(Guid.Empty, statement.Id);
    }

    /// <summary>
    ///     Verifies that properties are correctly assigned and stored during initialization.
    /// </summary>
    [Fact]
    public void Properties_ShouldBeCorrectlyAssigned()
    {
        var contractorId = Guid.NewGuid();
        const int fiscalYear = 2024;
        const decimal assets = 1500000m;

        var statement = new FinancialStatement
        {
            ContractorId = contractorId,
            Year = fiscalYear,
            TotalAssets = assets,
            TotalLiabilities = 700000m,
            CurrentAssets = 400000m,
            CurrentLiabilities = 300000m,
            WorkingCapital = 100000m,
            RetainedEarnings = 200000m,
            EBIT = 120000m,
            MarketValueEquity = 2000000m,
            BookValueEquity = 800000m,
            Sales = 2500000m,
            NetIncome = 180000m,
            PreviousNetIncome = 150000m,
            FundsFromOperations = 210000m,
            GNPPriceIndex = 1.02m
        };

        Assert.Equal(contractorId, statement.ContractorId);
        Assert.Equal(fiscalYear, statement.Year);
        Assert.Equal(assets, statement.TotalAssets);
    }
}
