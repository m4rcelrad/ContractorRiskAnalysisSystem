using CRAS.Domain.Enums;
using CRAS.Domain.Models;

namespace CRAS.Api.Models;

/// <summary>
///     Represents a response containing the result of a contractor's risk assessment.
///     This includes the overall risk evaluation, detailed breakdown, and other metadata.
/// </summary>
public class RiskAssessmentResponse
{
    public Guid ContractorId { get; set; }
    public string ContractorName { get; set; } = string.Empty;
    public RiskLevel OverallRisk { get; set; }
    public decimal OverallScore { get; set; }
    public List<RiskResult> Breakdown { get; set; } = [];
    public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
}