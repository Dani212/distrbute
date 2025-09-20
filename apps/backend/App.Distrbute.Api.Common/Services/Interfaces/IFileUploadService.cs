using App.Distrbute.Common.Models;
using ObjectStorage.Sdk.Dtos;
using ObjectStorage.Sdk.Extensions;

namespace App.Distrbute.Api.Common.Services.Interfaces;

public interface IFileUploadService
{
    Task<DocumentFile> UploadProfilePicture(Email principal, MultipartMetadata metadata);
    Task<ContentDocumentFile> UploadContent(Email principal, MultipartMetadata metadata);
}