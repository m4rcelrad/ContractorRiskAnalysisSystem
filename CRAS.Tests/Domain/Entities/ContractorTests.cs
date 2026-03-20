using CRAS.Domain.Entities;

namespace CRAS.Tests.Domain.Entities;

/// <summary>
/// Unit tests for the <see cref="Contractor"/> class.
/// </summary>
/// <remarks>
/// This class contains tests to validate the behavior of the Contractor's Tax ID (NIP) validation logic,
/// ensuring the correctness of the checksum algorithm and proper handling of valid and invalid inputs.
/// </remarks>
public class ContractorTests
{
    /// <summary>
    /// Verifies that <see cref="Contractor.IsValidTaxId"/> returns true for numerically correct
    /// Polish Tax Identification Numbers (NIP).
    /// </summary>
    /// <param name="taxId">A valid 10-digit NIP string to be tested.</param>
    [Theory]
    [InlineData("7740001454")]
    [InlineData("5260250995")]
    public void IsValidTaxId_ReturnsTrue_ForValidNip(string taxId)
    {
        var result = Contractor.IsValidTaxId(taxId);
        Assert.True(result);
    }

    /// <summary>
    /// Verifies that <see cref="Contractor.IsValidTaxId"/> returns false for inputs that fail
    /// format, length, or checksum requirements.
    /// </summary>
    /// <param name="taxId">An invalid NIP string (e.g., wrong length, containing letters, or incorrect checksum).</param>
    [Theory]
    [InlineData("1234567890")]
    [InlineData("ABC1234567")]
    [InlineData("123-456-78")]
    public void IsValidTaxId_ReturnsFalse_ForInvalidNip(string taxId)
    {
        var result = Contractor.IsValidTaxId(taxId);
        Assert.False(result);
    }
}
