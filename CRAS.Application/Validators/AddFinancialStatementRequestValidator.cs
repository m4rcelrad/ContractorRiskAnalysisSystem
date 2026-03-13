using CRAS.Application.Requests;
using FluentValidation;

namespace CRAS.Application.Validators;

public class AddFinancialStatementRequestValidator : AbstractValidator<AddFinancialStatementRequest>
{
    public AddFinancialStatementRequestValidator()
    {
        RuleFor(x => x.ContractorId)
            .NotEmpty().WithMessage("ContractorId is required.");

        RuleFor(x => x.FiscalYear)
            .GreaterThan(1900).WithMessage("FiscalYear must be greater than 1900.")
            .LessThanOrEqualTo(DateTime.UtcNow.Year).WithMessage("FiscalYear must be less than or equal to the current year.");

        RuleFor(x => x.TotalAssets)
            .GreaterThan(0).WithMessage("TotalAssets must be greater than 0.");

        RuleFor(x => x.TotalLiabilities)
            .GreaterThan(0).WithMessage("TotalLiabilities must be greater than 0.");

        RuleFor(x => x.CurrentAssets)
            .GreaterThan(0).WithMessage("CurrentAssets must be greater than 0.");

        RuleFor(x => x.CurrentLiabilities)
            .GreaterThan(0).WithMessage("CurrentLiabilities must be greater than 0.");

        RuleFor(x => x.WorkingCapital)
            .GreaterThan(0).WithMessage("WorkingCapital must be greater than 0.");

        RuleFor(x => x.RetainedEarnings)
            .GreaterThan(0).WithMessage("RetainedEarnings must be greater than 0.");

        RuleFor(x => x.EBIT)
            .GreaterThan(0).WithMessage("EBIT must be greater than 0.");

        RuleFor(x => x.MarketValueEquity)
            .GreaterThan(0).WithMessage("MarketValueEquity must be greater than 0.");

        RuleFor(x => x.BookValueEquity)
            .GreaterThan(0).WithMessage("BookValueEquity must be greater than 0.");

        RuleFor(x => x.Sales)
            .GreaterThan(0).WithMessage("Sales must be greater than 0.");

        RuleFor(x => x.NetIncome)
            .GreaterThan(0).WithMessage("NetIncome must be greater than 0.");

        RuleFor(x => x.PreviousNetIncome)
            .GreaterThan(0).WithMessage("PreviousNetIncome must be greater than 0.");

        RuleFor(x => x.FundsFromOperations)
            .GreaterThan(0).WithMessage("FundsFromOperations must be greater than 0.");

        RuleFor(x => x.GNPPriceIndex)
            .GreaterThan(0).WithMessage("GNPPriceIndex must be greater than 0.");
    }
}
