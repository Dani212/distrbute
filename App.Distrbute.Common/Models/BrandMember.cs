using System.ComponentModel.DataAnnotations;
using App.Distrbute.Common.Enums;

namespace App.Distrbute.Common.Models;

public class BrandMember : BrandResource
{
    [Required] public Email Email { get; set; } = null!;
    [Required] public BrandRole? Role { get; set; }
}