using System.ComponentModel.DataAnnotations;

namespace App.Distrbute.Api.Common.Options;

public class BearerTokenConfig
{
    [Required(AllowEmptyStrings = false)] public string Audience { get; set; } = null!;
    [Required(AllowEmptyStrings = false)] public string Issuer { get; set; } = null!;
    [Required(AllowEmptyStrings = false)] public string SigningKey { get; set; } = null!;
}