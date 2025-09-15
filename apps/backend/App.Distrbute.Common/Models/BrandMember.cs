using System.ComponentModel.DataAnnotations;
using App.Distrbute.Common.Enums;
using Persistence.Sdk.Models;

namespace App.Distrbute.Common.Models;

public class BrandMember : BrandResource
{
    [ForeignKey]
    [Required] public Email Email { get; set; } = null!;
    [Required] public BrandRole? Role { get; set; }
}