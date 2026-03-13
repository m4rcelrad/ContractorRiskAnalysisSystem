namespace CRAS.Application.Requests;

/// <summary>
///     Represents a request to add a new contractor to the system.
/// </summary>
public class AddContractorRequest
{
    public string TaxId { get; set; } = string.Empty;
}
