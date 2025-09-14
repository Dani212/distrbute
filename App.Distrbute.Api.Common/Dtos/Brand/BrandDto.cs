using System.ComponentModel.DataAnnotations;
using App.Distrbute.Common.Enums;
using Utility.Sdk.Extensions;

namespace App.Distrbute.Api.Common.Dtos.brand;

public class BrandDto
{
    public string? Id { get; set; }
    public string? Email { get; set; }
    [Required] public string Name { get; set; } = null!;

    [Required(ErrorMessage = "'Bio' is required, this will help us better curate your ad content")]
    public string Bio { get; set; } = null!;

    [AttributeExtensions.DisallowedEnumValues((int)Niche.Default)]
    public List<Niche>? Niches { get; set; }

    public DocumentFileDto? ProfilePicture { get; set; }
    public BrandRole? Role { get; set; }

    public DateTime? CreatedAt { get; set; }
}