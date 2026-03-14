using Bogus;
using CRAS.Domain.Entities;

namespace CRAS.Infrastructure.Data;

/// <summary>
///     Provides functionality to seed the database with realistic test data using the Bogus library.
/// </summary>
public static class DataSeeder
{
    /// <summary>
    ///     Populates the database with initial test data if no contractors currently exist.
    /// </summary>
    /// <param name="context">The database context used to interact with the underlying database.</param>
    public static void Seed(AppDbContext context)
    {
        if (context.Contractors.Any()) return;

        Randomizer.Seed = new Random(2026);

        var contractors = GenerateContractors(50);
        context.Contractors.AddRange(contractors);
        context.SaveChanges();

        var financialStatements = GenerateFinancialStatements(contractors);
        context.FinancialStatements.AddRange(financialStatements);
        context.SaveChanges();

        var invoices = GenerateInvoices(contractors);
        context.Invoices.AddRange(invoices);

        context.SaveChanges();
    }

    /// <summary>
    ///     Generates a list of contractor entities populated with realistic and valid data.
    /// </summary>
    /// <param name="count">The number of contractor entities to generate.</param>
    /// <returns>A list of <see cref="Contractor" /> objects populated with realistic data.</returns>
    private static List<Contractor> GenerateContractors(int count)
    {
        var contractorFaker = new Faker<Contractor>()
            .CustomInstantiator(f => new Contractor
            {
                TaxId = GenerateValidTaxId(f.Random)
            });

        return contractorFaker.Generate(count);
    }

    /// <summary>
    ///     Generates a list of financial statement entities populated with realistic data.
    /// </summary>
    /// <param name="contractors"></param>
    /// <returns>A list of <see cref="FinancialStatement" /> objects populated with realistic data.</returns>
    private static List<FinancialStatement> GenerateFinancialStatements(IEnumerable<Contractor> contractors)
    {
        var statements = new List<FinancialStatement>();
        var currentYear = DateTime.UtcNow.Year;

        foreach (var contractor in contractors)
        {
            var statementFaker = new Faker<FinancialStatement>()
                .CustomInstantiator(f => new FinancialStatement
                {
                    ContractorId = contractor.Id,
                    Year = currentYear - f.Random.Int(1, 3),
                    TotalAssets = f.Finance.Amount(500000m, 5000000m),
                    TotalLiabilities = f.Finance.Amount(100000m, 3000000m),
                    CurrentAssets = f.Finance.Amount(200000m, 2000000m),
                    CurrentLiabilities = f.Finance.Amount(50000m, 1500000m),
                    WorkingCapital = f.Finance.Amount(50000m, 1000000m),
                    RetainedEarnings = f.Finance.Amount(10000m, 500000m),
                    EBIT = f.Finance.Amount(20000m, 800000m),
                    MarketValueEquity = f.Finance.Amount(100000m, 4000000m),
                    BookValueEquity = f.Finance.Amount(100000m, 3000000m),
                    Sales = f.Finance.Amount(500000m, 10000000m),
                    NetIncome = f.Finance.Amount(10000m, 500000m),
                    PreviousNetIncome = f.Finance.Amount(10000m, 400000m),
                    FundsFromOperations = f.Finance.Amount(20000m, 600000m),
                    GNPPriceIndex = f.Finance.Amount(100m, 150m)
                });

            statements.AddRange(statementFaker.Generate(3));
        }

        return statements;
    }

    /// <summary>
    ///     Generates a list of invoice entities populated with realistic data.
    /// </summary>
    /// <param name="contractors"></param>
    /// <returns>A list of <see cref="Invoice" /> objects populated with realistic data.</returns>
    private static List<Invoice> GenerateInvoices(IEnumerable<Contractor> contractors)
    {
        var invoices = new List<Invoice>();

        foreach (var contractor in contractors)
        {
            var invoiceFaker = new Faker<Invoice>()
                .CustomInstantiator(f =>
                {
                    var issueDate = f.Date.Past().ToUniversalTime();
                    var dueDate = issueDate.AddDays(f.PickRandom(14, 30, 60));
                    var isPaid = f.Random.Bool(0.7f);
                    var paymentDate = isPaid ? dueDate.AddDays(f.Random.Int(-5, 30)) : (DateTime?)null;

                    return new Invoice
                    {
                        ContractorId = contractor.Id,
                        Amount = f.Finance.Amount(1000m, 100000m),
                        Currency = f.Finance.Currency().Code,
                        IssueDate = issueDate,
                        DueDate = dueDate,
                        IsPaid = isPaid,
                        PaymentDate = paymentDate
                    };
                });

            invoices.AddRange(invoiceFaker.Generate(15));
        }

        return invoices;
    }

    /// <summary>
    ///     Generates a valid tax ID number using the Luhn algorithm.
    /// </summary>
    /// <param name="randomizer"></param>
    /// <returns>A string containing a valid tax ID.</returns>
    private static string GenerateValidTaxId(Randomizer randomizer)
    {
        int[] weights = [ 6, 5, 7, 2, 3, 4, 5, 6, 7 ];

        while (true)
        {
            var digits = new int[9];
            var sum = 0;

            for (var i = 0; i < 9; i++)
            {
                digits[i] = randomizer.Int(0, 9);
                sum += digits[i] * weights[i];
            }

            var checksum = sum % 11;

            if (checksum == 10) continue;

            return string.Join("", digits) + checksum;
        }
    }
}
