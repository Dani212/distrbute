using App.Distrbute.Api.Common.Services.Interfaces;
using App.Distrbute.Common.Extensions;
using App.Distrbute.Common.Models;
using App.Distrbute.Common.Options;
using DataProtection.Sdk.Core;
using Microsoft.Extensions.Options;
using ObjectStorage.Sdk;
using ObjectStorage.Sdk.Dtos;
using ObjectStorage.Sdk.Extensions;
using ObjectStorage.Sdk.Services.Interfaces;
using Socials.Sdk.Enums;
using Utility.Sdk.Exceptions;

namespace App.Distrbute.Api.Common.Services.Providers;

public class FileUploadService : IFileUploadService
{
    private readonly IObjectStorageSdk _objectStorageSdk;
    private readonly IDataProtectionSdk _dataProtectionSdk;
    private readonly EncryptionConfig  _encryptionConfig;

    public FileUploadService(
        IObjectStorageSdk objectStorageSdk,
        IDataProtectionSdk dataProtectionSdk,
        IOptions<EncryptionConfig> encryptionConfig)
    {
        _objectStorageSdk = objectStorageSdk;
        _dataProtectionSdk = dataProtectionSdk;
        _encryptionConfig = encryptionConfig.Value;
    }
    
    public async Task<DocumentFile> UploadProfilePicture(Email principal, MultipartMetadata metadata)
    {
        var req = new ObjectStoreReq();
        req.SupportedFormats = CommonConstants.IMAGE_MEDIA_TYPES;
        req.MaxSize = CommonConstants.TWENTY_MB;
        req.ValidateOwnership = false;
        req.OwnerId = principal.Address;
        req.PathPrefix = Distrbute.Common.CommonConstants.PROFILE_PICTURE_STORAGE_PREFIX;
        var presignedUploadUrlResp = await _objectStorageSdk.GeneratePresignedUploadUrlAsync<DocumentFile>(req, metadata);
        
        return presignedUploadUrlResp;
    }

    public async Task<ContentDocumentFile> UploadContent(Email principal, MultipartMetadata metadata)
    {
        // content type
        var contentTypeField = nameof(ContentType).ToLower();
        metadata.FormData.TryGetValue(nameof(ContentType).ToLower(), out var contentTypeString);
        if (string.IsNullOrWhiteSpace(contentTypeString))
            throw new BadRequest($"Form field: {contentTypeField} is required.");
        var isValidContentType = Enum.TryParse(contentTypeString, out ContentType contentType) &&
                                 contentType != ContentType.Default;
        if (!isValidContentType)
            throw new BadRequest(
                $"Invalid content type: {contentTypeString}, must be one of [{string.Join(", ", ContentTypeExtensions.All)}]");

        // preferred caption field
        var preferredCaptionField = "PreferredCaption".ToLower();
        metadata.FormData.TryGetValue(preferredCaptionField, out var preferredCaption);

        var req = new ObjectStoreReq();
        req.SupportedFormats = CommonConstants.IMAGE_MEDIA_TYPES
            .Concat(CommonConstants.VIDEO_MEDIA_TYPES).ToList();
        req.MaxSize = CommonConstants.FIVE_HUNDRED_MB;
        req.ValidateOwnership = false;
        req.OwnerId = principal.Address;
        req.PathPrefix = Distrbute.Common.CommonConstants.CONTENT_STORAGE_PREFIX;

        var additionalMetadata = new
        {
            ContentType = contentType,
            PreferredCaption = preferredCaption
        };
        
        var presignedUploadUrlResp =
            await _objectStorageSdk.GeneratePresignedUploadUrlAsync<ContentDocumentFile>(req, metadata, additionalMetadata);
        
        return presignedUploadUrlResp;
    }

    private string EncryptUploadUrlWithSymmetricKey(string uploadUrl)
    {
        var symmetricKey = _encryptionConfig.SymmetricKey;
        var encryptedUrl = _dataProtectionSdk.EncryptWithSymmetricKey(uploadUrl, symmetricKey);
        
        return encryptedUrl;
    }
}