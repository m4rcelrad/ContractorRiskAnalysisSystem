using CRAS.Application.Requests;
using CRAS.Domain.Entities;
using FluentValidation;

namespace CRAS.Application.Validators;

/// <summary>
///     Defines a validator for <see cref="AddContractorRequest" />.
/// </summary>
public class AddContractRequestValidator : AbstractValidator<AddContractorRequest>
{
    public AddContractRequestValidator()
    {
        RuleFor(x => x.TaxId)
            .Must(Contractor.IsValidTaxId).WithMessage("Invalid Tax ID format.");
    }
}
