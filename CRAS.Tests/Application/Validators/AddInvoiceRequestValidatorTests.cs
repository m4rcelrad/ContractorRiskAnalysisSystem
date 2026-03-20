using CRAS.Application.Requests;
using CRAS.Application.Validators;
using FluentValidation.TestHelper;

namespace CRAS.Tests.Application.Validators;

/// <summary>
///     Unit tests for <see cref="AddInvoiceRequestValidator" /> validating monetary,
///     string, and date constraints.
/// </summary>
public class AddInvoiceRequestValidatorTests
{
    private readonly AddInvoiceRequestValidator _validator = new();

    /// <summary>
    ///     Verifies that a perfectly valid request passes all validation rules.
    /// </summary>
    [Fact]
    public void Should_NotHaveAnyErrors_When_RequestIsValid()
    {
        var request = new AddInvoiceRequest
        {
            ContractorId = Guid.NewGuid(),
            Amount = 1500.50m,
            Currency = "PLN",
            IssueDate = DateTime.UtcNow.AddDays(-10),
            DueDate = DateTime.UtcNow.AddDays(10)
        };

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    /// <summary>
    ///     Verifies that specific invalid properties trigger their respective validation rules [cite: 46-49].
    /// </summary>
    [Fact]
    public void Should_HaveErrors_When_PropertiesAreInvalid()
    {
        var request = new AddInvoiceRequest
        {
            ContractorId = Guid.Empty,
            Amount = 0,
            Currency = "US",
            IssueDate = DateTime.UtcNow.AddDays(1)
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.ContractorId);
        result.ShouldHaveValidationErrorFor(x => x.Amount);
        result.ShouldHaveValidationErrorFor(x => x.Currency);
        result.ShouldHaveValidationErrorFor(x => x.IssueDate);
        result.ShouldHaveValidationErrorFor(x => x.DueDate);
    }
}
