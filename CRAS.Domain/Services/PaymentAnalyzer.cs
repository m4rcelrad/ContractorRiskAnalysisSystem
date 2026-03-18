using CRAS.Domain.Entities;
using CRAS.Domain.Interfaces;
using CRAS.Domain.Models;

namespace CRAS.Domain.Services;

/// <summary>
///     Represents a service that analyzes payment behaviors based on a collection of invoices.
/// </summary>
public class PaymentAnalyzer : IPaymentAnalyzer
{
    public PaymentBehaviorMetrics Analyze(IReadOnlyCollection<Invoice> invoices)
    {
        if (invoices == null || invoices.Count == 0)
        {
            return new PaymentBehaviorMetrics(0m, 0, 0m, 0);
        }

        var unpaidInvoices = invoices.Where(i => !i.IsPaid).ToList();
        var paidInvoices = invoices.Where(i => i.IsPaid).ToList();

        var totalUnpaidAmount = unpaidInvoices.Sum(i => i.Amount);

        var avgDelay = paidInvoices.Count != 0
            ? paidInvoices.Average(i => (i.PaymentDate!.Value.Date - i.DueDate.Date).TotalDays)
            : 0;

        var unpaidRatio = (decimal)unpaidInvoices.Count / invoices.Count;

        return new PaymentBehaviorMetrics(
            totalUnpaidAmount,
            avgDelay,
            unpaidRatio,
            unpaidInvoices.Count
        );
    }
}
