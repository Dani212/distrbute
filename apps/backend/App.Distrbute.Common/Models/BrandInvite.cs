using System.ComponentModel.DataAnnotations;
using App.Distrbute.Common.Enums;

namespace App.Distrbute.Common.Models;

public class BrandInvite : BrandResource
{
    public string? Email { get; set; }
    [Required] public BrandRole? Role { get; set; }
    [Required] public BrandInviteStatus? Status { get; set; } = BrandInviteStatus.Pending;
    [Required] public string Token { get; set; } = null!;
    [Required] public DateTime? Expires { get; set; }
}