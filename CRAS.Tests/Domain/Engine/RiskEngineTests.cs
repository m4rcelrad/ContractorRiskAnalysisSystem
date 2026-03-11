using CRAS.Domain.Engine;
using CRAS.Domain.Entities;
using CRAS.Domain.Interfaces;
using CRAS.Domain.Models;
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
    public void Assess_CallsAllModelsAndAggregatesResults()
    {
        var statement = new FinancialStatement
        {
            ContractorId = Guid.NewGuid(),
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

        var modelResult = new RiskResult { Model = "MockModel" };

        var modelMock = new Mock<IRiskModel>();
        modelMock.Setup(m => m.CalculateRisk(statement)).Returns(modelResult);

        var strategyMock = new Mock<IRiskAggregationStrategy>();

        var engine = new RiskEngine([ modelMock.Object ], strategyMock.Object);

        engine.Assess(statement);

        modelMock.Verify(m => m.CalculateRisk(statement), Times.Once);
        strategyMock.Verify(s => s.Aggregate(It.Is<IReadOnlyCollection<RiskResult>>(c => c.Contains(modelResult))), Times.Once);
    }
}
