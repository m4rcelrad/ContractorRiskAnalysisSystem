using CRAS.Domain.Entities;
using CRAS.Domain.Enums;
using CRAS.Domain.Interfaces;
using CRAS.Domain.Models;

namespace CRAS.Domain.Services;

/// <summary>
///     Represents a model for assessing the risk of payment delays for a contractor.
/// </summary>
/// <remarks>
///     This model calculates a weighted payment delay score based on the contractor's invoice history.
///     It evaluates invoices that are either paid or past due and calculates a risk level (Safe, Grey, or Distress)
///     using the weighted average delay across the relevant invoices. The model uses an exchange rate service
///     to normalize invoice amounts to a base currency for accurate weight calculation.
/// </remarks>
/// <param name="exchangeRateService">The service used to retrieve exchange rates for currency conversion.</param>
public class PaymentDelayModel(IExchangeRateService exchangeRateService) : IBehavioralRiskModel
{
    /// <summary>
    ///     Asynchronously calculates the behavioral risk based on payment delays.
    /// </summary>
    /// <param name="contractor">The contractor entity containing invoice history.</param>
    /// <returns>A <see cref="RiskResult" /> containing the weighted average delay and the determined risk level.</returns>
    public async Task<RiskResult> CalculateRiskAsync(Contractor contractor)
    {
        if (contractor.Invoices.Count == 0)
        {
            return new RiskResult
            {
                Model = "Weighted Payment Delay",
                Score = 0m,
                RiskLevel = RiskLevel.Grey
            };
        }

        var relevantInvoices = contractor.Invoices
            .Where(i => i.IsPaid || i.DueDate < DateTime.UtcNow)
            .ToList();

        if (relevantInvoices.Count == 0)
        {
            return new RiskResult
            {
                Model = "Weighted Payment Delay",
                Score = 0m,
                RiskLevel = RiskLevel.Grey
            };
        }

        var totalWeight = 0m;
        var weightedDelaySum = 0m;

        foreach (var invoice in relevantInvoices)
        {
            var delayDays = invoice.IsPaid switch
            {
                true when invoice.PaymentDate.HasValue => (invoice.PaymentDate.Value - invoice.DueDate).Days,
                false when invoice.DueDate < DateTime.UtcNow => (DateTime.UtcNow - invoice.DueDate).Days,
                _ => 0
            };

            if (delayDays < 0)
            {
                delayDays = 0;
            }

            var rate = await exchangeRateService.GetExchangeRateAsync(invoice.Currency, invoice.IssueDate);
            var convertedAmount = invoice.Amount * rate;

            weightedDelaySum += delayDays * convertedAmount;
            totalWeight += convertedAmount;
        }

        var averageDelay = totalWeight > 0 ? weightedDelaySum / totalWeight : 0m;

        var riskLevel = averageDelay switch
        {
            < 5 => RiskLevel.Safe,
            < 14 => RiskLevel.Grey,
            _ => RiskLevel.Distress
        };

        return new RiskResult
        {
            Model = "Weighted Payment Delay",
            Score = averageDelay,
            RiskLevel = riskLevel
        };
    }
}
