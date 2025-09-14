using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Utility.Sdk.Dtos;

namespace App.Distrbute.Api.Common.Controllers;

[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(EmptyApiResponse))]
[ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(EmptyApiResponse))]
[ProducesResponseType(StatusCodes.Status424FailedDependency, Type = typeof(EmptyApiResponse))]
[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(EmptyApiResponse))]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public abstract class CustomControllerBase : ControllerBase
{
    protected IActionResult ToActionResult<T>(IApiResponse<T> apiResponse)
    {
        return StatusCode(apiResponse.Code, apiResponse);
    }
}