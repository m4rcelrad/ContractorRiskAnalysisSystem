using CRAS.Domain.Enums;

namespace CRAS.Domain.ValueObjects;

/// <summary>
///     Represents the final, consolidated outcome of applying multiple financial risk models.
/// </summary>
/// <remarks>
///     This model encapsulates both the overarching risk classification determined by a specific
///     aggregation strategy and the detailed results from each  model used in the evaluation.
/// </remarks>
public class AggregatedRiskResult
{
    /// <summary>
    ///     Gets the final, aggregated risk level determined by the applied aggregation strategy.
    /// </summary>
    public RiskLevel OverallRiskLevel { get; init; }

    /// <summary>
    ///     Gets the read-only collection of individual risk assessments produced by the underlying risk models.
    /// </summary>
    public IReadOnlyCollection<RiskResult> IndividualResults { get; init; } = new List<RiskResult>();
}
