using CRAS.Domain.Entities;
using CRAS.Domain.Enums;
using CRAS.Domain.Services;

namespace CRAS.Tests.Domain.Services;

/// <summary>
///     Contains unit tests for the <see cref="PaymentDelayModel" />.
/// </summary>
/// <remarks>
///     These tests verify the weighted average calculation logic and ensure that
///     unpaid overdue invoices are correctly factored into the risk assessment.
///     Instantiations are updated to satisfy the required properties of the Invoice entity.
/// </remarks>
public class PaymentDelayModelTests
{
    private readonly Contractor _contractor = new() { TaxId = "0000000000" };
    private readonly PaymentDelayModel _model = new();

    /// <summary>
    ///     Verifies that the model returns <see cref="RiskLevel.Safe" /> when the weighted
    ///     average delay is within the safe threshold (less than 5 days).
    /// </summary>
    [Fact]
    public void CalculateRisk_ReturnsSafe_WhenWeightedDelayIsLow()
    {
        _contractor.Invoices =
        [
            new Invoice
            {
                ContractorId = _contractor.Id,
                Amount = 1000m,
                IssueDate = DateTime.UtcNow.AddDays(-20),
                DueDate = DateTime.UtcNow.AddDays(-10),
                PaymentDate = DateTime.UtcNow.AddDays(-10),
                IsPaid = true
            },
            new Invoice
            {
                ContractorId = _contractor.Id,
                Amount = 1000m,
                IssueDate = DateTime.UtcNow.AddDays(-20),
                DueDate = DateTime.UtcNow.AddDays(-10),
                PaymentDate = DateTime.UtcNow.AddDays(-6),
                IsPaid = true
            }
        ];

        var result = _model.CalculateRisk(_contractor);

        Assert.Equal(RiskLevel.Safe, result.RiskLevel);
        Assert.True(result.Score < 5m);
    }

    /// <summary>
    ///     Verifies that the model returns <see cref="RiskLevel.Distress" /> when a high-value invoice
    ///     is significantly overdue, even if other smaller invoices were paid on time.
    /// </summary>
    [Fact]
    public void CalculateRisk_ReturnsDistress_WhenHighValueInvoiceIsSeverelyOverdue()
    {
        _contractor.Invoices =
        [
            new Invoice
            {
                ContractorId = _contractor.Id,
                Amount = 10000m,
                IssueDate = DateTime.UtcNow.AddDays(-45),
                DueDate = DateTime.UtcNow.AddDays(-30),
                IsPaid = false
            },
            new Invoice
            {
                ContractorId = _contractor.Id,
                Amount = 100m,
                IssueDate = DateTime.UtcNow.AddDays(-20),
                DueDate = DateTime.UtcNow.AddDays(-10),
                PaymentDate = DateTime.UtcNow.AddDays(-10),
                IsPaid = true
            }
        ];

        var result = _model.CalculateRisk(_contractor);

        Assert.Equal(RiskLevel.Distress, result.RiskLevel);
    }

    /// <summary>
    ///     Verifies that the model returns <see cref="RiskLevel.Grey" /> when the weighted
    ///     average delay falls between the safe and distress thresholds.
    /// </summary>
    [Fact]
    public void CalculateRisk_ReturnsGrey_WhenWeightedDelayIsModerate()
    {
        _contractor.Invoices =
        [
            new Invoice
            {
                ContractorId = _contractor.Id,
                Amount = 1000m,
                IssueDate = DateTime.UtcNow.AddDays(-30),
                DueDate = DateTime.UtcNow.AddDays(-20),
                PaymentDate = DateTime.UtcNow.AddDays(-10),
                IsPaid = true
            }
        ];

        var result = _model.CalculateRisk(_contractor);

        Assert.Equal(RiskLevel.Grey, result.RiskLevel);
        Assert.Equal(10m, result.Score);
    }

    /// <summary>
    ///     Verifies that the model returns <see cref="RiskLevel.Grey" /> when no relevant
    ///     invoices (paid or overdue) are available for analysis.
    /// </summary>
    [Fact]
    public void CalculateRisk_ReturnsGrey_WhenNoRelevantInvoicesExist()
    {
        _contractor.Invoices =
        [
            new Invoice
            {
                ContractorId = _contractor.Id,
                Amount = 1000m,
                IssueDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(10),
                IsPaid = false
            }
        ];

        var result = _model.CalculateRisk(_contractor);

        Assert.Equal(RiskLevel.Grey, result.RiskLevel);
    }
}
