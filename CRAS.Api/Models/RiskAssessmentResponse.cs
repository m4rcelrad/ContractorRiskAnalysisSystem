using CRAS.Domain.Enums;
using CRAS.Domain.Models;

namespace CRAS.Api.Models;

/// <summary>
///     Represents a response containing the result of a contractor's risk assessment.
///     This includes the overall risk evaluation, detailed breakdown, and other metadata.
/// </summary>
public class RiskAssessmentResponse
{
    public Guid ContractorId { get; init; }
    public string ContractorName { get; init; } = string.Empty;
    public RiskLevel OverallRisk { get; init; }
    public List<RiskResult> Breakdown { get; init; } = [ ];
}
