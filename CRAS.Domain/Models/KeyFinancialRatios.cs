namespace CRAS.Domain.Models;

/// <summary>
///     Represents a record containing key financial ratios.
/// </summary>
/// <param name="CurrentRatio"></param>
/// <param name="DebtRatio"></param>
/// <param name="ReturnOnEquity"></param>
/// <param name="WorkingCapitalToAssets"></param>
/// <param name="AssetTurnover"></param>
/// <param name="EbitMargin"></param>
public record KeyFinancialRatios(
    decimal CurrentRatio,
    decimal DebtRatio,
    decimal ReturnOnEquity,
    decimal WorkingCapitalToAssets,
    decimal AssetTurnover,
    decimal EbitMargin
);
