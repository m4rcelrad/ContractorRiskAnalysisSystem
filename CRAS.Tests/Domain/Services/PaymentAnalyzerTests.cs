using CRAS.Domain.Entities;
using CRAS.Domain.Services;

namespace CRAS.Tests.Domain.Services;

/// <summary>
/// Unit tests for the <see cref="PaymentAnalyzer"/> service.
/// </summary>
public class PaymentAnalyzerTests
{
    private readonly PaymentAnalyzer _analyzer = new();

    /// <summary>
    /// Verifies that <see cref="PaymentAnalyzer.Analyze"/> returns zeroed metrics
    /// when provided with an empty collection of invoices.
    /// </summary>
    [Fact]
    public void Analyze_WithEmptyCollection_ReturnsZeroMetrics()
    {
        var result = _analyzer.Analyze(new List<Invoice>());

        Assert.Equal(0m, result.TotalUnpaidAmount);
        Assert.Equal(0, result.AverageDelayDays);
        Assert.Equal(0m, result.UnpaidRatio);
        Assert.Equal(0, result.UnpaidCount);
    }

    /// <summary>
    /// Verifies that <see cref="PaymentAnalyzer.Analyze"/> correctly calculates
    /// totals, average delays, and unpaid ratios for a mixed set of invoices.
    /// </summary>
    [Fact]
    public void Analyze_WithMixedInvoices_CalculatesCorrectMetrics()
    {
        var contractorId = Guid.NewGuid();
        var today = DateTime.UtcNow.Date;

        var invoices = new List<Invoice>
        {
            new()
            {
                ContractorId = contractorId, Amount = 1000, IsPaid = true, DueDate = today.AddDays(-10), PaymentDate = today.AddDays(-5),
                IssueDate = today.AddDays(-20)
            },
            new()
            {
                ContractorId = contractorId, Amount = 2000, IsPaid = true, DueDate = today.AddDays(-5), PaymentDate = today.AddDays(-5),
                IssueDate = today.AddDays(-15)
            },
            new() { ContractorId = contractorId, Amount = 5000, IsPaid = false, DueDate = today, IssueDate = today.AddDays(-5) }
        };

        var result = _analyzer.Analyze(invoices);

        Assert.Equal(5000m, result.TotalUnpaidAmount);
        Assert.Equal(2.5, result.AverageDelayDays);
        Assert.Equal(1m / 3m, result.UnpaidRatio);
        Assert.Equal(1, result.UnpaidCount);
    }
}
