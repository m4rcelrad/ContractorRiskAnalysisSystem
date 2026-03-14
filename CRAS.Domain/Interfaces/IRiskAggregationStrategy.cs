using CRAS.Domain.Models;

namespace CRAS.Domain.Interfaces;

/// <summary>
///     Defines the contract for strategies that aggregate multiple financial risk assessments into a single conclusive
///     result.
/// </summary>
/// <remarks>
///     This interface implements the Strategy design pattern, allowing the domain to dynamically
///     switch between different evaluation policies (e.g., Majority Vote, Worst Case, Weighted Score)
///     without modifying the core risk engine logic.
/// </remarks>
public interface IRiskAggregationStrategy
{
    /// <summary>
    ///     Aggregates a collection of individual risk model results into a final, unified risk assessment.
    /// </summary>
    /// <param name="results">A read-only collection of individual <see cref="RiskResult" /> outcomes to be evaluated.</param>
    /// <returns>
    ///     An <see cref="AggregatedRiskResult" /> containing the overall determined risk level
    ///     and the underlying individual results.
    /// </returns>
    AggregatedRiskResult Aggregate(IReadOnlyCollection<RiskResult> results);
}
