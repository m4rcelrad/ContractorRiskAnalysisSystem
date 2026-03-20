using CRAS.Application.Requests;
using CRAS.Application.Validators;
using FluentValidation.TestHelper;

namespace CRAS.Tests.Application.Validators;

/// <summary>
///     Unit tests for <see cref="AddContractRequestValidator" /> .
/// </summary>
public class AddContractRequestValidatorTests
{
    private readonly AddContractRequestValidator _validator = new();

    /// <summary>
    ///     Verifies that a valid Polish Tax ID passes validation without errors.
    /// </summary>
    [Fact]
    public void Should_NotHaveError_When_TaxIdIsValid()
    {
        var request = new AddContractorRequest { TaxId = "7740001454" };
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.TaxId);
    }

    /// <summary>
    ///     Verifies that an invalid Tax ID triggers the appropriate validation error.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("INVALIDNIP")]
    [InlineData("1234567890")]
    public void Should_HaveError_When_TaxIdIsInvalid(string invalidTaxId)
    {
        var request = new AddContractorRequest { TaxId = invalidTaxId };
        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.TaxId)
            .WithErrorMessage("Invalid Tax ID format.");
    }
}
