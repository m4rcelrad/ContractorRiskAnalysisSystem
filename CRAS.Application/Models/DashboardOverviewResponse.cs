namespace CRAS.Application.Models;

/// <summary>
///     Represents an overview of a contractor's dashboard data, including contractor details
///     and their associated risk assessment.
/// </summary>
public class DashboardOverviewResponse
{
    public Guid ContractorId { get; init; }
    public string TaxId { get; init; } = string.Empty;
    public RiskAssessmentResponse Assessment { get; init; } = new();
}
