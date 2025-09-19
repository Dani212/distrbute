using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Utility.Sdk.Dtos;

namespace App.Distrbute.Api.Common.Middlewares;

public class DtoValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errorDetails = context.ModelState
                .Where(m => m.Value?.Errors.Count > 0)
                .SelectMany(m => m.Value!.Errors.Select(e =>
                    string.IsNullOrWhiteSpace(m.Key)
                        ? e.ErrorMessage
                        : $"{m.Key}: {e.ErrorMessage}"))
                .ToList();
            
            var combinedMessage = "Let’s try that again. Some details are missing or need to be corrected: " +
                                  string.Join("; ", errorDetails);


            context.Result = new BadRequestObjectResult(
                new ApiResponse<object>(
                    combinedMessage,
                    400)
            );
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
