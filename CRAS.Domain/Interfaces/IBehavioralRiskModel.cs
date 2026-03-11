using CRAS.Domain.Entities;
using CRAS.Domain.Models;

namespace CRAS.Domain.Interfaces;

/// <summary>
///     Defines the contract for risk models that evaluate a contractor's overall reliability and payment history.
/// </summary>
public interface IBehavioralRiskModel
{
    /// <summary>
    ///     Calculates the risk level based on the full profile and history of a specific contractor.
    /// </summary>
    /// <param name="contractor">The <see cref="Contractor" /> entity containing profile data and invoice history.</param>
    /// <returns>A <see cref="RiskResult" /> representing the calculated behavioral risk.</returns>
    RiskResult CalculateRisk(Contractor contractor);
}