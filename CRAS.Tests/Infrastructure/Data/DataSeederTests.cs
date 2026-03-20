using CRAS.Domain.Entities;
using CRAS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRAS.Tests.Infrastructure;

/// <summary>
/// Unit tests for the <see cref="DataSeeder"/> class to ensure test data
/// generation works correctly without duplicating existing records [cite: 8-10].
/// </summary>
public class DataSeederTests
{
    /// <summary>
    /// Verifies that the seeder successfully generates and saves the expected
    /// amount of dummy data when the database is initially empty.
    /// </summary>
    [Fact]
    public void Seed_WhenDatabaseIsEmpty_ShouldPopulateWithDummyData()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);

        DataSeeder.Seed(context);

        Assert.Equal(50, context.Contractors.Count());
        Assert.Equal(150, context.FinancialStatements.Count());
        Assert.Equal(750, context.Invoices.Count());
    }

    /// <summary>
    /// Verifies that the seeder aborts execution and makes no changes if it
    /// detects that the database already contains contractor records.
    /// </summary>
    [Fact]
    public void Seed_WhenDatabaseHasData_ShouldNotDuplicateData()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new AppDbContext(options);

        context.Contractors.Add(new Contractor { TaxId = "7740001454" });
        context.SaveChanges();

        DataSeeder.Seed(context);

        Assert.Equal(1, context.Contractors.Count());
        Assert.Equal(0, context.FinancialStatements.Count());
        Assert.Equal(0, context.Invoices.Count());
    }
}
