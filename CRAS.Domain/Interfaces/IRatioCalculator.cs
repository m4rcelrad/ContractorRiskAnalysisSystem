using CRAS.Domain.Entities;
using CRAS.Domain.ValueObjects;

namespace CRAS.Domain.Interfaces;

/// <summary>
///     Represents an interface for calculating financial ratios.
/// </summary>
public interface IRatioCalculator
{
    KeyFinancialRatios CalculateFor(FinancialStatement statement);
}
