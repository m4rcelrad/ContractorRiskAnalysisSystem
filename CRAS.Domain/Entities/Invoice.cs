using System.Text.Json.Serialization;

namespace CRAS.Domain.Entities;

/// <summary>
///     Represents a financial invoice issued to a contractor, used for behavioral risk tracking.
/// </summary>
/// <remarks>
///     This entity is a key part of calculating payment-based risk metrics,
///     such as average payment delays or default probabilities.
/// </remarks>
public class Invoice
{
    /// <summary>
    ///     Gets or sets the unique identifier for the invoice.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    ///     Gets or sets the identifier of the contractor associated with this invoice.
    /// </summary>
    public required Guid ContractorId { get; init; }

    /// <summary>
    ///     Gets or sets the total gross amount of the invoice.
    /// </summary>
    public required decimal Amount { get; init; }

    /// <summary>
    ///     Gets or sets the three-letter ISO currency code.
    /// </summary>
    public string Currency { get; set; } = "PLN";

    /// <summary>
    ///     Gets or sets the date when the invoice was issued.
    /// </summary>
    public required DateTime IssueDate { get; init; }

    /// <summary>
    ///     Gets or sets the formal deadline for payment.
    /// </summary>
    public required DateTime DueDate { get; set; }

    /// <summary>
    ///     Gets or sets the actual date when the payment was received.
    ///     Returns null if the invoice remains unpaid.
    /// </summary>
    public DateTime? PaymentDate { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether the invoice has been fully paid.
    /// </summary>
    public bool IsPaid { get; set; }

    /// <summary>
    ///     Gets the navigation property to the associated contractor.
    /// </summary>
    [JsonIgnore]
    public Contractor Contractor { get; init; } = null!;

    /// <summary>
    ///     Calculates the number of days the payment is (or was) delayed beyond the due date.
    /// </summary>
    /// <remarks>
    ///     If the invoice is unpaid, the delay is calculated relative to the current UTC time.
    ///     Returns 0 if paid on time or if the deadline has not yet passed.
    /// </remarks>
    public int DelayInDays
    {
        get
        {
            var referenceDate = PaymentDate ?? DateTime.UtcNow;
            return referenceDate <= DueDate ? 0 : (referenceDate - DueDate).Days;
        }
    }
}
