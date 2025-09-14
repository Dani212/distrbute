using System.ComponentModel.DataAnnotations;
using Socials.Sdk.Enums;
using Utility.Sdk.Extensions;

namespace App.Distrbute.Api.Common.Dtos.brand;

public class HandleDto
{
    [Required(AllowEmptyStrings = false)] public string Name { get; set; } = string.Empty;

    [Required]
    [AttributeExtensions.DisallowedEnumValues((int)Socials.Sdk.Enums.Platform.Default)]
    public Platform? Platform { get; set; }
}