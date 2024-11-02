using BlogApi.Shared.Models.Exceptions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace BlogApi.Api.Filters;

public class ModelValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid) return;
        var errorMessages = context.ModelState
            .Where(ms => ms.Value.Errors.Any())
            .SelectMany(ms => ms.Value.Errors)
            .Select(e => e.ErrorMessage)
            .ToArray();
        
        var message = string.Join("; ", errorMessages);
        var errorResponse = new ErrorResponse
        {
            Success = false,
            Error = new ErrorDetail
            {
                Code = 0,
                Message = message,
            }
        };

        context.Result = new BadRequestObjectResult(errorResponse);
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {

    }
}