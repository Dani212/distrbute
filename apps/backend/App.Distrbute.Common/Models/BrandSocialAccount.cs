using System.ComponentModel.DataAnnotations;
using Persistence.Sdk.Models;

namespace App.Distrbute.Common.Models;

public class BrandSocialAccount : SocialAccountBase
{
    [ForeignKey, Required]
    public Brand Brand { get; set; } = null!;
}