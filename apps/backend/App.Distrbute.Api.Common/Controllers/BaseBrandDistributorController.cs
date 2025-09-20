using App.Distrbute.Api.Common.Extensions;
using App.Distrbute.Api.Common.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ObjectStorage.Sdk.Dtos;
using ObjectStorage.Sdk.Extensions;
using Swashbuckle.AspNetCore.Annotations;
using Utility.Sdk.Dtos;
using CommonConstants = ObjectStorage.Sdk.CommonConstants;

namespace App.Distrbute.Api.Common.Controllers;

public class BaseBrandDistributorController : CustomControllerBase
{
    private readonly IFileUploadService _fileService;

    public BaseBrandDistributorController(IFileUploadService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload-profile-picture")]
    [SwaggerOperation(
        Summary = "This endpoint uploads profile picture"
    )]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IApiResponse<PresignedUploadResponse>))]
    [RequestSizeLimit(CommonConstants.TWENTY_MB)]
    [RequestFormLimits(MultipartBodyLengthLimit = CommonConstants.TWENTY_MB)]
    public async Task<IActionResult> UploadProfilePicture()
    {
        var user = User.GetAccount();

        var cancellationToken = HttpContext.RequestAborted;
        var metadata = await HttpContext.Request.ReadMultipartMetadata(cancellationToken);

        var resp = await _fileService.UploadProfilePicture(user, metadata);

        return ToActionResult(resp.ToOkApiResponse());
    }
}