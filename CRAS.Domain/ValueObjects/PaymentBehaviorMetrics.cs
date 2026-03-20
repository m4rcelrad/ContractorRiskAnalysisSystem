namespace CRAS.Domain.ValueObjects;

/// <summary>
///     Represent a record of payment behavior metrics.
/// </summary>
/// <param name="TotalUnpaidAmount"></param>
/// <param name="AverageDelayDays"></param>
/// <param name="UnpaidRatio"></param>
/// <param name="UnpaidCount"></param>
public record PaymentBehaviorMetrics(
    decimal TotalUnpaidAmount,
    double AverageDelayDays,
    decimal UnpaidRatio,
    int UnpaidCount
);
