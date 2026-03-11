using CRAS.Domain.Entities;
using CRAS.Domain.Enums;
using CRAS.Domain.Interfaces;
using CRAS.Domain.Models;

namespace CRAS.Domain.Services;

/// <summary>
///     Implements the original Altman Z-Score financial risk model.
/// </summary>
/// <remarks>
///     This model is designed primarily to predict the probability of bankruptcy
///     for public manufacturing companies based on five specific financial ratios.
/// </remarks>
public class AltmanZScoreModel : IRiskModel
{
    /// <summary>
    ///     Gets the formal name of the risk model.
    /// </summary>
    public string ModelName => "Altman Z-Score";

    /// <summary>
    ///     Calculates the Altman Z-Score and determines the corresponding risk level.
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
        var x4 = statement.MarketValueEquity / statement.TotalLiabilities;
        var x5 = statement.Sales / statement.TotalAssets;

        var score = 1.2m * x1 + 1.4m * x2 + 3.3m * x3 + 0.6m * x4 + 1.0m * x5;

        var riskLevel = score switch
        {
            > 2.99m => RiskLevel.Safe,
            >= 1.81m and <= 2.99m => RiskLevel.Grey,
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