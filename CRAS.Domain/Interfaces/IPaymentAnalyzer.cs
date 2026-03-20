using CRAS.Domain.Entities;
using CRAS.Domain.ValueObjects;

namespace CRAS.Domain.Interfaces;

/// <summary>
///     Represents an interface for analyzing payment behavior based on a collection of invoices.
/// </summary>
public interface IPaymentAnalyzer
{
    PaymentBehaviorMetrics Analyze(IReadOnlyCollection<Invoice> invoices);
}
