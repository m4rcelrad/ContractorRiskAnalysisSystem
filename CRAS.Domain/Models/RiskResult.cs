using CRAS.Domain.Enums;

namespace CRAS.Domain.Models;

/// <summary>
///     Encapsulates the outcome of a risk model evaluation.
/// </summary>
public class RiskResult
{
    /// <summary>
    ///     Gets the name of the model that produced this result.
    /// </summary>
    public string Model { get; init; } = string.Empty;

    /// <summary>
    ///     Gets the raw numerical score calculated by the model.
    /// </summary>
    public decimal Score { get; init; }

    /// <summary>
    ///     Gets the categorized risk level derived from the calculated score.
    /// </summary>
    public RiskLevel RiskLevel { get; init; }

    /// <summary>
    ///     Gets a human-readable interpretation of the result (e.g., "Probability of bankruptcy: 15%").
    /// </summary>
    public string Interpretation { get; init; } = string.Empty;
}
