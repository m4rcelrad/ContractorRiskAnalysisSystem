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
    ///     Asynchronously evaluates the risk of a contractor by combining financial statement analysis and behavioral models.
    /// </summary>
    /// <param name="contractor">The contractor entity containing behavioral data such as invoice history.</param>
    /// <param name="statement">The financial statement data to be analyzed by financial models.</param>
    /// <returns>A task representing the asynchronous operation, containing the aggregated risk result.</returns>
    Task<AggregatedRiskResult> AssessAsync(Contractor contractor, FinancialStatement statement);
}

/// <summary>
///     Provides the standard implementation of the risk assessment orchestration logic.
/// </summary>
/// <remarks>
///     This engine uses a collection of injected financial and behavioral risk models to perform evaluations
///     and applies a specified aggregation strategy to produce a unified conclusion.
/// </remarks>
/// <param name="riskModels">The collection of financial risk models to be executed.</param>
/// <param name="behavioralRiskModels">The collection of behavioral risk models to be executed asynchronously.</param>
/// <param name="aggregationStrategy">The strategy used to consolidate multiple risk results into one.</param>
public class RiskEngine(
    IEnumerable<IRiskModel> riskModels,
    IEnumerable<IBehavioralRiskModel> behavioralRiskModels,
    IRiskAggregationStrategy aggregationStrategy) : IRiskEngine
{
    /// <summary>
    ///     Runs all registered financial and behavioral models and aggregates the outcomes into a single result.
    /// </summary>
    /// <param name="contractor">The contractor to assess.</param>
    /// <param name="statement">The financial statement to assess.</param>
    /// <returns>A consolidated <see cref="AggregatedRiskResult" />.</returns>
    public async Task<AggregatedRiskResult> AssessAsync(Contractor contractor, FinancialStatement statement)
    {
        var individualResults = new List<RiskResult>();

        individualResults.AddRange(riskModels.Select(m => m.CalculateRisk(statement)));

        foreach (var model in behavioralRiskModels)
        {
            var result = await model.CalculateRiskAsync(contractor);
            individualResults.Add(result);
        }

        return aggregationStrategy.Aggregate(individualResults);
    }
}
