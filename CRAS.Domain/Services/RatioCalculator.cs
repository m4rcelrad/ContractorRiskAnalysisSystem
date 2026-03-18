using CRAS.Domain.Entities;
using CRAS.Domain.Interfaces;
using CRAS.Domain.Models;

namespace CRAS.Domain.Services;

/// <summary>
///     Represents a service for calculating key financial ratios based on a provided financial statement.
/// </summary>
public class RatioCalculator : IRatioCalculator
{
    public KeyFinancialRatios CalculateFor(FinancialStatement statement) => new(
        SafeDivide(statement.CurrentAssets, statement.CurrentLiabilities),
        SafeDivide(statement.TotalLiabilities, statement.TotalAssets),
        SafeDivide(statement.NetIncome, statement.BookValueEquity),
        SafeDivide(statement.WorkingCapital, statement.TotalAssets),
        SafeDivide(statement.Sales, statement.TotalAssets),
        SafeDivide(statement.EBIT, statement.Sales)
    );

    private static decimal SafeDivide(decimal numerator, decimal denominator) => denominator == 0 ? 0m : numerator / denominator;
}
