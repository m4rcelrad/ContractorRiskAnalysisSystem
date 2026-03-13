using CRAS.Domain.Entities;
using CRAS.Domain.Enums;
using CRAS.Domain.Interfaces;
using CRAS.Domain.Services;
using Moq;

namespace CRAS.Tests.Domain.Services;

/// <summary>
///     Contains unit tests for the <see cref="PaymentDelayModel" />.
/// </summary>
/// <remarks>
///     These tests verify the weighted average calculation logic, integration with currency exchange rates,
///     and ensure that unpaid overdue invoices are correctly factored into the risk assessment asynchronously.
/// </remarks>
public class PaymentDelayModelTests
{
    private readonly Contractor _contractor = new() { TaxId = "0000000000" };
    private readonly Mock<IExchangeRateService> _exchangeRateMock = new();
    private readonly PaymentDelayModel _model;

    /// <summary>
    ///     Initializes a new instance of the <see cref="PaymentDelayModelTests"/> class.
    ///     Sets up a default mock for the exchange rate service returning a 1:1 rate.
    /// </summary>
    public PaymentDelayModelTests()
    {
        _exchangeRateMock.Setup(x => x.GetExchangeRateAsync(It.IsAny<string>(), It.IsAny<DateTime>()))
            .ReturnsAsync(1m);
        _model = new PaymentDelayModel(_exchangeRateMock.Object);
    }

    /// <summary>
    ///     Verifies that the model returns <see cref="RiskLevel.Safe" /> when the weighted
    ///     average delay is within the safe threshold (less than 5 days).
    /// </summary>
    [Fact]
    public async Task CalculateRiskAsync_ReturnsSafe_WhenWeightedDelayIsLow()
    {
        _contractor.Invoices.Add(new Invoice
        {
            ContractorId = _contractor.Id,
            Amount = 1000m,
            Currency = "PLN",
            IssueDate = DateTime.UtcNow.AddDays(-20),
            DueDate = DateTime.UtcNow.AddDays(-10),
            PaymentDate = DateTime.UtcNow.AddDays(-10),
            IsPaid = true
        });

        _contractor.Invoices.Add(new Invoice
        {
            ContractorId = _contractor.Id,
            Amount = 1000m,
            Currency = "PLN",
            IssueDate = DateTime.UtcNow.AddDays(-20),
            DueDate = DateTime.UtcNow.AddDays(-10),
            PaymentDate = DateTime.UtcNow.AddDays(-6),
            IsPaid = true
        });

        var result = await _model.CalculateRiskAsync(_contractor);

        Assert.Equal(RiskLevel.Safe, result.RiskLevel);
        Assert.True(result.Score < 5m);
    }

    /// <summary>
    ///     Verifies that the model returns <see cref="RiskLevel.Distress" /> when a high-value invoice
    ///     is significantly overdue, even if other smaller invoices were paid on time.
    /// </summary>
    [Fact]
    public async Task CalculateRiskAsync_ReturnsDistress_WhenHighValueInvoiceIsSeverelyOverdue()
    {
        _contractor.Invoices.Add(new Invoice
        {
            ContractorId = _contractor.Id,
            Amount = 10000m,
            Currency = "PLN",
            IssueDate = DateTime.UtcNow.AddDays(-45),
            DueDate = DateTime.UtcNow.AddDays(-30),
            IsPaid = false
        });

        _contractor.Invoices.Add(new Invoice
        {
            ContractorId = _contractor.Id,
            Amount = 100m,
            Currency = "PLN",
            IssueDate = DateTime.UtcNow.AddDays(-20),
            DueDate = DateTime.UtcNow.AddDays(-10),
            PaymentDate = DateTime.UtcNow.AddDays(-10),
            IsPaid = true
        });

        var result = await _model.CalculateRiskAsync(_contractor);

        Assert.Equal(RiskLevel.Distress, result.RiskLevel);
    }

    /// <summary>
    ///     Verifies that the model returns <see cref="RiskLevel.Grey" /> when the weighted
    ///     average delay falls between the safe and distress thresholds.
    /// </summary>
    [Fact]
    public async Task CalculateRiskAsync_ReturnsGrey_WhenWeightedDelayIsModerate()
    {
        _contractor.Invoices.Add(new Invoice
        {
            ContractorId = _contractor.Id,
            Amount = 1000m,
            Currency = "PLN",
            IssueDate = DateTime.UtcNow.AddDays(-30),
            DueDate = DateTime.UtcNow.AddDays(-20),
            PaymentDate = DateTime.UtcNow.AddDays(-10),
            IsPaid = true
        });

        var result = await _model.CalculateRiskAsync(_contractor);

        Assert.Equal(RiskLevel.Grey, result.RiskLevel);
        Assert.Equal(10m, result.Score);
    }

    /// <summary>
    ///     Verifies that the model returns <see cref="RiskLevel.Grey" /> when no relevant
    ///     invoices (paid or overdue) are available for analysis.
    /// </summary>
    [Fact]
    public async Task CalculateRiskAsync_ReturnsGrey_WhenNoRelevantInvoicesExist()
    {
        _contractor.Invoices.Add(new Invoice
        {
            ContractorId = _contractor.Id,
            Amount = 1000m,
            Currency = "PLN",
            IssueDate = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(10),
            IsPaid = false
        });

        var result = await _model.CalculateRiskAsync(_contractor);

        Assert.Equal(RiskLevel.Grey, result.RiskLevel);
    }
}
