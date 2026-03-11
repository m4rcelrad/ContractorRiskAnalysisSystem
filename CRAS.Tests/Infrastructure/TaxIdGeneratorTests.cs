using System.Reflection;
using Bogus;
using CRAS.Infrastructure.Data;

namespace CRAS.Tests.Infrastructure;

/// <summary>
///     Provides a suite of unit tests for validating the functionality of tax ID generation in the application.
/// </summary>
/// <remarks>
///     This class contains test methods to ensure that the tax ID generation logic in the <see cref="DataSeeder" /> class
///     produces valid tax IDs, adhering to the expected checksum verification rules. The tests involve dynamic invocation
///     of private methods using reflection and validation against checksum algorithms.
/// </remarks>
public class TaxIdGeneratorTests
{
    [Fact]
    public void GenerateValidTaxId_ShouldAlwaysProduceValidChecksum()
    {
        var randomizer = new Randomizer();

        for (var i = 0; i < 100; i++)
        {
            var method =
                typeof(DataSeeder).GetMethod("GenerateValidTaxId", BindingFlags.NonPublic | BindingFlags.Static);
            var taxId = (string)method!.Invoke(null, [randomizer])!;

            Assert.True(IsValidTaxId(taxId));
        }
    }

    private static bool IsValidTaxId(string taxId)
    {
        if (string.IsNullOrWhiteSpace(taxId) || taxId.Length != 10) return false;
        int[] weights = [6, 5, 7, 2, 3, 4, 5, 6, 7];
        var sum = 0;
        for (var i = 0; i < 9; i++)
            sum += (taxId[i] - '0') * weights[i];

        return sum % 11 == taxId[9] - '0';
    }
}