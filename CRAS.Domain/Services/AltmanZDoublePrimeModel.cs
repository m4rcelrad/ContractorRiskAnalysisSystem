using CRAS.Domain.Entities;
using CRAS.Domain.Enums;
using CRAS.Domain.Interfaces;
using CRAS.Domain.Models;

namespace CRAS.Domain.Services;

/// <summary>
///     Implements the Altman Z''-Score (Double Prime) financial risk model.
/// </summary>
/// <remarks>
///     This variant is designed specifically for private, non-manufacturing companies,
///     as well as for companies operating in emerging markets.
/// </remarks>
public class AltmanZDoublePrimeModel : IRiskModel
{
    /// <summary>
    ///     Gets the formal name of the risk model.
    /// </summary>
    public string ModelName => "Altman Z''-Score";

    /// <summary>
    ///     Calculates the Altman Z''-Score and determines the corresponding risk level.
    /// </summary>
    /// <param name="statement">The financial statement containing the necessary data points.</param>
    /// <returns>A <see cref="RiskResult" /> containing the calculated score and risk level.</returns>
    /// <exception cref="ArgumentException">Thrown when essential denominator data is zero.</exception>
    public RiskResult CalculateRisk(FinancialStatement statement)
    {
        if (statement.TotalAssets == 0m || statement.TotalLiabilities == 0m)
            throw new ArgumentException("Invalid financial data: TotalAssets and TotalLiabilities cannot be zero.");

        var x1 = statement.WorkingCapital / statement.TotalAssets;
        var x2 = statement.RetainedEarnings / statement.TotalAssets;
        var x3 = statement.EBIT / statement.TotalAssets;
        var x4 = statement.BookValueEquity / statement.TotalLiabilities;

        var score = 6.56m * x1 + 3.26m * x2 + 6.72m * x3 + 1.05m * x4;

        var riskLevel = score switch
        {
            > 2.60m => RiskLevel.Safe,
            >= 1.1m and <= 2.60m => RiskLevel.Grey,
            _ => RiskLevel.Distress
        };

        return new RiskResult
        {
            Model = ModelName,
            Score = score,
            RiskLevel = riskLevel,
            Interpretation = riskLevel.ToString()
        };
    }
}