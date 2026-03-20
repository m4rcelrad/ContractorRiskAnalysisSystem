using CRAS.Application.Requests;
using CRAS.Application.Validators;
using FluentValidation.TestHelper;

namespace CRAS.Tests.Application.Validators;

/// <summary>
///     Unit tests for <see cref="AddFinancialStatementRequestValidator" /> ensuring strict
///     positive value requirements for financial metrics [cite: 29-44].
/// </summary>
public class AddFinancialStatementRequestValidatorTests
{
    private readonly AddFinancialStatementRequestValidator _validator = new();

    /// <summary>
    ///     Verifies that validation fails if the fiscal year is out of sensible bounds[cite: 30].
    /// </summary>
    [Theory]
    [InlineData(1899)]
    [InlineData(3000)]
    public void Should_HaveError_When_FiscalYearIsOutOfBounds(int invalidYear)
    {
        var request = new AddFinancialStatementRequest { FiscalYear = invalidYear };
        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.FiscalYear);
    }

    /// <summary>
    ///     Verifies that all financial metrics reject zero or negative values .
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(-100)]
    public void Should_HaveErrors_When_FinancialValuesAreNotPositive(decimal invalidValue)
    {
        var request = new AddFinancialStatementRequest
        {
            TotalAssets = invalidValue,
            TotalLiabilities = invalidValue,
            CurrentAssets = invalidValue,
            CurrentLiabilities = invalidValue,
            WorkingCapital = invalidValue,
            RetainedEarnings = invalidValue,
            EBIT = invalidValue,
            MarketValueEquity = invalidValue,
            BookValueEquity = invalidValue,
            Sales = invalidValue,
            NetIncome = invalidValue,
            PreviousNetIncome = invalidValue,
            FundsFromOperations = invalidValue,
            GNPPriceIndex = invalidValue
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.TotalAssets);
        result.ShouldHaveValidationErrorFor(x => x.TotalLiabilities);
        result.ShouldHaveValidationErrorFor(x => x.CurrentAssets);
        result.ShouldHaveValidationErrorFor(x => x.CurrentLiabilities);
        result.ShouldHaveValidationErrorFor(x => x.WorkingCapital);
        result.ShouldHaveValidationErrorFor(x => x.RetainedEarnings);
        result.ShouldHaveValidationErrorFor(x => x.EBIT);
        result.ShouldHaveValidationErrorFor(x => x.MarketValueEquity);
        result.ShouldHaveValidationErrorFor(x => x.BookValueEquity);
        result.ShouldHaveValidationErrorFor(x => x.Sales);
        result.ShouldHaveValidationErrorFor(x => x.NetIncome);
        result.ShouldHaveValidationErrorFor(x => x.PreviousNetIncome);
        result.ShouldHaveValidationErrorFor(x => x.FundsFromOperations);
        result.ShouldHaveValidationErrorFor(x => x.GNPPriceIndex);
    }

    /// <summary>
    ///     Verifies that validation passes when all required fields have valid, positive data.
    /// </summary>
    [Fact]
    public void Should_NotHaveErrors_When_AllDataIsValid()
    {
        var request = new AddFinancialStatementRequest
        {
            ContractorId = Guid.NewGuid(),
            FiscalYear = 2023,
            TotalAssets = 100, TotalLiabilities = 100, CurrentAssets = 100,
            CurrentLiabilities = 100, WorkingCapital = 100, RetainedEarnings = 100,
            EBIT = 100, MarketValueEquity = 100, BookValueEquity = 100,
            Sales = 100, NetIncome = 100, PreviousNetIncome = 100,
            FundsFromOperations = 100, GNPPriceIndex = 1
        };

        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
