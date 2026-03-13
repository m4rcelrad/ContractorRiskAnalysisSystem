namespace CRAS.Application.Requests;

/// <summary>
///     Represents a request to add a new invoice to the system.
/// </summary>
public class AddInvoiceRequest
{
    public Guid ContractorId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
}
