using Logged.Sdk.Core;
using Microsoft.AspNetCore.Http;
using Utility.Sdk.Dtos;
using IExceptionHandler = Microsoft.AspNetCore.Diagnostics.IExceptionHandler;

namespace App.Distrbute.Api.Common.Handlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ICoolLogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ICoolLogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        IApiResponse<object>? problemDetails;

        switch (exception)
        {
            default:
            {
                _logger.LogError(
                    exception, nameof(TryHandleAsync), "Exception occurred: {Message}", exception.Message);

                problemDetails = ApiResponse<object>.Default.ToServerErrorApiResponse(
                    "An error occurred, please try again later");
                break;
            }
        }

        httpContext.Response.StatusCode = problemDetails.Code;

        await httpContext.Response
            .WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}