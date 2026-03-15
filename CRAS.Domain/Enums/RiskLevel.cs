namespace CRAS.Domain.Enums;

/// <summary>
///     Represents the categorized financial risk level of a contractor based on their financial statement.
/// </summary>
public enum RiskLevel
{
    /// <summary>
    ///     Indicates a low probability of financial distress or bankruptcy. The contractor is considered financially healthy.
    /// </summary>
    Low,

    /// <summary>
    ///     Indicates an uncertain financial situation. The contractor requires further monitoring or manual analysis.
    /// </summary>
    Moderate,

    /// <summary>
    ///     Indicates a high probability of bankruptcy or severe financial problems. Represents a high risk for cooperation.
    /// </summary>
    Critical
}
