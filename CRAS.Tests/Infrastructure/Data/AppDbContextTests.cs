using CRAS.Domain.Entities;
using CRAS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRAS.Tests.Infrastructure;

/// <summary>
///     Unit tests for <see cref="AppDbContext" /> focusing on Entity Framework
///     configurations and data persistence[cite: 2].
/// </summary>
public class AppDbContextTests
{
    /// <summary>
    ///     Verifies that the global DateTime converter correctly forces UTC kind
    ///     when saving entities to the database[cite: 6, 7].
    /// </summary>
    [Fact]
    public async Task SaveChanges_ShouldForceUtcDateTimeKind()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        await using var context = new AppDbContext(options);
        var localTime = DateTime.Now;

        var invoice = new Invoice
        {
            ContractorId = Guid.NewGuid(),
            Amount = 500,
            IssueDate = localTime,
            DueDate = localTime.AddDays(14)
        };

        context.Invoices.Add(invoice);
        await context.SaveChangesAsync();

        context.ChangeTracker.Clear();

        var savedInvoice = await context.Invoices.FirstAsync();
        Assert.Equal(DateTimeKind.Utc, savedInvoice.IssueDate.Kind);
    }
}
