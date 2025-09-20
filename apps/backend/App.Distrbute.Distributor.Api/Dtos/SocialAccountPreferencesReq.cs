using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Socials.Sdk.Enums;
using Utility.Sdk.Extensions;

namespace App.Distrbute.Distributor.Api.Dtos;

public class SocialAccountPreferencesReq
{
    [Required]
    [AttributeExtensions.ListRange(1, 3, true)]
    //Distributor Niche
    public List<string> Niches { get; set; } = new();

    [Required]
    [AttributeExtensions.ListRange(0, 100, true)]
    //Brand Niche
    public List<string> ExcludeFromNiches { get; set; } = new();

    [Required]
    [AttributeExtensions.ListRange(0, 100, true)]
    [AttributeExtensions.DisallowedEnumValues((int)ContentType.Default)]
    public List<ContentType> ExcludeFromContent { get; set; } = new();
    
    [Required] public bool IsDisabled { get; set; }
}