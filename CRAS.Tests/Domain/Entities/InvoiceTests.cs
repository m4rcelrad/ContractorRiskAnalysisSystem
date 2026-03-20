using CRAS.Domain.Entities;

namespace CRAS.Tests.Domain.Entities;

/// <summary>
///     Unit tests for the <see cref="Invoice" /> class.
/// </summary>
/// <remarks>
///     This class ensures the correctness of behavioral risk metrics calculations,
///     specifically focusing on the payment delay logic[cite: 176, 195].
/// </remarks>
public class InvoiceTests
{
    /// <summary>
    ///     Verifies that <see cref="Invoice.DelayInDays" /> returns zero when the invoice is paid
    ///     on or before its due date[cite: 198].
    /// </summary>
    [Fact]
    public void DelayInDays_ReturnsZero_WhenPaidOnTime()
    {
        var date = DateTime.UtcNow;
        var invoice = new Invoice
        {
            ContractorId = Guid.NewGuid(),
            Amount = 1000,
            IssueDate = date.AddDays(-10),
            DueDate = date,
            PaymentDate = date,
            IsPaid = true
        };

        Assert.Equal(0, invoice.DelayInDays);
    }

    /// <summary>
    ///     Verifies that <see cref="Invoice.DelayInDays" /> calculates the correct number of days
    ///     between the due date and the actual payment date for late payments[cite: 199].
    /// </summary>
    [Fact]
    public void DelayInDays_ReturnsPositiveValue_WhenPaidLate()
    {
        var dueDate = DateTime.UtcNow.AddDays(-10);
        var paymentDate = DateTime.UtcNow.AddDays(-5);

        var invoice = new Invoice
        {
            ContractorId = Guid.NewGuid(),
            Amount = 1000,
            IssueDate = DateTime.UtcNow.AddDays(-20),
            DueDate = dueDate,
            PaymentDate = paymentDate,
            IsPaid = true
        };

        Assert.Equal(5, invoice.DelayInDays);
    }

    /// <summary>
    ///     Verifies that <see cref="Invoice.DelayInDays" /> returns zero for unpaid invoices
    ///     whose deadline has not yet passed[cite: 198].
    /// </summary>
    [Fact]
    public void DelayInDays_ReturnsZero_WhenUnpaidButNotOverdue()
    {
        var invoice = new Invoice
        {
            ContractorId = Guid.NewGuid(),
            Amount = 1000,
            IssueDate = DateTime.UtcNow.AddDays(-5),
            DueDate = DateTime.UtcNow.AddDays(5),
            IsPaid = false
        };

        Assert.Equal(0, invoice.DelayInDays);
    }

    /// <summary>
    ///     Verifies that <see cref="Invoice.DelayInDays" /> calculates the delay relative to
    ///     the current time for invoices that are overdue and remain unpaid [cite: 196-199].
    /// </summary>
    [Fact]
    public void DelayInDays_ReturnsPositiveValue_WhenUnpaidAndOverdue()
    {
        var dueDate = DateTime.UtcNow.AddDays(-7);
        var invoice = new Invoice
        {
            ContractorId = Guid.NewGuid(),
            Amount = 1000,
            IssueDate = DateTime.UtcNow.AddDays(-15),
            DueDate = dueDate,
            IsPaid = false
        };

        Assert.Equal(7, invoice.DelayInDays);
    }
}
