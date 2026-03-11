using CRAS.Domain.Entities;
using CRAS.Domain.Interfaces;
using CRAS.Domain.Models;

namespace CRAS.Domain.Engine;

/// <summary>
///     Defines the core engine responsible for orchestrating the risk assessment process.
/// </summary>
public interface IRiskEngine
{
    /// <summary>
    ///     Evaluates the financial risk of a contractor based on a specific financial statement.
    /// </summary>
    /// <param name="contractor">The contractor for whom risk assessment is being performed.</param>
    /// <param name="statement">The financial statement data to be analyzed.</param>
    /// <returns>An aggregated result containing scores from all active risk models and a final risk level.</returns>
    AggregatedRiskResult Assess(Contractor contractor, FinancialStatement statement);
}

/// <summary>
///     Provides the standard implementation of the risk assessment orchestration logic.
/// </summary>
/// <remarks>
///     This engine uses a collection of injected risk models to perform individual evaluations
///     and applies a specified aggregation strategy to produce a unified conclusion.
/// </remarks>
/// <param name="riskModels">The collection of financial risk models to be executed.</param>
/// <param name="behavioralRiskModels">The collection of behavioral risk models to be executed.</param>
/// <param name="aggregationStrategy">The strategy used to consolidate multiple risk results into one.</param>
public class RiskEngine(
    IEnumerable<IRiskModel> riskModels,
    IEnumerable<IBehavioralRiskModel> behavioralRiskModels,
    IRiskAggregationStrategy aggregationStrategy) : IRiskEngine
{
    /// <summary>
    ///     Runs all registered risk models against the provided statement and aggregates the outcomes.
    /// </summary>
    /// <param name="contractor">The contractor for whom risk assessment is being performed.</param>
    /// <param name="statement">The financial statement to assess.</param>
    /// <returns>A consolidated <see cref="AggregatedRiskResult" />.</returns>
    public AggregatedRiskResult Assess(Contractor contractor, FinancialStatement statement)
    {
        var individualResults = new List<RiskResult>();

        individualResults.AddRange(riskModels.Select(m => m.CalculateRisk(statement)));
        individualResults.AddRange(behavioralRiskModels.Select(m => m.CalculateRisk(contractor)));

        return aggregationStrategy.Aggregate(individualResults);
    }
}