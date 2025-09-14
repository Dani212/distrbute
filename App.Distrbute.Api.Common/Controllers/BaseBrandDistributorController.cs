using App.Distrbute.Api.Common.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ObjectStorage.Sdk.Dtos;
using ObjectStorage.Sdk.Extensions;
using ObjectStorage.Sdk.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using Utility.Sdk.Dtos;
using CommonConstants = ObjectStorage.Sdk.CommonConstants;

namespace App.Distrbute.Api.Common.Controllers;

public class BaseBrandDistributorController : CustomControllerBase
{
    private readonly IObjectStorageService _fileService;

    public BaseBrandDistributorController(IObjectStorageService fileService)
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

        var req = new ObjectStoreReq();
        req.SupportedFormats = CommonConstants.IMAGE_MEDIA_TYPES;
        req.MaxSize = CommonConstants.TWENTY_MB;
        req.ValidateOwnership = false;
        req.OwnerId = user.Address;
        req.PathPrefix = Distrbute.Common.CommonConstants.PROFILE_PICTURE_STORAGE_PREFIX;
        var presignedUploadUrlResp = await _fileService.GeneratePresignedUploadUrlAsync<DocumentFile>(req, metadata);

        return ToActionResult(presignedUploadUrlResp.ToOkApiResponse());
    }
}