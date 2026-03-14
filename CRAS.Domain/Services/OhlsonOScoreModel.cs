using CRAS.Domain.Entities;
using CRAS.Domain.Enums;
using CRAS.Domain.Interfaces;
using CRAS.Domain.Models;

namespace CRAS.Domain.Services;

/// <summary>
///     Implements the Ohlson O-Score model.
/// </summary>
/// <remarks>
///     This is a probabilistic model using logistic regression to predict
///     the likelihood of bankruptcy based on nine financial ratios.
/// </remarks>
public class OhlsonOScoreModel : IRiskModel
{
    /// <summary>
    ///     Gets the formal name of the risk model.
    /// </summary>
    public string ModelName => "Ohlson O-Score";

    /// <summary>
    ///     Calculates the Ohlson O-Score probability and determines the corresponding risk level.
    /// </summary>
    /// <param name="statement">The financial statement containing the necessary data points.</param>
    /// <returns>A <see cref="RiskResult" /> containing the calculated probability of bankruptcy.</returns>
    /// <exception cref="ArgumentException">Thrown when essential denominator data is zero.</exception>
    public RiskResult CalculateRisk(FinancialStatement statement)
    {
        if (statement.TotalAssets == 0m || statement.CurrentAssets == 0m || statement.TotalLiabilities == 0m || statement.GNPPriceIndex == 0m)
            throw new ArgumentException("Invalid financial data: Required denominators cannot be zero.");

        var totalAssets = (double)statement.TotalAssets;
        var gnpPriceIndex = (double)statement.GNPPriceIndex;
        var totalLiabilities = (double)statement.TotalLiabilities;
        var workingCapital = (double)statement.WorkingCapital;
        var currentAssets = (double)statement.CurrentAssets;
        var currentLiabilities = (double)statement.CurrentLiabilities;
        var netIncome = (double)statement.NetIncome;
        var previousNetIncome = (double)statement.PreviousNetIncome;
        var fundsFromOperations = (double)statement.FundsFromOperations;

        var size = Math.Log10(totalAssets / gnpPriceIndex);
        var tlta = totalLiabilities / totalAssets;
        var wcta = workingCapital / totalAssets;
        var clca = currentLiabilities / currentAssets;
        var nita = netIncome / totalAssets;
        var futl = fundsFromOperations / totalLiabilities;

        var intwo = netIncome < 0 && previousNetIncome < 0 ? 1.0 : 0.0;
        var oeneg = totalLiabilities > totalAssets ? 1.0 : 0.0;

        var oScore = -1.32
            - 0.407 * size
            + 6.03 * tlta
            - 1.43 * wcta
            + 0.0757 * clca
            - 2.37 * nita
            - 1.83 * futl
            + 0.285 * intwo
            - 1.72 * oeneg;

        var probability = 1.0 / (1.0 + Math.Exp(-oScore));
        var decimalProbability = (decimal)probability;

        var riskLevel = decimalProbability switch
        {
            < 0.20m => RiskLevel.Safe,
            >= 0.20m and <= 0.50m => RiskLevel.Grey,
            _ => RiskLevel.Distress
        };

        return new RiskResult
        {
            Model = ModelName,
            Score = decimalProbability,
            RiskLevel = riskLevel,
            Interpretation = $"Probability of bankruptcy: {decimalProbability:P2}"
        };
    }
}
