using CRAS.Application.Requests;
using FluentValidation;

namespace CRAS.Application.Validators;

/// <summary>
///     Defines a validator for <see cref="AddInvoiceRequest" />.
/// </summary>
public class AddInvoiceRequestValidator : AbstractValidator<AddInvoiceRequest>
{
    public AddInvoiceRequestValidator()
    {
        RuleFor(x => x.ContractorId)
            .NotEmpty().WithMessage("ContractorId is required.");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be greater than 0.");

        RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency is required.")
            .Length(3).WithMessage("Currency string must be 3 characters long.");

        RuleFor(x => x.IssueDate)
            .LessThan(DateTime.UtcNow).WithMessage("IssueDate must be in the past.");

        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(x => x.IssueDate).WithMessage("DueDate must be after or equal to IssueDate.");
    }
}
