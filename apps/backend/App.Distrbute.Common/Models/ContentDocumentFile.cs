using ObjectStorage.Sdk.Dtos;
using Socials.Sdk.Enums;

namespace App.Distrbute.Common.Models;

public class ContentDocumentFile : DocumentFile
{

    public string? PreferredCaption { get; set; }
}

public class UGCDocumentFile
{
    public string? Rules { get; set; }
}