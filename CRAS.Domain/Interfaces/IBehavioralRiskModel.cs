using CRAS.Domain.Entities;
using CRAS.Domain.Models;

namespace CRAS.Domain.Interfaces;

/// <summary>
///     Defines the contract for risk models that evaluate a contractor's overall reliability and payment history.
/// </summary>
public interface IBehavioralRiskModel
{
    /// <summary>
    ///     Asynchronously evaluates the behavioral risk associated with a specific contractor.
    /// </summary>
    /// <param name="contractor">The contractor entity containing the data to be analyzed, such as invoice history.</param>
    /// <returns>A task representing the asynchronous operation, containing the calculated <see cref="RiskResult" />.</returns>
    Task<RiskResult> CalculateRiskAsync(Contractor contractor);
}
