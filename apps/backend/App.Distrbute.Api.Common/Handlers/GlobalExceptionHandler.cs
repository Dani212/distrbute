using Amazon.Runtime;
using App.Distrbute.Common.Exceptions;
using Ledgr.Sdk.Exceptions;
using Logged.Sdk.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ObjectStorage.Sdk.Exceptions;
using Paystack.Sdk.Exceptions;
using Pipeline.Sdk.Core;
using Quartz;
using Redis.Sdk.Exceptions;
using Refit;
using Rest.Sdk.Exceptions;
using Scheduler.Sdk.Exceptions;
using Socials.Sdk.Exceptions;
using StackExchange.Redis;
using Utility.Sdk.Dtos;
using Utility.Sdk.Exceptions;
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
        Utility.Sdk.Dtos.IApiResponse<object>? problemDetails;

        switch (exception)
        {
            case BadRequest or ArgumentException or BadObjectStorageRequestException:
            {
                _logger.LogError(
                    exception, nameof(TryHandleAsync), "Exception occurred: {Message}", exception.Message);

                problemDetails = Utility.Sdk.Dtos.ApiResponse<object>.Default.ToBadRequestApiResponse(exception.Message);
                break;
            }
            case Conflict:
            {
                problemDetails = Utility.Sdk.Dtos.ApiResponse<object>.Default.ToConflictApiResponse(exception.Message);
                break;
            }
            case DbUpdateException:
            {
                _logger.LogError(
                    exception, nameof(TryHandleAsync), "Exception occurred: {Message}",
                    exception.GetBaseException().Message);

                problemDetails = Utility.Sdk.Dtos.ApiResponse<object>.Default.ToFailedDependencyApiResponse(
                    "We couldn’t save your changes just now. Let’s try again in a moment.");

                break;
            }
            case FailedDependency failedDependency:
                _logger.LogError(
                    exception, nameof(TryHandleAsync), "Exception occurred: {Message}", failedDependency.ErrorMessage);

                problemDetails = Utility.Sdk.Dtos.ApiResponse<object>.Default.ToFailedDependencyApiResponse(exception.Message);
                break;
            case LedgerException ledgerException:
            {
                _logger.LogError(
                    ledgerException, nameof(TryHandleAsync), "Exception occurred: {Message}", ledgerException.Errors());

                problemDetails = new Utility.Sdk.Dtos.ApiResponse<object>(ledgerException.DefaultUserMessage(),
                    ledgerException.StatusCode());
                break;
            }
            case RedisTimeoutException or RedisOperationException
                or PayStackException or ScheduleException or SchedulerException
                or ObjectStorageException
                or AmazonClientException:
            {
                _logger.LogError(
                    exception, nameof(TryHandleAsync), "Exception occurred: {Message}", exception.Message);

                problemDetails = Utility.Sdk.Dtos.ApiResponse<object>.Default.ToFailedDependencyApiResponse(exception.Message);
                break;
            }
            case RestException restException:
                _logger.LogError(
                    restException, nameof(TryHandleAsync), "Exception occurred: {Message}", restException.Message);

                problemDetails = Utility.Sdk.Dtos.ApiResponse<object>.Default.ToFailedDependencyApiResponse(
                    exception.Message);

                break;
            case TwitterException twitterException:
                problemDetails = new Utility.Sdk.Dtos.ApiResponse<object>(twitterException.Message,
                    twitterException.StatusCode());
                break;
            case Forbidden or ForbiddenObjectStorageAccessException:
            {
                problemDetails = Utility.Sdk.Dtos.ApiResponse<object>.Default.ToForbiddenApiResponse(exception.Message);
                break;
            }
            case NotFound or FileNotFoundException:
            {
                problemDetails = Utility.Sdk.Dtos.ApiResponse<object>.Default.ToNotFoundApiResponse(exception.Message);
                break;
            }
            case Unauthorized:
            {
                problemDetails = Utility.Sdk.Dtos.ApiResponse<object>.Default.ToUnAuthorizedApiResponse(exception.Message);
                break;
            }
            // case NoMatchedDistributors:
            // {
            //     problemDetails = Utility.Sdk.Dtos.ApiResponse<object>.Default.ToNotFoundApiResponse(exception.Message);
            //     break;
            // }
            case ApiException apiException:
                _logger.LogError(
                    exception, nameof(TryHandleAsync), "Exception occurred: {Message}", exception.Message);

                problemDetails = new Utility.Sdk.Dtos.ApiResponse<object>(apiException.Message,
                    (int)apiException.StatusCode);

                break;
            case PipelineException pipelineException:
                var effective = pipelineException.GetBaseException();
            
                return await TryHandleAsync(httpContext, effective, cancellationToken);
            default:
            {
                _logger.LogError(
                    exception, nameof(TryHandleAsync), "Exception occurred: {Message}", exception.Message);

                problemDetails = Utility.Sdk.Dtos.ApiResponse<object>.Default.ToServerErrorApiResponse(
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