using CRAS.Domain.Entities;

namespace CRAS.Domain.Interfaces;

/// <summary>
///     Defines the contract for financial risk assessment models (e.g., Altman Z-Score, Ohlson O-Score).
/// </summary>
public interface IRiskModel
{
    /// <summary>
    ///     Gets the formal name of the implemented risk model.
    /// </summary>
    string ModelName { get; }

    /// <summary>
    ///     Calculates the numerical risk score based on the provided financial statement.
    /// </summary>
    /// <param name="financialStatement">The financial data of the contractor used for the calculation.</param>
    /// <returns>A decimal value representing the calculated risk score or probability.</returns>
    decimal CalculateRisk(FinancialStatement financialStatement);
}
