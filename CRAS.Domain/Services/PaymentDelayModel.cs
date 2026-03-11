using CRAS.Domain.Entities;
using CRAS.Domain.Enums;
using CRAS.Domain.Interfaces;
using CRAS.Domain.Models;

namespace CRAS.Domain.Services;

/// <summary>
///     Evaluates contractor risk by analyzing payment delays, prioritizing high-value invoices,
///     and accounting for currently overdue unpaid documents.
/// </summary>
public class PaymentDelayModel : IBehavioralRiskModel
{
    public RiskResult CalculateRisk(Contractor contractor)
    {
        var relevantInvoices = contractor.Invoices
            .Where(i => i.IsPaid || !i.IsPaid && i.DueDate < DateTime.UtcNow)
            .ToList();

        if (relevantInvoices.Count == 0)
        {
            return new RiskResult { Model = "Weighted Payment Delay", RiskLevel = RiskLevel.Grey };
        }

        var totalAmount = relevantInvoices.Sum(i => i.Amount);

        if (totalAmount == 0)
        {
            return new RiskResult { Model = "Weighted Payment Delay", RiskLevel = RiskLevel.Grey };
        }

        var weightedDelay = relevantInvoices
            .Sum(i => i.DelayInDays * (i.Amount / totalAmount));

        var level = weightedDelay switch
        {
            < 5 => RiskLevel.Safe,
            >= 5 and < 15 => RiskLevel.Grey,
            _ => RiskLevel.Distress
        };

        return new RiskResult
        {
            Model = "Weighted Payment Delay",
            RiskLevel = level,
            Score = weightedDelay
        };
    }
}
