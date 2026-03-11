using CRAS.Domain.Entities;
using CRAS.Domain.Interfaces;
using CRAS.Domain.Models;

namespace CRAS.Domain.Engine;

public class RiskEngine(IEnumerable<IRiskModel> riskModels, IRiskAggregationStrategy aggregationStrategy)
{
    public AggregatedRiskResult Assess(FinancialStatement statement)
    {
        var individualResults = riskModels.Select(m => m.CalculateRisk(statement)).ToList();

        return aggregationStrategy.Aggregate(individualResults);
    }
}