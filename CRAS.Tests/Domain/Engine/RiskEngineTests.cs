using CRAS.Domain.Entities;
using CRAS.Domain.Interfaces;
using CRAS.Domain.Services;
using CRAS.Domain.ValueObjects;
using Moq;

namespace CRAS.Tests.Domain.Engine;

/// <summary>
///     Verifies the orchestration logic of the <see cref="RiskEngine" />.
/// </summary>
public class RiskEngineTests
{
    /// <summary>
    ///     Ensures that the engine correctly invokes all provided models and passes their results
    ///     to the designated aggregation strategy.
    /// </summary>
    [Fact]
    public async Task AssessAsync_CallsAllModelsAndAggregatesResults()
    {
        var contractorId = Guid.NewGuid();

        var contractor = new Contractor
        {
            TaxId = "0000000000"
        };

        var statement = new FinancialStatement
        {
            ContractorId = contractorId,
            Year = DateTime.UtcNow.Year,
            TotalAssets = 0m,
            TotalLiabilities = 0m,
            CurrentAssets = 0m,
            CurrentLiabilities = 0m,
            WorkingCapital = 0m,
            RetainedEarnings = 0m,
            EBIT = 0m,
            MarketValueEquity = 0m,
            BookValueEquity = 0m,
            Sales = 0m,
            NetIncome = 0m,
            PreviousNetIncome = 0m,
            FundsFromOperations = 0m,
            GNPPriceIndex = 1m
        };

        var financialModelResult = new RiskResult { Model = "MockFinancialModel" };
        var behavioralModelResult = new RiskResult { Model = "MockBehavioralModel" };

        var financialModelMock = new Mock<IRiskModel>();
        financialModelMock.Setup(m => m.CalculateRisk(statement)).Returns(financialModelResult);

        var behavioralModelMock = new Mock<IBehavioralRiskModel>();
        behavioralModelMock.Setup(m => m.CalculateRiskAsync(contractor)).ReturnsAsync(behavioralModelResult);

        var strategyMock = new Mock<IRiskAggregationStrategy>();

        var engine = new RiskEngine([ financialModelMock.Object ], [ behavioralModelMock.Object ], strategyMock.Object);

        await engine.AssessAsync(contractor, statement);

        financialModelMock.Verify(m => m.CalculateRisk(statement), Times.Once);
        behavioralModelMock.Verify(m => m.CalculateRiskAsync(contractor), Times.Once);

        strategyMock.Verify(s => s.Aggregate(It.Is<IReadOnlyCollection<RiskResult>>(c =>
            c.Contains(financialModelResult) && c.Contains(behavioralModelResult))), Times.Once);
    }
}
