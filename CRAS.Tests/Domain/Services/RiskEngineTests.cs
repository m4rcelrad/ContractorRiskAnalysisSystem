using CRAS.Domain.Entities;
using CRAS.Domain.Interfaces;
using CRAS.Domain.Services;
using CRAS.Domain.ValueObjects;
using Moq;

namespace CRAS.Tests.Domain.Services;

/// <summary>
/// Unit tests for the <see cref="RiskEngine"/> class.
/// </summary>
public class RiskEngineTests
{
    /// <summary>
    /// Verifies that <see cref="RiskEngine.AssessAsync"/> correctly invokes all registered
    /// models and aggregates their results into a final assessment.
    /// </summary>
    [Fact]
    public async Task AssessAsync_OrchestratesAllModelsAndAggregates_ReturnsResult()
    {
        var contractor = new Contractor { TaxId = "7740001454" };
        var statement = new FinancialStatement
        {
            ContractorId = contractor.Id, Year = 2025, TotalAssets = 1, TotalLiabilities = 1,
            CurrentAssets = 1, CurrentLiabilities = 1, WorkingCapital = 1, RetainedEarnings = 1,
            EBIT = 1, MarketValueEquity = 1, BookValueEquity = 1, Sales = 1, NetIncome = 1,
            PreviousNetIncome = 1, FundsFromOperations = 1, GNPPriceIndex = 1
        };

        var financialModelMock = new Mock<IRiskModel>();
        financialModelMock.Setup(m => m.CalculateRisk(It.IsAny<FinancialStatement>()))
            .Returns(new RiskResult { Model = "Financial", Score = 1 });

        var behavioralModelMock = new Mock<IBehavioralRiskModel>();
        behavioralModelMock.Setup(m => m.CalculateRiskAsync(It.IsAny<Contractor>()))
            .ReturnsAsync(new RiskResult { Model = "Behavioral", Score = 1 });

        var aggregationMock = new Mock<IRiskAggregationStrategy>();
        aggregationMock.Setup(a => a.Aggregate(It.IsAny<IReadOnlyCollection<RiskResult>>()))
            .Returns(new AggregatedRiskResult());

        var engine = new RiskEngine(
            [ financialModelMock.Object ],
            [ behavioralModelMock.Object ],
            aggregationMock.Object);

        await engine.AssessAsync(contractor, statement);

        financialModelMock.Verify(m => m.CalculateRisk(statement), Times.Once);
        behavioralModelMock.Verify(m => m.CalculateRiskAsync(contractor), Times.Once);
        aggregationMock.Verify(a => a.Aggregate(It.Is<IReadOnlyCollection<RiskResult>>(c => c.Count == 2)), Times.Once);
    }
}
