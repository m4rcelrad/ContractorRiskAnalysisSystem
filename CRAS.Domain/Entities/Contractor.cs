namespace CRAS.Domain.Entities;

/// <summary>
///     Represents a contractor entity.
/// </summary>
public class Contractor
{
    /// <summary>
    ///     Gets the unique identifier for the contractor.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    ///     Gets the contractor's Tax Identification Number (NIP for Polish entities).
    /// </summary>
    /// <exception cref="ArgumentException">
    ///     Thrown when the provided value does not pass the checksum or formatting validation.
    /// </exception>
    public required string TaxId
    {
        get;
        init => field = IsValidTaxId(value) ? value : throw new ArgumentException("Invalid TaxId format or checksum.");
    }

    /// <summary>
    ///     Gets the collection of financial statements associated with the contractor.
    /// </summary>
    public ICollection<FinancialStatement> FinancialStatements { get; private set; } =
        new HashSet<FinancialStatement>();

    /// <summary>
    ///     Gets the collection of invoices associated with this contractor.
    /// </summary>
    public ICollection<Invoice> Invoices { get; private set; } = new List<Invoice>();

    /// <summary>
    ///     Validates a Polish Tax Identification Number (NIP) using standard weights and checksum algorithms.
    /// </summary>
    /// <param name="taxId">The raw string representing the Tax ID.</param>
    /// <returns>True if the NIP has a valid 10-digit format and a correct checksum; otherwise, false.</returns>
    public static bool IsValidTaxId(string taxId)
    {
        if (string.IsNullOrWhiteSpace(taxId)) return false;
        var cleaned = taxId.Replace("-", "");
        if (cleaned.Length != 10 || !cleaned.All(char.IsDigit)) return false;

        int[] weights = [ 6, 5, 7, 2, 3, 4, 5, 6, 7 ];
        var sum = 0;
        for (var i = 0; i < 9; i++)
            sum += (cleaned[i] - '0') * weights[i];

        return sum % 11 == cleaned[9] - '0';
    }
}
