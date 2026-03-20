using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CRAS.Api.Filters;

/// <summary>
///     A filter that performs validation on action method parameters using FluentValidation.
/// </summary>
/// <remarks>
///     This filter ensures that DTOs provided to an action method are validated
///     before the action logic is executed. If validation fails, the request is
///     short-circuited, and a bad request response is returned with the validation errors.
/// </remarks>
public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values.Where(v => v != null))
        {
            var argumentType = argument!.GetType();

            var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);

            if (context.HttpContext.RequestServices.GetService(validatorType) is not IValidator validator) continue;
            var validationContext = new ValidationContext<object>(argument);
            var validationResult = await validator.ValidateAsync(validationContext);

            if (validationResult.IsValid) continue;
            context.Result = new BadRequestObjectResult(validationResult.Errors);
            return;
        }

        await next();
    }
}
